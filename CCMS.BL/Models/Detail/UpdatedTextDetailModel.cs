using System;
using System.Collections.Generic;
using CCMS.BL.Models.Base;
using CCMS.BL.Models.List;

namespace CCMS.BL.Models.Detail
{
    public class UpdatedTextDetailModel : BaseModel, IDetailModel
    {
        public UserListModel UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Text { get; set; }
        public ICollection<PatchListModel> Patches { get; set; } = new List<PatchListModel>();
    }
}