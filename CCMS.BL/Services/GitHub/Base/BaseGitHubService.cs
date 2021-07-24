using AutoMapper;
using CCMS.BL.Services.Base;
using CCMS.BL.Services.GitHub.Exceptions;
using Octokit;
using Octokit.Internal;

namespace CCMS.BL.Services.GitHub.Base
{
    public class BaseGitHubService : IService
    {
        protected readonly IMapper Mapper;

        public BaseGitHubService(IMapper mapper)
        {
            Mapper = mapper;
        }

        protected GitHubClient GetClient(string accessToken)
        {
            if (accessToken == null) throw new AccessTokenNullException();
            return new GitHubClient(new ProductHeaderValue("CCMS"),
                new InMemoryCredentialStore(new Credentials(accessToken)));
        }
    }
}