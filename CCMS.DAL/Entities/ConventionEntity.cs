using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class ConventionEntity : BaseEntity
    {
        [Required] public string Title { get; set; }
        [Required] public DateTimeOffset UpdatedAt { get; set; }
        [Required] public UserEntity UpdatedBy { get; set; }
        [Required] public UpdatedTextEntity FormattedText { get; set; }
        [Required] public UpdatedTextEntity FormalText { get; set; }
        public ICollection<CommentEntity> Comments { get; set; }
        [Required] public RepositoryEntity Repository { get; set; }
    }
}