using System;

namespace CCMS.BL.Models.Base
{
    public abstract class BaseModel : IModel
    {
        public Guid Id { get; set; }
    }
}