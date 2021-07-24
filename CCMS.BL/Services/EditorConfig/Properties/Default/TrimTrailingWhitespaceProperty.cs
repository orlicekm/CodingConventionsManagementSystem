using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Default.Enums;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;

namespace CCMS.BL.Services.EditorConfig.Properties.Default
{
    /// <summary>Ensures whether all whitespace characters preceding newline are removed or not.</summary>
    public class TrimTrailingWhitespaceProperty : IProperty, ICheckable, IImportable
    {
        public string Name => this.ToName();

        public string Description =>
            "Ensures whether all whitespace characters preceding newline are removed or not.";

        public string AllowedValues => typeof(EBool).ToAllowedValue();
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(EBool).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            var lines = File.ReadAllLines(file.FullName);
            var i = 0;
            var result = new CheckResult
            {
                State = ECheckState.Success,
                Message = string.Empty
            };
            var failedLines = new List<int>();
            foreach (var line in lines)
            {
                i++;
                if (line.Length == line.TrimEnd().Length || value != "true") continue;
                failedLines.Add(i);
            }
            if (failedLines.Any())
            {
                result.State = ECheckState.Fail;
                if (failedLines.Count == 1)
                    result.Message += $"Detected whitespaces at the end of the line {failedLines.First()}.";
                else
                    result.Message += $"Detected whitespaces at the end of the lines {string.Join(", ", failedLines)}.";
            }
            return result;
        }

        public ImportResult Import(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var result = new ImportResult() { Result = new Dictionary<string, double>() };

            foreach (var line in lines)
            {
                if (line.Length == line.TrimEnd().Length)
                {
                    if (result.Result.ContainsKey("true"))
                        result.Result["true"] += 1;
                    else result.Result["true"] = 1;
                }
                else
                {
                    if (result.Result.ContainsKey("false"))
                        result.Result["false"] += 1;
                    else result.Result["false"] = 1;
                }
            }
            return result;
        }
    }
}