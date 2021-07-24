using System;
using CCMS.BL.Models.Base;
using CCMS.DAL.Entities;

namespace CCMS.BL.Models.List
{
    public class PatchListModel : BaseModel, IListModel
    {
        public UserEntity CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Patch { get; set; }
    }
}