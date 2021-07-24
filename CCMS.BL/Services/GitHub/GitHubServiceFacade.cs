using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Configurator;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Base;

namespace CCMS.BL.Services.GitHub
{
    public class GitHubServiceFacade : IService, ISingleton
    {
        private readonly GitHubContentService gitHubContentService;
        private readonly GitHubRepositoryService gitHubRepositoryService;
        private readonly GitHubUserService gitHubUserService;


        public GitHubServiceFacade(IMapper mapper)
        {
            gitHubUserService = new GitHubUserService(mapper);
            gitHubRepositoryService = new GitHubRepositoryService(mapper);
            gitHubContentService = new GitHubContentService(mapper);
        }

        public async Task<bool> HasUserWriteAccess(string accessToken, UserListModel user,
            RepositoryListModel repository)
        {
            return await gitHubUserService.HasUserWriteAccess(accessToken, user, repository);
        }

        public async Task<UserListModel> GetCurrentUser(string accessToken)
        {
            return await gitHubUserService.GetCurrentUser(accessToken);
        }

        public async Task<RepositoryListModel> GetRepository(string accessToken, string repositoryOwner,
            string repositoryName)
        {
            return await gitHubRepositoryService.GetRepository(accessToken, repositoryOwner, repositoryName);
        }

        public async Task<ICollection<RepositoryListModel>> GetAllRepositoriesForCurrentUser(string accessToken)
        {
            return await gitHubRepositoryService.GetAllRepositoriesForCurrentUser(accessToken);
        }

        public async Task<ICollection<BranchListModel>> GetAllBranches(string accessToken, long repositoryId)
        {
            return await gitHubRepositoryService.GetAllBranches(accessToken, repositoryId);
        }

        public async Task<GitHubContent> GetContent(string accessToken, long repositoryId, string branch)
        {
            return await gitHubContentService.GetContent(accessToken, repositoryId, branch);
        }
    }
}