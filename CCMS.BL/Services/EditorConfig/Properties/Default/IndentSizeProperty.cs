using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;

namespace CCMS.BL.Services.EditorConfig.Properties.Default
{
    /// <summary>Determines number of columns used for each indentation level and the width of soft tabs (when supported). Width of soft tabs and tab value cannot be supported outside editor.</summary>
    public class IndentSizeProperty : IProperty, ICheckable, IImportable
    {
        public string Name => this.ToName();

        public string Description =>
            "Determines number of columns used for each indentation level and the width of soft tabs (when supported).\nWidth of soft tabs and tab value cannot be supported outside editor.";

        public string AllowedValues => "Number";
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            int intValue;
            try
            {
                intValue = int.Parse(value);
            }
            catch (Exception)
            {
                throw new UnsupportedPropertyValueException();
            }

            if (intValue < 0) throw new UnsupportedPropertyValueException();

            var lines = File.ReadAllLines(file.FullName);
            var i = 0;
            var result = new CheckResult
            {
                State = ECheckState.Success,
                Message = string.Empty
            };
            var failedLines = new List<(int spaces, int line)>();
            foreach (var line in lines)
            {
                i++;
                var spaces = line.Length - line.TrimStart(' ').Length;
                if (intValue == 0) continue;
                if (spaces == 0) continue;
                if (spaces % intValue == 0) continue;
                failedLines.Add((spaces, i));
            }
            if (failedLines.Any())
            {
                result.State = ECheckState.Fail;
                foreach (var fail in failedLines.GroupBy(l => l.spaces))
                {
                    result.Message += $"{fail.Key} spaces detected on line(s) {string.Join(", ", fail.Select(f => f.line))}. ";
                }
            }
            return result;
        }

        public ImportResult Import(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var result = new ImportResult() { Result = new Dictionary<string, double>() };

            foreach (var line in lines)
            {
                if (!line.StartsWith(' ')) continue;
                var spaceCount = line.Length - line.TrimStart(' ').Length;
                if (result.Result.ContainsKey(spaceCount.ToString()))
                    result.Result[spaceCount.ToString()] += 1;
                else result.Result[spaceCount.ToString()] = 1;
            }
            return result;
        }
    }
}