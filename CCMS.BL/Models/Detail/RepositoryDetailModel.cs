using System.Collections.Generic;
using CCMS.BL.Models.Base;
using CCMS.BL.Models.List;

namespace CCMS.BL.Models.Detail
{
    public class RepositoryDetailModel : BaseModel, IDetailModel
    {
        public long GitHubId { get; set; }
        public string FullName { get; set; }
        public ICollection<ConventionListModel> Conventions { get; set; } = new List<ConventionListModel>();
        public ICollection<ConventionCheckListModel> ConventionChecks { get; set; } = new List<ConventionCheckListModel>();
    }
}