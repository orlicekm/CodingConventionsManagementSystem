using System;
using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class ConventionListModel : BaseModel, IListModel
    {
        public string Title { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public UserListModel UpdatedBy { get; set; }
        public string FormattedText { get; set; }
        public string FormalText { get; set; }
        public int Properties { get; set; }
        public int Sections { get; set; }
        public int Comments { get; set; }
    }
}