using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.BL.Configurator;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Database;
using CCMS.BL.Services.GitHub;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels
{
    public class RepositoriesViewModel : BaseViewModel, IScoped
    {
        private readonly GitHubServiceFacade gitHubServiceFacade;
        private readonly IMessenger messenger;
        private readonly RepositoryDataService repositoryDataService;
        private ICollection<RepositoryListModel> allRepositories;
        private ICollection<RepositoryListModel> filteredRepositories = new List<RepositoryListModel>();
        private ICollection<RepositoryListModel> visibleRepositories = new List<RepositoryListModel>();

        public RepositoriesViewModel(GitHubServiceFacade gitHubServiceFacade,
            RepositoryDataService repositoryDataService, IMessenger messenger)
        {
            this.gitHubServiceFacade = gitHubServiceFacade;
            this.repositoryDataService = repositoryDataService;
            this.messenger = messenger;

            messenger.Register<AccessTokenChangedMessage>(this, AccessTokenChangedMessageReceived);
            messenger.Send(new AccessTokenAskMessage());
        }

        public string AccessToken { get; private set; }

        public int PageSize => 5;

        public ICollection<RepositoryListModel> AllRepositories
        {
            get => allRepositories;
            set
            {
                value = value.OrderByDescending(r => r.UpdatedAt).ToList();
                SetPropertyValue(ref allRepositories, value);
                FilteredRepositories = AllRepositories;
            }
        }

        public ICollection<RepositoryListModel> VisibleRepositories
        {
            get => visibleRepositories;
            set => SetPropertyValue(ref visibleRepositories, value);
        }

        public ICollection<RepositoryListModel> FilteredRepositories
        {
            get => filteredRepositories;
            set
            {
                SetPropertyValue(ref filteredRepositories, value);
                VisibleRepositories = FilteredRepositories.Take(PageSize).ToList();
            }
        }

        private void AccessTokenChangedMessageReceived(AccessTokenChangedMessage m)
        {
            AccessToken = m.AccessToken;
        }

        public async Task InitializeRepositories()
        {
            var repositories = await gitHubServiceFacade.GetAllRepositoriesForCurrentUser(AccessToken);
            AllRepositories = await repositoryDataService.Bind(repositories);
        }
    }
}