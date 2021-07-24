using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Database;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.GitHub;
using CCMS.BL.Services.Helpers;
using CCMS.BL.ViewModels.Observables;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.BL.Services.EditorConfig.Check
{
    //EditorConfig parsing part inspired by roslyn AnalyzerConfig class
    public class Checker : IDisposable
    {
        private readonly string accessToken;
        private readonly CancellationToken cancellationToken;
        private readonly CheckDataService checkDataService;
        private readonly GitHubServiceFacade gitHubServiceFacade;
        private readonly IEnumerable<IProperty> properties;

        private readonly Regex propertyMatcher =
            new(@"^\s*([\w\.\-_]+)\s*[=:]\s*(.*?)\s*([#;].*)?$", RegexOptions.Compiled);

        private readonly string PropertyNotFound = "Property could not be found.";
        private readonly string PropertyUnsupportedValue = "Unsupported value.";
        private readonly RoslynInteropService roslynInteropService;
        private readonly Regex sectionMatcher = new(@"^\s*\[(([^#;]|\\#|\\;)+)\]\s*([#;].*)?$", RegexOptions.Compiled);

        public Checker(IServiceProvider serviceProvider, CancellationToken cancellationToken, string accessToken,
            ObservableCollection<ExecuteMessageObservable> executeMessages)
        {
            ExecuteMessages = executeMessages;
            this.cancellationToken = cancellationToken;
            this.accessToken = accessToken;

            gitHubServiceFacade = (GitHubServiceFacade) serviceProvider.GetService(typeof(GitHubServiceFacade));
            roslynInteropService = (RoslynInteropService) serviceProvider.GetService(typeof(RoslynInteropService));
            checkDataService = (CheckDataService) serviceProvider.GetService(typeof(CheckDataService));
            properties = serviceProvider.GetServices<IProperty>().Where(p => p is ICheckable).ToList();
        }

        public RepositoryCheckListModel Result { get; private set; }

        public ObservableCollection<ExecuteMessageObservable> ExecuteMessages { get; }
        public GitHubContent Content { get; private set; }

        public void Dispose()
        {
            Content?.Dispose();
        }

        public event EventHandler MessageChanged;
        public event EventHandler CheckFinished;

        protected void OnMessageChanged()
        {
            MessageChanged?.Invoke(this, new EventArgs());
        }

        protected void OnCheckFinished()
        {
            CheckFinished?.Invoke(this, new EventArgs());
        }

        public async Task Execute(string branch, RepositoryDetailModel selectedRepository, UserListModel currentUser,
            IEnumerable<ConventionListModel> selectedConventions)
        {
            var lastMessage = new ExecuteMessageObservable("Initializing")
                {State = ExecuteMessageObservable.EState.Success};
            ExecuteMessages.Add(lastMessage);
            if (cancellationToken.IsCancellationRequested) return;

            lastMessage = new ExecuteMessageObservable("Parsing formal texts");
            ExecuteMessages.Add(lastMessage);
            var rules = GetRules(selectedConventions.ToList(), lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Branch content downloading");
            ExecuteMessages.Add(lastMessage);
            Content = await gitHubServiceFacade.GetContent(accessToken, selectedRepository.GitHubId, branch);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Connecting rules to the files");
            ExecuteMessages.Add(lastMessage);
            rules = Connect(rules.ToList(), lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Checking");
            ExecuteMessages.Add(lastMessage);
            rules = Check(rules.ToList(), lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Creating results");
            ExecuteMessages.Add(lastMessage);
            Result = CreateResult(rules, selectedConventions, selectedRepository, currentUser, lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Saving results");
            ExecuteMessages.Add(lastMessage);
            Result = await checkDataService.Insert(Result);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Check finished")
                {State = ExecuteMessageObservable.EState.Success};
            ExecuteMessages.Add(lastMessage);
            OnCheckFinished();
        }

        private IEnumerable<CheckRule> GetRules(ICollection<ConventionListModel> conventionListModels,
            ExecuteMessageObservable message)
        {
            var count = conventionListModels.Sum(c => c.FormalText.ToLines().Count());
            var i = -1;

            foreach (var conventionListModel in conventionListModels)
            {
                var section = "*";
                var lineNumber = 0;
                foreach (var line in conventionListModel.FormalText.ToLines())
                {
                    i++;
                    message.Text = $"Parsing formal texts {100 * i / count}%";
                    OnMessageChanged();

                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (IsComment(line)) continue;

                    var sectionMatches = sectionMatcher.Matches(line);
                    if (sectionMatches.Count > 0 && sectionMatches[0].Groups.Count > 0)
                        section = sectionMatches[0].Groups[1].Value;

                    var propertyMatches = propertyMatcher.Matches(line);
                    if (propertyMatches.Count > 0 && propertyMatches[0].Groups.Count > 1)
                    {
                        var key = propertyMatches[0].Groups[1].Value.ToLower();
                        var value = propertyMatches[0].Groups[2].Value.ToLower();
                        var property = properties.FirstOrDefault(p => p.Name.ToLower() == key);

                        yield return new CheckRule
                        {
                            Convention = conventionListModel,
                            Property = property,
                            Section = section,
                            Value = value,
                            Line = lineNumber
                        };
                    }
                }
            }

            message.Text = "Parsing formal texts";
            OnMessageChanged();
        }

        private static bool IsComment(string line)
        {
            return (from c in line where !char.IsWhiteSpace(c) select c is '#' or ';').FirstOrDefault();
        }

        private IEnumerable<CheckRule> Connect(ICollection<CheckRule> rules, ExecuteMessageObservable message)
        {
            var files = Directory.GetFiles(Content.ContentDirectory.FullName, "*", SearchOption.AllDirectories)
                .ToList();
            files = files.Select(f => Path.GetRelativePath(Content.ContentDirectory.FullName, f).Replace("\\", "/"))
                .ToList();
            var count = rules.Count() * files.Count;
            var i = 0;

            foreach (var file in files)
            {
                var usedRules = new List<CheckRule>();
                foreach (var rule in rules)
                {
                    if (roslynInteropService.IsSectionMatchingFile(rule.Section, Path.Combine("/", file)))
                    {
                        if (usedRules.Any(r => r.Convention == rule.Convention && r.Property == rule.Property))
                        {
                            var usedRule = usedRules.First(r =>
                                r.Convention == rule.Convention && r.Property == rule.Property);
                            usedRule.Results.Remove(usedRule.Results.First(r => r.file == file));
                            usedRules.Remove(usedRule);
                        }

                        if (rule.Value.ToLower() != "unset")
                        {
                            rule.Results.Add((new CheckResult(), file));
                            usedRules.Add(rule);
                        }
                    }

                    i++;
                    message.Text = $"Connecting rules to the files {100 * i / count}%";
                    OnMessageChanged();
                }
            }

            message.Text = "Connecting rules to the files";
            OnMessageChanged();
            return rules;
        }

        private IEnumerable<CheckRule> Check(ICollection<CheckRule> rules, ExecuteMessageObservable message)
        {
            var count = rules.Sum(r => r.Results.Count);
            var i = 0;

            foreach (var rule in rules)
            foreach (var (result, file) in rule.Results)
            {
                if (rule.Property == null)
                {
                    result.Message = PropertyNotFound;
                    result.State = ECheckState.Fail;
                }
                else
                {
                    try
                    {
                        var tmp = ((ICheckable) rule.Property)?.Check(rule.Value,
                            new FileInfo(Path.Combine(Content.ContentDirectory.FullName, file)));
                        result.Message = tmp.Message;
                        result.State = tmp.State;
                    }
                    catch (UnsupportedPropertyValueException e)
                    {
                        result.Message = PropertyUnsupportedValue;
                        result.State = ECheckState.Fail;
                    }
                }

                i++;
                message.Text = $"Checking {100 * i / count}%";
                OnMessageChanged();
            }

            message.Text = "Checking";
            OnMessageChanged();
            return rules;
        }

        private RepositoryCheckListModel CreateResult(IEnumerable<CheckRule> rules,
            IEnumerable<ConventionListModel> conventions, RepositoryDetailModel repository, UserListModel user,
            ExecuteMessageObservable message)
        {
            var count = rules.Sum(r => r.Results.Count);
            var i = 0;

            var repositoryCheck = new RepositoryCheckListModel
            {
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = user,
                Repository = repository
            };
            var conventionChecks = new List<(ConventionListModel, ConventionCheckListModel)>();
            conventions.ToList().ForEach(c =>
            {
                var conventionCheckListModel = new ConventionCheckListModel {Title = c.Title};
                conventionChecks.Add((c, conventionCheckListModel));
                repositoryCheck.ConventionChecks.Add(conventionCheckListModel);
            });
            foreach (var conventionCheck in conventionChecks)
            {
                var j = 0;
                if (string.IsNullOrEmpty(conventionCheck.Item1.FormalText.Trim()))
                {
                    j++;
                    conventionCheck.Item2.Results.Add(new ResultCheckListModel
                    {
                        LineId = j,
                        Line = "Sorry, there is no text to show.",
                        State = ECheckState.None,
                        Message = string.Empty
                    });
                    continue;
                }

                foreach (var line in conventionCheck.Item1.FormalText.ToLines())
                {
                    j++;
                    conventionCheck.Item2.Results.Add(new ResultCheckListModel
                    {
                        LineId = j,
                        Line = line,
                        State = ECheckState.None,
                        Message = string.Empty
                    });
                }

                foreach (var rule in rules.Where(r => r.Convention == conventionCheck.Item1))
                {
                    var check = conventionCheck.Item2.Results.FirstOrDefault(c => c.LineId == rule.Line);
                    if (rule.Results.All(
                            r => r.result.State == ECheckState.Fail && r.result.Message == PropertyNotFound) &&
                        rule.Results.Any())
                    {
                        check.State = ECheckState.Fail;
                        check.Message = PropertyNotFound;
                        i += rule.Results.Count();
                        message.Text = $"Creating results {100 * i / count}%";
                        OnMessageChanged();
                        continue;
                    }

                    if (rule.Results.All(r =>
                            r.result.State == ECheckState.Fail && r.result.Message == PropertyUnsupportedValue) &&
                        rule.Results.Any())
                    {
                        check.State = ECheckState.Fail;
                        check.Message = PropertyUnsupportedValue;
                        i += rule.Results.Count();
                        message.Text = $"Creating results {100 * i / count}%";
                        OnMessageChanged();
                        continue;
                    }

                    foreach (var result in rule.Results)
                    {
                        if (result.result.State == ECheckState.Fail)
                        {
                            check.State = ECheckState.Fail;
                            check.Message =
                                $"{check.Message}{result.file}: {result.result.Message}{Environment.NewLine}";
                        }

                        if (result.result.State == ECheckState.Success && check.State == ECheckState.None)
                            check.State = ECheckState.Success;
                        i++;
                        message.Text = $"Creating results {100 * i / count}%";
                        OnMessageChanged();
                    }
                }
            }

            message.Text = "Creating results";
            OnMessageChanged();
            return repositoryCheck;
        }
    }
}