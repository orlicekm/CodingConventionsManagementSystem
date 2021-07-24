using System;
using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class UpdatedTextListModel : BaseModel, IListModel
    {
        public UserListModel UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Text { get; set; }
    }
}