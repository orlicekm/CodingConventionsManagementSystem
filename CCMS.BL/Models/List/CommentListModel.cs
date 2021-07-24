using System;
using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class CommentListModel : BaseModel, IListModel
    {
        public UserListModel Owner { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Text { get; set; }
        public ConventionListModel Convention { get; set; }
    }
}