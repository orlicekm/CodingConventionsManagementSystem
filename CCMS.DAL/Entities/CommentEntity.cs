using System;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class CommentEntity : BaseEntity
    {
        [Required] public ConventionEntity Convention { get; set; }
        [Required] public UserEntity Owner { get; set; }
        [Required] public DateTimeOffset CreatedAt { get; set; }
        [Required] public string Text { get; set; }
    }
}