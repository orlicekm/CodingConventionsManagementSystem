using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Default.Enums;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;

namespace CCMS.BL.Services.EditorConfig.Properties.Default
{
    /// <summary>Determines file character set. There are known issues with distinguishing latin1 and utf-8 from utf-8bom.</summary>
    public class CharsetProperty : IProperty, ICheckable, IImportable
    {

        private Encoding Latin1 => Encoding.GetEncoding("iso-8859-1");
        private Encoding Utf8 => new UTF8Encoding(false);

        public string Name => this.ToName();
        public string Description => "Determines file character set.\nThere are known issues with distinguishing latin1 and utf-8 from utf-8bom.";
        public string AllowedValues => typeof(ECharset).ToAllowedValue();
        public string AllowedFileTypes => "All (*)";
        public string DefaultSection => "*";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(ECharset).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            using var reader = new StreamReader(file.FullName, Encoding.UTF8, true);
            reader.Peek();
            if (value == EncodingToString(reader.CurrentEncoding))
                return new CheckResult { State = ECheckState.Success };
            return new CheckResult
            {
                State = ECheckState.Fail,
                Message = $"Detected {EncodingToString(reader.CurrentEncoding)} encoding."
            };
        }

        public ImportResult Import(FileInfo file)
        {
            using var reader = new StreamReader(file.FullName, Encoding.UTF8, true);
            reader.Peek();
            var result = new ImportResult() {Result = new Dictionary<string, double>()};
            result.Result[EncodingToString(reader.CurrentEncoding)] = 1;
            return result;
        }

        private string EncodingToString(Encoding encoding)
        {
            if (Equals(encoding, Latin1)) return "latin1";
            if (Equals(encoding, Utf8)) return "utf-8";
            if (Equals(encoding, Encoding.UTF8)) return "utf-8-bom";
            if (Equals(encoding, Encoding.BigEndianUnicode)) return "utf-16be";
            if (Equals(encoding, Encoding.Unicode)) return "utf-16le";
            throw new NotSupportedException();
        }
    }
}