using System;
using System.Linq;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CCMS.DAL.Repositories
{
    public static class ConventionRepositoryExtension
    {
        public static ConventionEntity GetByIdExtended(this IRepository<ConventionEntity> repository,
            Guid id)
        {
            return repository.Query
                .Include(c => c.UpdatedBy)
                .Include(c => c.FormattedText.UpdatedBy)
                .Include(c => c.FormalText.UpdatedBy)
                .Include(c => c.Comments)
                .ThenInclude(t => t.Owner)
                .Include(c => c.Repository)
                .FirstOrDefault(u => u.Id == id);
        }
    }
}