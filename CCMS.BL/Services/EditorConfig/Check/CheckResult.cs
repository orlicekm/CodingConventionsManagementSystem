using CCMS.BL.Services.EditorConfig.Enums;

namespace CCMS.BL.Services.EditorConfig.Check
{
    public class CheckResult
    {
        public ECheckState State { get; set; }
        public string Message { get; set; }
    }
}