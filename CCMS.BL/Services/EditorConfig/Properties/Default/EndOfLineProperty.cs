using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Default.Enums;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;

namespace CCMS.BL.Services.EditorConfig.Properties.Default
{
    /// <summary>Determines how line breaks are represented.</summary>
    public class EndOfLineProperty : IProperty, ICheckable, IImportable
    {
        private IEnumerable<string> LineEnds => new List<string>() { "\r\n", "\n", "\r" };

        public string Name => this.ToName();
        public string Description => "Determines how line breaks are represented.";
        public string AllowedValues => typeof(EEndOfLine).ToAllowedValue();
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(EEndOfLine).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            var lines = GetLineEnds(File.ReadAllText(file.FullName));
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
                if (line == StringToEndOfLine(value)) continue;
                failedLines.Add(i);
            }
            if (failedLines.Any())
            {
                result.State = ECheckState.Fail;
                if(failedLines.Count == 1)
                    result.Message += $"Detected different end of line on line {failedLines.First()}.";
                else
                    result.Message += $"Detected different end of line on lines {string.Join(", ", failedLines)}.";
            }
            return result;
        }

        public ImportResult Import(FileInfo file)
        {
            var lines = GetLineEnds(File.ReadAllText(file.FullName));
            var result = new ImportResult() { Result = new Dictionary<string, double>() };

            foreach (var line in lines)
            {
                foreach (var lineEnd in LineEnds)
                {
                    if (line != lineEnd) continue;
                    if (result.Result.ContainsKey(EndOfLineToString(lineEnd)))
                        result.Result[EndOfLineToString(lineEnd)] += 1;
                    else result.Result[EndOfLineToString(lineEnd)] = 1;
                    break;
                }
            }

            return result;
        }

        private List<string> GetLineEnds(string text)
        {
            return Regex.Matches(text, "\r\n|\r|\n")
                .Select(m => m.Value).ToList();
        }

        private string EndOfLineToString(string endOfLine)
        {
            return endOfLine switch
            {
                "\r\n" => "crlf",
                "\n" => "lf",
                "\r" => "cr",
                _ => throw new NotSupportedException()
            };
        }

        private string StringToEndOfLine(string value)
        {
            return value switch
            {
                "crlf" => "\r\n",
                "lf" => "\n",
                "cr" => "\r",
                _ => throw new NotSupportedException()
            };
        }
    }
}