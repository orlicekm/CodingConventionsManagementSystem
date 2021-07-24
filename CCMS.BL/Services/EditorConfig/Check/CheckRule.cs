using System.Collections.Generic;
using CCMS.BL.Models.List;
using CCMS.BL.Services.EditorConfig.Properties.Base;

namespace CCMS.BL.Services.EditorConfig.Check
{
    internal class CheckRule
    {
        public string Section { get; set; }
        public string Value { get; set; }
        public IProperty Property { get; set; }
        public ConventionListModel Convention { get; set; }
        public int Line { get; set; }

        public ICollection<(CheckResult result, string file)> Results { get; set; } =
            new List<(CheckResult result, string file)>();
    }
}