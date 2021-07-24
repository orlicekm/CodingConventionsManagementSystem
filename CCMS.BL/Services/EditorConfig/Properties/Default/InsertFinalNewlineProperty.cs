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
    /// <summary>Ensures whether file ends with new line or not.</summary>
    public class InsertFinalNewlineProperty : IProperty, ICheckable, IImportable
    {
        private IEnumerable<string> LineEnds => new List<string>() { "\r\n", "\n", "\r" };

        public string Name => this.ToName();
        public string Description => "Ensures whether file ends with new line or not.";
        public string AllowedValues => typeof(EBool).ToAllowedValue();
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(EBool).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            var text = File.ReadAllText(file.FullName);
            if (LineEnds.Any(e => text.EndsWith(e)) && value == "true" ||
                LineEnds.All(e => !text.EndsWith(e)) && value == "false")
                return new CheckResult { State = ECheckState.Success };

            if (LineEnds.All(e => !text.EndsWith(e)) && value == "true")
                return new CheckResult
                {
                    State = ECheckState.Fail,
                    Message = "New line missing."
                };

            if (LineEnds.Any(e => text.EndsWith(e)) && value == "false")
                return new CheckResult
                {
                    State = ECheckState.Fail,
                    Message = "New line detected."
                };

            throw new NotSupportedException();
        }

        public ImportResult Import(FileInfo file)
        {
            var text = File.ReadAllText(file.FullName);
            var result = new ImportResult() { Result = new Dictionary<string, double>() };
            if (LineEnds.Any(e => text.EndsWith(e)))
                result.Result["true"] = 1;
            else
                result.Result["false"] = 1;
            return result;
        }
    }
}