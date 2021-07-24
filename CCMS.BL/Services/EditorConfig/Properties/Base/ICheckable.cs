using System.IO;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;

namespace CCMS.BL.Services.EditorConfig.Properties.Base
{
    /// <summary>Interface for property that can be checked.</summary>
    public interface ICheckable
    {
        /// <summary>User-friendly allowed values of property check that will be showed in properties page.</summary>
        public string AllowedValues { get; }

        /// <summary>User-friendly allowed file types of property check that will be showed in properties page.</summary>
        public string AllowedFileTypes { get; }

        /// <summary>Check method called for each file in repository that match section.</summary>
        /// <param name="file">Path to file, where convention is checked.</param>
        /// <param name="value">Value of checked property in convention.</param>
        /// <exception cref="UnsupportedPropertyValueException"></exception>
        /// <returns>Fail or success. If fails, message with reason is used.</returns>
        public CheckResult Check(string value, FileInfo file);
    }
}