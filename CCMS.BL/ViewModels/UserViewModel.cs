using System.Threading.Tasks;
using CCMS.BL.Configurator;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Database;
using CCMS.BL.Services.GitHub;
using CCMS.BL.Services.GitHub.Exceptions;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels
{
    public class UserViewModel : BaseViewModel, IScoped
    {
        private readonly GitHubServiceFacade gitHubServiceFacade;
        private readonly IMessenger messenger;
        private readonly UserDataService userDataService;
        private string accessToken;

        private UserListModel selectedUser;

        public UserViewModel(GitHubServiceFacade gitHubServiceFacade,
            UserDataService userDataService, IMessenger messenger)
        {
            this.gitHubServiceFacade = gitHubServiceFacade;
            this.userDataService = userDataService;
            this.messenger = messenger;

            messenger.Register<AccessTokenAskMessage>(this, AccessTokenAskMessageReceived);
            messenger.Register<SelectedUserAskMessage>(this, SelectedUserAskMessageReceived);
        }

        public string AccessToken
        {
            get => accessToken;
            set
            {
                accessToken = value;
                messenger.Send(new AccessTokenChangedMessage(value));
            }
        }

        public UserListModel SelectedUser
        {
            get => selectedUser;
            set
            {
                SetPropertyValue(ref selectedUser, value);
                messenger.Send(new SelectedUserChangedMessage(value));
            }
        }

        private void AccessTokenAskMessageReceived(AccessTokenAskMessage _)
        {
            messenger.Send(new AccessTokenChangedMessage(accessToken));
        }

        private void SelectedUserAskMessageReceived(SelectedUserAskMessage _)
        {
            messenger.Send(new SelectedUserChangedMessage(selectedUser));
        }

        public async Task<UserListModel> InitUser()
        {
            var user = await gitHubServiceFacade.GetCurrentUser(AccessToken);
            SelectedUser = await userDataService.InsertOrUpdate(user);
            return SelectedUser;
        }

        public async Task<bool> HasAccessToRepository(string owner, string repository)
        {
            try
            {
                var repositoryModel = await gitHubServiceFacade.GetRepository(AccessToken, owner, repository);
                return await HasAccessToRepository(repositoryModel);
            }
            catch (RepositoryNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> HasAccessToRepository(RepositoryListModel repositoryModel)
        {
            if (repositoryModel == null) return false;
            UserListModel user;
            if (SelectedUser == null)
                user = await InitUser();
            else
                user = SelectedUser;
            return await gitHubServiceFacade.HasUserWriteAccess(AccessToken, user, repositoryModel);
        }

        public async Task<bool> HasAccessToConvention(ConventionDetailModel convention)
        {
            return await HasAccessToRepository(convention.Repository);
        }
    }
}