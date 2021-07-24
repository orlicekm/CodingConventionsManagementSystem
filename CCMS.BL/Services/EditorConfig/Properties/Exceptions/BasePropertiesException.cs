using System;
using System.Runtime.Serialization;

namespace CCMS.BL.Services.EditorConfig.Properties.Exceptions
{
    /// <summary>Base class for exceptions in properties.</summary>
    public class BasePropertiesException: Exception
    {
        public BasePropertiesException()
        {
        }

        public BasePropertiesException(string? message) : base(message)
        {
        }

        public BasePropertiesException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}