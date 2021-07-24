using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CCMS.DAL.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CCMSDBContext context;
        private IDbContextTransaction transaction;

        public UnitOfWork(IDbContextFactory<CCMSDBContext> dbContextFactory)
        {
            context = dbContextFactory.CreateDbContext();
            Comments = new Repository<CommentEntity>(context);
            Conventions = new Repository<ConventionEntity>(context);
            Patches = new Repository<PatchEntity>(context);
            Repositories = new Repository<RepositoryEntity>(context);
            UpdatedTexts = new Repository<UpdatedTextEntity>(context);
            Users = new Repository<UserEntity>(context);
            RepositoryChecks = new Repository<RepositoryCheckEntity>(context);
            ConventionChecks = new Repository<ConventionCheckEntity>(context);
            ResultChecks = new Repository<ResultCheckEntity>(context);
        }


        public IRepository<CommentEntity> Comments { get; }
        public IRepository<ConventionEntity> Conventions { get; }
        public IRepository<PatchEntity> Patches { get; }
        public IRepository<RepositoryEntity> Repositories { get; }
        public IRepository<UpdatedTextEntity> UpdatedTexts { get; }
        public IRepository<UserEntity> Users { get; }
        public IRepository<RepositoryCheckEntity> RepositoryChecks { get; }
        public IRepository<ConventionCheckEntity> ConventionChecks { get; }
        public IRepository<ResultCheckEntity> ResultChecks { get; }

        public async Task CreateTransaction()
        {
            transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
        }

        public async Task<int> Complete()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<int> Complete(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) context.Dispose();
        }
    }
}