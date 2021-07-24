using CCMS.DAL.Entities.Base;
using CCMS.DAL.Entities.Enums;

namespace CCMS.DAL.Entities
{
    public class ResultCheckEntity : BaseEntity
    {
        public int LineId { get; set; }
        public string Line { get; set; }
        public ECheckStateEntity State { get; set; }
        public string Message { get; set; }
    }
}