using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;

namespace CCMS.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<CommentEntity> Comments { get; }
        IRepository<ConventionEntity> Conventions { get; }
        IRepository<PatchEntity> Patches { get; }
        IRepository<RepositoryEntity> Repositories { get; }
        IRepository<UpdatedTextEntity> UpdatedTexts { get; }
        IRepository<UserEntity> Users { get; }
        IRepository<RepositoryCheckEntity> RepositoryChecks { get; }
        IRepository<ConventionCheckEntity> ConventionChecks { get; }
        IRepository<ResultCheckEntity> ResultChecks { get; }
        Task CreateTransaction();
        Task Commit();
        Task Rollback();
        Task<int> Complete();
        Task<int> Complete(CancellationToken cancellationToken);
    }
}