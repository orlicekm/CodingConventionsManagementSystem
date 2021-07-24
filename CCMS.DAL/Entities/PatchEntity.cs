using System;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class PatchEntity : BaseEntity
    {
        [Required] public UpdatedTextEntity UpdatedTextEntity { get; set; }
        [Required] public UserEntity CreatedBy { get; set; }
        [Required] public DateTimeOffset CreatedAt { get; set; }
        [Required] public string Patch { get; set; }
    }
}