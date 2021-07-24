using CCMS.BL.Configurator;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services;
using CCMS.BL.Services.Database;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.Helpers;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using CCMS.BL.ViewModels.Observables;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CCMS.BL.ViewModels
{
    public class CheckerViewModel : BaseViewModel, IScoped, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly CheckDataService checkDataService;
        private ObservableCollection<ExecuteMessageObservable> executeMessages = new();
        private string selectedBranch;
        private int step;
        private bool nextStepAllowed;

        public CheckerViewModel(IMessenger messenger, IServiceProvider serviceProvider, CheckDataService checkDataService)
        {
            this.serviceProvider = serviceProvider;
            this.checkDataService = checkDataService;

            messenger.Register<AccessTokenChangedMessage>(this, AccessTokenChangedMessageReceived);
            messenger.Send(new AccessTokenAskMessage());

            messenger.Register<SelectedRepositoryChangedMessage>(this, SelectedRepositoryChangedMessageReceived);
            messenger.Send(new SelectedRepositoryAskMessage());

            messenger.Register<SelectedUserChangedMessage>(this, SelectedUserChangedMessageReceived);
            messenger.Send(new SelectedUserAskMessage());
        }

        public CancellationTokenSource CancellationTokenSource = new();
        private IEnumerable<ConventionListModel> selectedConventions;
        private RepositoryDetailModel selectedRepository;
        private ICollection<ConventionCheckListModel> results;

        private ICollection<RepositoryCheckListModel> allChecks =
            new List<RepositoryCheckListModel>();

        private ICollection<RepositoryCheckListModel> visibleCheck =
            new List<RepositoryCheckListModel>();

        public string AccessToken { get; private set; }

        public Checker Checker { get; private set; }

        public int PageSize => 1;

        public ICollection<RepositoryCheckListModel> AllChecks
        {
            get => allChecks;
            set
            {
                SetPropertyValue(ref allChecks, value);
                VisibleCheck = AllChecks.Take(PageSize).ToList();
            }
        }

        public ICollection<RepositoryCheckListModel> VisibleCheck
        {
            get => visibleCheck;
            set => SetPropertyValue(ref visibleCheck, value);
        }

        public RepositoryDetailModel SelectedRepository
        {
            get => selectedRepository;
            private set => SetPropertyValue(ref selectedRepository, value);
        }

        public IEnumerable<ConventionListModel> SelectedConventions
        {
            get => selectedConventions;
            set => SetPropertyValue(ref selectedConventions, value);
        }

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

        public string SelectedBranch
        {
            get => selectedBranch;
            set => SetPropertyValue(ref selectedBranch, value);
        }

        public ObservableCollection<ExecuteMessageObservable> ExecuteMessages
        {
            get => executeMessages;
            set => SetPropertyValue(ref executeMessages, value);
        }

        public ICollection<ConventionCheckListModel> Results
        {
            get => results;
            private set => SetPropertyValue(ref results, value);
        }

        public UserListModel SelectedUser { get; private set; }

        public void StepChanged(int s)
        {
            NextStepAllowed = false;
            Step = s;
            if (Step == 1)
            {
                Checker = new Checker(serviceProvider, CancellationTokenSource.Token, AccessToken, ExecuteMessages);
                Checker.MessageChanged += ((_, _) => OnPropertyChanged(nameof(ExecuteMessages)));
                Checker.CheckFinished += ((_, _) => CheckFinished());
                ((Func<Task>)(async () => await Checker.Execute(SelectedBranch, SelectedRepository, SelectedUser, SelectedConventions))).Invoke();
            }
        }

        private void CheckFinished()
        {
            Results = Checker.Result.ConventionChecks.OrderBy(c => c.Title).ToList();
            NextStepAllowed = true;
        }

        private void SelectedUserChangedMessageReceived(SelectedUserChangedMessage m)
        {
            SelectedUser = m.SelectedUser;
        }

        private void SelectedRepositoryChangedMessageReceived(SelectedRepositoryChangedMessage m)
        {
            SelectedRepository = m.SelectedRepository;
        }

        private void AccessTokenChangedMessageReceived(AccessTokenChangedMessage m)
        {
            AccessToken = m.AccessToken;
        }

        public void Reset()
        {
            Step = 0;
            ExecuteMessages = new ObservableCollection<ExecuteMessageObservable>();
            CancellationTokenSource = new CancellationTokenSource();
            SelectedConventions = SelectedRepository.Conventions;
            NextStepAllowed = selectedConventions.Any();
        }

        public void Close()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            Checker?.Dispose();
        }

        public void SelectedConventionsChanged()
        {
            NextStepAllowed = selectedConventions.Any();
        }

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
            Checker?.Dispose();
        }

        public async Task UpdateChecksHistory()
        {
            var tmp = (await checkDataService.Get(SelectedRepository)).OrderByDescending(c => c.CreatedAt).ToList();
            tmp.ForEach(r => r.ConventionChecks = r.ConventionChecks.OrderBy(c => c.Title).ToList());
            AllChecks = tmp;
        }
    }
}