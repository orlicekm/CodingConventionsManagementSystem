using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public class RepositoryDataService : BaseDataService
    {
        public RepositoryDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager) : base(mapper,
            unitOfWorkManager)
        {
        }

        public async Task<RepositoryDetailModel> InsertOrGet(RepositoryListModel repository)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            var repositoryEntity = Mapper.Map<RepositoryEntity>(repository);
            try
            {
                var entity = unitOfWork.Repositories.GetByGitHubForDetailId(repository.GitHubId);
                if (entity == null)
                    unitOfWork.Repositories.Insert(repositoryEntity);
                else
                    repositoryEntity = entity;

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<RepositoryDetailModel>(repositoryEntity);
        }

        public async Task<ICollection<RepositoryListModel>> Bind(
            ICollection<RepositoryListModel> repositories)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            try
            {
                foreach (var repository in repositories)
                {
                    var entity = unitOfWork.Repositories.GetByGitHubForListId(repository.GitHubId);
                    repository.Conventions = entity?.Conventions?.Count ?? 0;
                    repository.Checks = entity?.Checks?.Count ?? 0;
                }

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return repositories;
        }

        public async Task<RepositoryDetailModel> Get(Guid id)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            RepositoryEntity repository;
            try
            {
                repository = unitOfWork.Repositories.GetByIdExtended(id);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<RepositoryDetailModel>(repository);
        }
    }
}