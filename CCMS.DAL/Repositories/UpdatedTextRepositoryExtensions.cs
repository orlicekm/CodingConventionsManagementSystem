using System;
using System.Linq;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CCMS.DAL.Repositories
{
    public static class UpdatedTextRepositoryExtensions
    {
        public static UpdatedTextEntity GetByIdExtended(this IRepository<UpdatedTextEntity> repository,
            Guid id)
        {
            return repository.Query
                .Include(c => c.UpdatedBy)
                .Include(c => c.Patches)
                .ThenInclude(t => t.CreatedBy)
                .FirstOrDefault(u => u.Id == id);
        }
    }
}