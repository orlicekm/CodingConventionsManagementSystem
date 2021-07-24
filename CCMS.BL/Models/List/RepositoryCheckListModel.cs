using System;
using System.Collections.Generic;
using CCMS.BL.Models.Base;
using CCMS.BL.Models.Detail;

namespace CCMS.BL.Models.List
{
    public class RepositoryCheckListModel : BaseModel, IListModel
    {
        public RepositoryDetailModel Repository { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public UserListModel CreatedBy { get; set; }
        public ICollection<ConventionCheckListModel> ConventionChecks { get; set; } = new List<ConventionCheckListModel>();
    }
}