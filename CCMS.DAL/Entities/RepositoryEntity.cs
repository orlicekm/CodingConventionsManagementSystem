using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class RepositoryEntity : BaseEntity
    {
        public long GitHubId { get; set; }
        public string FullName { get; set; }
        [Required] public ICollection<ConventionEntity> Conventions { get; set; } = new List<ConventionEntity>();
        [Required] public ICollection<RepositoryCheckEntity> Checks { get; set; } = new List<RepositoryCheckEntity>();
    }
}