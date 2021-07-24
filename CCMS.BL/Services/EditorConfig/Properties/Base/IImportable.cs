using System.IO;
using CCMS.BL.Services.EditorConfig.Import;

namespace CCMS.BL.Services.EditorConfig.Properties.Base
{
    /// <summary>Interface for property that can be imported.</summary>
    public interface IImportable
    {
        /// <summary>Section that will appear, when property will be selected for import.</summary>
        public string DefaultSection { get; }

        /// <summary>Import method called for each file in repository that match section.</summary>
        /// <param name="file">Path to file, from which is convention actually imported.</param>
        /// <returns>List of string with weights. In the result of import all weights for each string from each import method will be summed and ratio of each one will be calculated.</returns>
        public ImportResult Import(FileInfo file);
    }
}