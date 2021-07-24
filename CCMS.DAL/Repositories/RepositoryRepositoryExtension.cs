using System;
using System.Linq;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CCMS.DAL.Repositories
{
    public static class RepositoryRepositoryExtension
    {
        public static RepositoryEntity GetByGitHubForListId(this IRepository<RepositoryEntity> repository,
            long id)
        {
            return repository.Query.Include(r => r.Conventions)
                .ThenInclude(c => c.UpdatedBy)
                .Include(r => r.Checks)
                .FirstOrDefault(u => u.GitHubId == id);
        }

        public static RepositoryEntity GetByGitHubForDetailId(this IRepository<RepositoryEntity> repository,
            long id)
        {
            return repository.Query
                .Include(r => r.Conventions).ThenInclude(c => c.UpdatedBy)
                .Include(r => r.Conventions).ThenInclude(c => c.FormalText)
                .Include(r => r.Conventions).ThenInclude(c => c.FormattedText)
                .Include(r => r.Conventions).ThenInclude(c => c.Comments)
                .FirstOrDefault(u => u.GitHubId == id);
        }

        public static RepositoryEntity GetByIdExtended(this IRepository<RepositoryEntity> repository,
            Guid id)
        {
            return repository.Query
                .Include(r => r.Conventions).ThenInclude(c => c.UpdatedBy)
                .Include(r => r.Conventions).ThenInclude(c => c.FormalText)
                .Include(r => r.Conventions).ThenInclude(c => c.FormattedText)
                .Include(r => r.Conventions).ThenInclude(c => c.Comments)
                .FirstOrDefault(u => u.Id == id);
        }

        public static RepositoryEntity GetWithChecks(this IRepository<RepositoryEntity> repository,
            Guid id)
        {
            return repository.Query
                .Include(r => r.Checks).ThenInclude(c => c.ConventionChecks).ThenInclude(r => r.Results)
                .Include(r => r.Checks).ThenInclude(c => c.CreatedBy)
                .FirstOrDefault(u => u.Id == id);
        }
    }
}