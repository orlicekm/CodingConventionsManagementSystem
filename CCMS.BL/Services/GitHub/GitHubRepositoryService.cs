using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.List;
using CCMS.BL.Services.GitHub.Base;
using CCMS.BL.Services.GitHub.Exceptions;
using Octokit;

namespace CCMS.BL.Services.GitHub
{
    internal class GitHubRepositoryService : BaseGitHubService
    {
        public GitHubRepositoryService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<RepositoryListModel> GetRepository(string accessToken, string repositoryOwner,
            string repositoryName)
        {
            try
            {
                var repository = await GetClient(accessToken).Repository.Get(repositoryOwner, repositoryName);
                return Mapper.Map<RepositoryListModel>(repository);
            }
            catch (ApiException)
            {
                throw new RepositoryNotFoundException($"{repositoryOwner}/{repositoryName}");
            }
        }

        public async Task<ICollection<RepositoryListModel>> GetAllRepositoriesForCurrentUser(string accessToken)
        {
            var repositories = await GetClient(accessToken).Repository.GetAllForCurrent();
            return Mapper.Map<ICollection<RepositoryListModel>>(repositories);
        }

        public async Task<ICollection<BranchListModel>> GetAllBranches(string accessToken, long repositoryId)
        {
            var branches = await GetClient(accessToken).Repository.Branch.GetAll(repositoryId);
            return Mapper.Map<ICollection<BranchListModel>>(branches);
        }
    }
}