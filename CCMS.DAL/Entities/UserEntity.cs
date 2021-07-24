using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class UserEntity : BaseEntity
    {
        public int GitHubId { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string AvatarUrl { get; set; }
        public string HtmlUrl { get; set; }
    }
}