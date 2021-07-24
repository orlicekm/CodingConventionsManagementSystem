using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Entities
{
    public class ConventionCheckEntity : BaseEntity
    {
        [Required] public string Title { get; set; } 
        public List<ResultCheckEntity> Results { get; set; }
    }
}