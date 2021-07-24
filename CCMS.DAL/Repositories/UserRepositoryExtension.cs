using System.Linq;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories.Base;

namespace CCMS.DAL.Repositories
{
    public static class UserRepositoryExtension
    {
        public static UserEntity GetByGitHubId(this IRepository<UserEntity> repository,
            int id)
        {
            return repository.Query.FirstOrDefault(u => u.GitHubId == id);
        }
    }
}