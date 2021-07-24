using System;
using System.Linq;
using CCMS.DAL.Entities.Base;

namespace CCMS.DAL.Repositories.Base
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        IQueryable<TEntity> Query { get; }
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        TEntity GetById(Guid id);
    }
}