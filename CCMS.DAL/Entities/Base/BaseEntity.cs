using System;

namespace CCMS.DAL.Entities.Base
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }
    }
}