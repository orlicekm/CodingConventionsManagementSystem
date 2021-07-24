using System;
using System.Linq;
using CCMS.DAL.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace CCMS.DAL.Repositories.Base
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly CCMSDBContext Context;

        public Repository(CCMSDBContext context)
        {
            Context = context;
        }

        protected DbSet<TEntity> Set => Context.Set<TEntity>();

        public IQueryable<TEntity> Query => Set;

        public TEntity GetById(Guid id)
        {
            return Set.Find(id);
        }

        public void Insert(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Attach(entity);
            Context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                DetachIfExist(entity);
                Set.Attach(entity);
            }

            Context.Set<TEntity>().Update(entity);
        }

        private void DetachIfExist(TEntity entity)
        {
            var local = Context.Set<TEntity>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (local != null) Context.Entry(local).State = EntityState.Detached;
        }
    }
}