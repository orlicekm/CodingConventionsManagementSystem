using CCMS.BL.Models.Base;

namespace CCMS.BL.Models.List
{
    public class UserListModel : BaseModel, IListModel
    {
        public int GitHubId { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string AvatarUrl { get; set; }
        public string HtmlUrl { get; set; }
    }
}