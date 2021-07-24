using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.BL.Configurator;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Database;
using CCMS.BL.Services.GitHub;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels
{
    public class RepositoryViewModel : BaseViewModel, IScoped
    {
        private readonly ConventionDataService conventionDataService;
        private readonly GitHubServiceFacade gitHubServiceFacade;
        private readonly IMessenger messenger;
        private readonly RepositoryDataService repositoryDataService;
        private ICollection<ConventionListModel> allConventions;
        private ICollection<ConventionListModel> filteredConventions = new List<ConventionListModel>();

        private RepositoryDetailModel selectedRepository;
        private ICollection<ConventionListModel> visibleConventions = new List<ConventionListModel>();

        public RepositoryViewModel(GitHubServiceFacade gitHubServiceFacade, RepositoryDataService repositoryDataService,
            ConventionDataService conventionDataService, IMessenger messenger)
        {
            this.gitHubServiceFacade = gitHubServiceFacade;
            this.repositoryDataService = repositoryDataService;
            this.conventionDataService = conventionDataService;
            this.messenger = messenger;

            messenger.Register<SelectRepositoryMessage>(this,
                async m => SelectedRepository = await repositoryDataService.Get(m.RepositoryId));

            messenger.Register<AccessTokenChangedMessage>(this, AccessTokenChangedMessageReceived);
            messenger.Send(new AccessTokenAskMessage());

            messenger.Register<SelectedUserChangedMessage>(this, SelectedUserChangedMessageReceived);
            messenger.Send(new SelectedUserAskMessage());

            messenger.Register<SelectedRepositoryAskMessage>(this, SelectedRepositoryAskMessageReceived);
        }

        public UserListModel SelectedUser { get; private set; }

        public string AccessToken { get; private set; }

        public RepositoryDetailModel SelectedRepository
        {
            get => selectedRepository;
            set
            {
                SetPropertyValue(ref selectedRepository, value);
                messenger.Send(new SelectedRepositoryChangedMessage(SelectedRepository));
                AllConventions = SelectedRepository.Conventions;
                GetBranches();
            }
        }

        public int PageSize => 5;

        public ICollection<ConventionListModel> AllConventions
        {
            get => allConventions;
            set
            {
                value = value.OrderByDescending(c => c.UpdatedAt).ToList();
                SetPropertyValue(ref allConventions, value);
                FilteredConventions = AllConventions;
            }
        }

        public ICollection<ConventionListModel> VisibleConventions
        {
            get => visibleConventions;
            set => SetPropertyValue(ref visibleConventions, value);
        }

        public ICollection<ConventionListModel> FilteredConventions
        {
            get => filteredConventions;
            set
            {
                SetPropertyValue(ref filteredConventions, value);
                VisibleConventions = FilteredConventions.Take(PageSize).ToList();
            }
        }

        private void SelectedRepositoryAskMessageReceived(SelectedRepositoryAskMessage _)
        {
            messenger.Send(new SelectedRepositoryChangedMessage(SelectedRepository));
        }

        private void SelectedUserChangedMessageReceived(SelectedUserChangedMessage m)
        {
            SelectedUser = m.SelectedUser;
        }

        private void AccessTokenChangedMessageReceived(AccessTokenChangedMessage m)
        {
            AccessToken = m.AccessToken;
        }

        public async Task SetSelectedRepository(string ownerName, string repositoryName)
        {
            var repository = await gitHubServiceFacade.GetRepository(AccessToken, ownerName, repositoryName);
            SelectedRepository = await repositoryDataService.InsertOrGet(repository);
        }

        public async Task AddConvention(string title)
        {
            var conventionItem = await conventionDataService.Insert(SelectedRepository, SelectedUser, title);
            var tmp = AllConventions;
            tmp.Add(conventionItem);
            AllConventions = tmp;
            SelectedRepository.Conventions = AllConventions;
        }

        public async Task<ICollection<BranchListModel>> GetBranches()
        {
            return await gitHubServiceFacade.GetAllBranches(AccessToken, SelectedRepository.GitHubId);
        }
    }
}