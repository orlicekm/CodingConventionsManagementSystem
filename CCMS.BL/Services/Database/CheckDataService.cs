using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public class CheckDataService : BaseDataService
    {
        public CheckDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager) :
            base(mapper,
                unitOfWorkManager)
        {
        }

        public async Task<RepositoryCheckListModel> Insert(RepositoryCheckListModel check)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            var repositoryCheckEntity = Mapper.Map<RepositoryCheckEntity>(check);
            try
            {
                repositoryCheckEntity.Repository = unitOfWork.Repositories.GetByIdExtended(repositoryCheckEntity.Repository.Id);
                repositoryCheckEntity.CreatedBy = unitOfWork.Users.GetById(repositoryCheckEntity.CreatedBy.Id);
                unitOfWork.RepositoryChecks.Insert(repositoryCheckEntity);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<RepositoryCheckListModel>(repositoryCheckEntity);
        }

        public async Task<ICollection<RepositoryCheckListModel>> Get(RepositoryDetailModel repository)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            RepositoryEntity entity;
            try
            {
                entity = unitOfWork.Repositories.GetWithChecks(repository.Id);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<ICollection<RepositoryCheckListModel>>(entity.Checks);
        }

    }
}