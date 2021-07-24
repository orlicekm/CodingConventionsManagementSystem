using System.Collections.Generic;
using CCMS.BL.Models.Base;
using CCMS.BL.Services.EditorConfig.Enums;

namespace CCMS.BL.Models.List
{
    public class ConventionCheckListModel : BaseModel, IListModel
    {
        public string Title { get; set; }
        public List<ResultCheckListModel> Results { get; set; } = new();
    }
}