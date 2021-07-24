namespace CCMS.BL.Services.GitHub.Exceptions
{
    public class RepositoryNotFoundException : BaseGitHubServiceException
    {
        public RepositoryNotFoundException()
        {
        }

        public RepositoryNotFoundException(string message) : base(message)
        {
        }
    }
}