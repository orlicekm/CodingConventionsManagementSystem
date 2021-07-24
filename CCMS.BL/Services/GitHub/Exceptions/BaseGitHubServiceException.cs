using System;
using System.Runtime.Serialization;

namespace CCMS.BL.Services.GitHub.Exceptions
{
    public class BaseGitHubServiceException : Exception
    {
        public BaseGitHubServiceException()
        {
        }

        public BaseGitHubServiceException(string message) : base(message)
        {
        }

        public BaseGitHubServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}