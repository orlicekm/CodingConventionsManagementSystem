using System;
using System.Collections.Generic;
using CCMS.BL.Models.Base;
using CCMS.BL.Models.List;

namespace CCMS.BL.Models.Detail
{
    public class ConventionDetailModel : BaseModel, IDetailModel
    {
        public string Title { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public UserListModel UpdatedBy { get; set; }
        public UpdatedTextListModel FormattedText { get; set; }
        public UpdatedTextListModel FormalText { get; set; }
        public ICollection<CommentListModel> Comments { get; set; } = new List<CommentListModel>();
        public RepositoryListModel Repository { get; set; }
    }
}