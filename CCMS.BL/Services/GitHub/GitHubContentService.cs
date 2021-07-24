using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Services.GitHub.Base;
using Octokit;

namespace CCMS.BL.Services.GitHub
{
    internal class GitHubContentService : BaseGitHubService
    {
        public GitHubContentService(IMapper mapper) : base(mapper)
        {
        }


        public async Task<GitHubContent> GetContent(string accessToken, long repositoryId, string branch)
        {
            var content = await GetClient(accessToken).Repository.Content
                .GetArchive(repositoryId, ArchiveFormat.Zipball, branch);
            return await GitHubContent.Create(content);
        }
    }
}