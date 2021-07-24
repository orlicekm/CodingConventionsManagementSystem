using System;

namespace CCMS.DAL.Entities.Base
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}