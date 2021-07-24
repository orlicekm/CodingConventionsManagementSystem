using System;
using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class RepositoryListModel : BaseModel, IListModel
    {
        public long GitHubId { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public UserListModel Owner { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string License { get; set; }
        public string HtmlUrl { get; set; }
        public bool Private { get; set; }
        public int Conventions { get; set; }
        public int Checks { get; set; }
        public int ForksCount { get; set; }
        public int StargazersCount { get; set; }
        public int OpenIssuesCount { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}