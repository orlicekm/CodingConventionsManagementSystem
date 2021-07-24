using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.List;
using CCMS.BL.Services.GitHub.Base;
using Octokit;

namespace CCMS.BL.Services.GitHub
{
    internal class GitHubUserService : BaseGitHubService
    {
        public GitHubUserService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<bool> HasUserWriteAccess(string accessToken, UserListModel user,
            RepositoryListModel repository)
        {
            try
            {
                var permission = await GetClient(accessToken).Repository.Collaborator
                    .ReviewPermission(repository.GitHubId, user.Login);
                return permission.Permission.Value is PermissionLevel.Write or PermissionLevel.Admin;
            }
            catch (ApiException)
            {
                return false;
            }
        }

        public async Task<UserListModel> GetCurrentUser(string accessToken)
        {
            var gitHubUser = await GetClient(accessToken).User.Current();
            return Mapper.Map<UserListModel>(gitHubUser);
        }
    }
}