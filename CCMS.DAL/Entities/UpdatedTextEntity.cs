using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class UpdatedTextEntity : BaseEntity
    {
        [Required] public UserEntity UpdatedBy { get; set; }
        [Required] public DateTimeOffset UpdatedAt { get; set; }
        [Required] public string Text { get; set; }
        public ICollection<PatchEntity> Patches { get; set; } = new List<PatchEntity>();
    }
}