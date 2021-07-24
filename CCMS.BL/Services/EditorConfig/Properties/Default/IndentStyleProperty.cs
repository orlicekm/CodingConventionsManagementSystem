using System;
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
    /// <summary>Determines use of hard or soft tabs.</summary>
    public class IndentStyleProperty : IProperty, ICheckable, IImportable
    {
        public string Name => this.ToName();
        public string Description => "Determines use of hard or soft tabs.";
        public string AllowedValues => typeof(EIndentStyle).ToAllowedValue();
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(EIndentStyle).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            var lines = File.ReadAllLines(file.FullName);
            var i = 0;
            var failedLines = new List<int>();
            var result = new CheckResult
            {
                State = ECheckState.Success,
                Message = string.Empty
            };
            foreach (var line in lines)
            {
                i++;
                if (line.StartsWith(" ") && value == "tab" ||
                    line.StartsWith("\t") && value == "space")
                {
                    failedLines.Add(i);
                }
            }
            if (failedLines.Any())
            {
                result.State = ECheckState.Fail;
                if (value == "tab") result.Message += $"Line(s) {string.Join(", ", failedLines)} start(s) with space.";
                if (value == "space") result.Message += $"Line(s) {string.Join(", ", failedLines)} start(s) with tab.";
            }
            return result;
        }

        public ImportResult Import(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var result = new ImportResult() { Result = new Dictionary<string, double>() };

            foreach (var line in lines)
            {
                if (line.StartsWith(" "))
                {
                    if (result.Result.ContainsKey("space"))
                        result.Result["space"] += 1;
                    else result.Result["space"] = 1;
                }
                if (line.StartsWith("\t"))
                {
                    if (result.Result.ContainsKey("tab"))
                        result.Result["tab"] += 1;
                    else result.Result["tab"] = 1;
                }
            }
            return result;
        }
    }
}