using System;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.List;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public class UserDataService : BaseDataService
    {
        public UserDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager) : base(mapper, unitOfWorkManager)
        {
        }

        public async Task<UserListModel> InsertOrUpdate(UserListModel user)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            var userEntity = Mapper.Map<UserEntity>(user);
            try
            {
                var entity = unitOfWork.Users.GetByGitHubId(userEntity.GitHubId);
                if (entity == null)
                {
                    unitOfWork.Users.Insert(userEntity);
                }
                else
                {
                    userEntity.Id = entity.Id;
                    unitOfWork.Users.Update(userEntity);
                }

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<UserListModel>(userEntity);
        }
    }
}