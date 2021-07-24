using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class BranchListModel : BaseModel, IListModel
    {
        public string Name { get; set; }
    }
}