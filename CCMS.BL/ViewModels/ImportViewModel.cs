using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.BL.Configurator;
using CCMS.BL.Models.Detail;
using CCMS.BL.Services.EditorConfig;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using CCMS.BL.ViewModels.Observables;
using GalaSoft.MvvmLight.Messaging;
using HtmlAgilityPack;

namespace CCMS.BL.ViewModels
{
    public class ImportViewModel : BaseViewModel, IScoped, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private bool append = true;
        private ObservableCollection<ExecuteMessageObservable> executeMessages = new();
        private ICollection<ValuablePropertyObservable> properties = new List<ValuablePropertyObservable>();
        private string selectedBranch;
        private int step;
        private bool nextStepAllowed;

        public ImportViewModel(IMessenger messenger, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            messenger.Register<AccessTokenChangedMessage>(this, AccessTokenChangedMessageReceived);
            messenger.Send(new AccessTokenAskMessage());

            messenger.Register<SelectedRepositoryChangedMessage>(this, SelectedRepositoryChangedMessageReceived);
            messenger.Send(new SelectedRepositoryAskMessage());
        }

        public CancellationTokenSource CancellationTokenSource = new();

        public string AccessToken { get; private set; }
        public Importer Importer { get; private set; }
        public string UnsetName => "unset";
        public string NoneName => "none";

        public RepositoryDetailModel SelectedRepository { get; private set; }

        public int Step
        {
            get => step;
            set =>
                SetPropertyValue(ref step, value);
        }

        public bool NextStepAllowed
        {
            get => nextStepAllowed;
            set => SetPropertyValue(ref nextStepAllowed, value);
        }

        public bool Append
        {
            get => append;
            set => SetPropertyValue(ref append, value);
        }

        public string SelectedBranch
        {
            get => selectedBranch;
            set => SetPropertyValue(ref selectedBranch, value);
        }

        public ICollection<ValuablePropertyObservable> Properties
        {
            get => properties;
            set => SetPropertyValue(ref properties, value);
        }

        public ObservableCollection<ExecuteMessageObservable> ExecuteMessages
        {
            get => executeMessages;
            set => SetPropertyValue(ref executeMessages, value);
        }

        public ObservableCollection<ImportResultObservable> Results { get; } = new();

        private void SelectedRepositoryChangedMessageReceived(SelectedRepositoryChangedMessage m)
        {
            SelectedRepository = m.SelectedRepository;
        }

        public void AddProperty(ICollection<IProperty> allProperties)
        {
            Properties.Add(new ValuablePropertyObservable(allProperties, allProperties.FirstOrDefault(), "*"));
            NextStepAllowed = true;
        }

        public void AddOneOfAllProperties(ICollection<IProperty> allProperties)
        {
            foreach (var property in allProperties)
            {
                if (Properties.Any(p => p.Property == property)) continue;
                Properties.Add(new ValuablePropertyObservable(allProperties, property, ((IImportable) property).DefaultSection));
                NextStepAllowed = true;
            }
        }

        public void StepChanged(int s)
        {
            NextStepAllowed = false;
            Step = s;
            if (Step == 1)
            {
                Importer = new Importer(serviceProvider, CancellationTokenSource.Token, AccessToken, ExecuteMessages);
                Importer.MessageChanged += ((_, _) => OnPropertyChanged(nameof(ExecuteMessages)));
                Importer.ImportFinished += ((_, _) => ImportFinished());
                ((Func<Task>) (async () => await Importer.Execute(SelectedBranch, SelectedRepository, Properties))).Invoke();
            }
        }

        private void ImportFinished()
        {
            Results.Clear();
            foreach (var (key, value) in Importer.Result)
            {
                var observable = new ImportResultObservable()
                {
                    Property = key,
                    ImportValues = value.Result.Select(i => new ImportResultObservable.ImportValueObservable
                            {Section = i.Key, Ratio = i.Value})
                        .OrderByDescending(v => v.Ratio).ToList()
                };
                observable.ImportValues.Add(new ImportResultObservable.ImportValueObservable {Section = UnsetName});
                observable.ImportValues.Add(new ImportResultObservable.ImportValueObservable {Section = NoneName});
                observable.SelectedValue = observable.ImportValues.FirstOrDefault()?.Section;
                Results.Add(observable);
            }

            NextStepAllowed = true;
        }

        public void Reset()
        {
            Step = 0;
            NextStepAllowed = true;
            Append = true;
            Properties = new List<ValuablePropertyObservable>();
            ExecuteMessages = new ObservableCollection<ExecuteMessageObservable>();
            CancellationTokenSource = new CancellationTokenSource();
        }

        private void AccessTokenChangedMessageReceived(AccessTokenChangedMessage m)
        {
            AccessToken = m.AccessToken;
        }

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
            Importer?.Dispose();
        }

        public void Close()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            Importer?.Dispose();
        }

        public void DeleteProperty(ValuablePropertyObservable property)
        {
            Properties.Remove(property);
            if (!Properties.Any()) NextStepAllowed = false;
        }

        public async Task<string> Save(string formalText)
        {
            var groups = Results.Where(r => r.SelectedValue != NoneName)
                .GroupBy(i => i.Property.Value)
                .ToList().OrderBy(g => g.Key.Length);
            var result = Append ? formalText + "\n" : string.Empty;
            foreach (var resultGroup in groups)
            {
                result += $"[{resultGroup.Key}]\n";
                result = resultGroup.Aggregate(result, (current, importResultObservable) => current + $"{importResultObservable.Property.Name} = {importResultObservable.SelectedValue}\n");
                result += "\n";
            }
            return result;
        }
    }
}