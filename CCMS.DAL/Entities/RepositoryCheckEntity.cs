using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class RepositoryCheckEntity : BaseEntity
    {
        [Required] public RepositoryEntity Repository { get; set; }
        [Required] public DateTimeOffset CreatedAt { get; set; }
        [Required] public UserEntity CreatedBy { get; set; }
        [Required] public ICollection<ConventionCheckEntity> ConventionChecks { get; set; } = new List<ConventionCheckEntity>();
    }
}