using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.BL.Models.Detail;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.GitHub;
using CCMS.BL.ViewModels.Observables;

namespace CCMS.BL.Services.EditorConfig.Import
{
    public class Importer : IDisposable
    {
        private readonly string accessToken;
        private readonly CancellationToken cancellationToken;
        private readonly GitHubServiceFacade gitHubServiceFacade;
        private readonly RoslynInteropService roslynInteropService;

        public Importer(IServiceProvider serviceProvider, CancellationToken cancellationToken, string accessToken,
            ObservableCollection<ExecuteMessageObservable> executeMessages)
        {
            ExecuteMessages = executeMessages;
            this.cancellationToken = cancellationToken;
            this.accessToken = accessToken;
            gitHubServiceFacade = (GitHubServiceFacade) serviceProvider.GetService(typeof(GitHubServiceFacade));
            roslynInteropService = (RoslynInteropService) serviceProvider.GetService(typeof(RoslynInteropService));
        }

        public ObservableCollection<ExecuteMessageObservable> ExecuteMessages { get; }

        public GitHubContent Content { get; private set; }

        public Dictionary<ValuablePropertyObservable, ImportResult> Result { get; private set; }

        public void Dispose()
        {
            Content?.Dispose();
        }

        public event EventHandler MessageChanged;
        public event EventHandler ImportFinished;

        protected void OnMessageChanged()
        {
            MessageChanged?.Invoke(this, new EventArgs());
        }

        protected void OnImportFinished()
        {
            ImportFinished?.Invoke(this, new EventArgs());
        }

        public async Task Execute(string branch, RepositoryDetailModel selectedRepository,
            ICollection<ValuablePropertyObservable> properties)
        {
            var lastMessage = new ExecuteMessageObservable("Initializing")
                {State = ExecuteMessageObservable.EState.Success};
            ExecuteMessages.Add(lastMessage);
            if (cancellationToken.IsCancellationRequested) return;

            lastMessage = new ExecuteMessageObservable("Branch content downloading");
            ExecuteMessages.Add(lastMessage);
            Content = await gitHubServiceFacade.GetContent(accessToken, selectedRepository.GitHubId, branch);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Content investigating");
            ExecuteMessages.Add(lastMessage);
            var investigatedProperties = Investigate(lastMessage, properties);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Importing");
            ExecuteMessages.Add(lastMessage);
            var results = Import(investigatedProperties, lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Merging and normalizing results");
            ExecuteMessages.Add(lastMessage);
            Result = MergeAndNormalize(results, lastMessage);
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Cleaning up");
            ExecuteMessages.Add(lastMessage);
            Cleanup();
            if (cancellationToken.IsCancellationRequested) return;
            lastMessage.State = ExecuteMessageObservable.EState.Success;

            lastMessage = new ExecuteMessageObservable("Import finished")
                {State = ExecuteMessageObservable.EState.Success};
            ExecuteMessages.Add(lastMessage);
            OnImportFinished();
        }

        private IDictionary<ValuablePropertyObservable, ICollection<string>> Investigate(
            ExecuteMessageObservable message, ICollection<ValuablePropertyObservable> properties)
        {
            var files = Directory.GetFiles(Content.ContentDirectory.FullName, "*", SearchOption.AllDirectories)
                .ToList();
            files = files.Select(f => Path.GetRelativePath(Content.ContentDirectory.FullName, f).Replace("\\", "/"))
                .ToList();
            var count = files.Count;
            var i = 0;
            var investigatedProperties = properties.ToDictionary(x => x, x => (ICollection<string>) new List<string>());

            foreach (var file in files)
            {
                foreach (var property in properties)
                    if (roslynInteropService.IsSectionMatchingFile(property.Value, Path.Combine("/", file)))
                        investigatedProperties[property].Add(file);
                i++;
                message.Text = $"Content investigating {100 * i / count}%";
                OnMessageChanged();
            }

            message.Text = "Content investigating";
            OnMessageChanged();
            return investigatedProperties;
        }

        private Dictionary<ValuablePropertyObservable, ICollection<ImportResult>> Import(
            IDictionary<ValuablePropertyObservable, ICollection<string>> properties,
            ExecuteMessageObservable message)
        {
            var results = new Dictionary<ValuablePropertyObservable, ICollection<ImportResult>>();
            var count = properties.Values.ToList().Select(c => c.Count).Sum();
            var i = 0;

            foreach (var (key, value) in properties)
            {
                results.Add(key, new List<ImportResult>());
                foreach (var file in value)
                {
                    if (key.Property is not IImportable importableProperty)
                        throw new NotSupportedException(nameof(importableProperty));
                    var importResult =
                        importableProperty.Import(new FileInfo(Path.Combine(Content.ContentDirectory.FullName, file)));
                    results[key].Add(importResult);

                    i++;
                    message.Text = $"Importing {100 * i / count}%";
                    OnMessageChanged();
                }
            }

            message.Text = "Importing";
            OnMessageChanged();
            return results;
        }

        private Dictionary<ValuablePropertyObservable, ImportResult> MergeAndNormalize(
            Dictionary<ValuablePropertyObservable, ICollection<ImportResult>> results,
            ExecuteMessageObservable message)
        {
            var result = new Dictionary<ValuablePropertyObservable, ImportResult>();
            var count = results.Count;
            var i = 0;

            foreach (var (valuablePropertyObservable, importResults) in results)
            {
                var subResult = new Dictionary<string, double>();
                foreach (var importResult in importResults)
                foreach (var (key, value) in importResult.Result)
                    if (subResult.ContainsKey(key)) subResult[key] += value;
                    else subResult[key] = value;
                var sum = subResult.Values.Sum();
                foreach (var (key, value) in subResult) subResult[key] = 100 * value / sum;

                result.Add(valuablePropertyObservable, new ImportResult {Result = subResult});

                i++;
                message.Text = $"Merging and normalizing results {100 * i / count}%";
                OnMessageChanged();
            }

            message.Text = "Merging and normalizing results";
            OnMessageChanged();
            return result;
        }

        private void Cleanup()
        {
            Content.Dispose();
            Content = null;
        }
    }
}