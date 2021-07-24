using CCMS.BL.Models.Base;
using CCMS.BL.Services.EditorConfig.Enums;

namespace CCMS.BL.Models.List
{
    public class ResultCheckListModel : BaseModel, IListModel
    {
        public int LineId { get; set; }
        public string Line { get; set; }
        public ECheckState State { get; set; }
        public string Message { get; set; }
    }
}