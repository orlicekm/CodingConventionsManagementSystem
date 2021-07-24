using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Helpers;
using CCMS.DAL.Entities;
using CCMS.DAL.Repositories;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public class ConventionDataService : BaseDataService
    {
        private readonly PatchService patchService;

        public ConventionDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager, PatchService patchService) :
            base(mapper,
                unitOfWorkManager)
        {
            this.patchService = patchService;
        }

        public async Task<ConventionListModel> Insert(RepositoryDetailModel repository, UserListModel userListModel,
            string title)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ConventionEntity conventionEntity;
            try
            {
                conventionEntity = new ConventionEntity
                {
                    Title = title,
                    Repository = unitOfWork.Repositories.GetById(repository.Id),
                    UpdatedBy = unitOfWork.Users.GetById(userListModel.Id),
                    UpdatedAt = DateTimeOffset.Now
                };
                conventionEntity.FormattedText = CreateNewUpdatedText(conventionEntity.UpdatedBy,
                    conventionEntity.UpdatedAt, "Edit this for convention description.".ToPlainText());
                conventionEntity.FormalText = CreateNewUpdatedText(conventionEntity.UpdatedBy,
                    conventionEntity.UpdatedAt, "#Edit this for convention properties in EditorConfig format.");
                unitOfWork.Conventions.Insert(conventionEntity);

                conventionEntity = unitOfWork.Conventions.GetById(conventionEntity.Id);
                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<ConventionListModel>(conventionEntity);
        }

        private UpdatedTextEntity CreateNewUpdatedText(UserEntity user, DateTimeOffset time, string text)
        {
            return new()
            {
                Text = text,
                UpdatedBy = user,
                UpdatedAt = time,
                Patches = new List<PatchEntity>
                {
                    new()
                    {
                        CreatedBy = user,
                        CreatedAt = time,
                        Patch = patchService.GeneratePatch(string.Empty, text)
                    }
                }
            };
        }

        public async Task<ConventionDetailModel> Get(Guid id)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ConventionEntity convention;
            try
            {
                convention = unitOfWork.Conventions.GetByIdExtended(id);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<ConventionDetailModel>(convention);
        }

        public async Task<ConventionDetailModel> UpdateTitle(string title, ConventionDetailModel convention,
            UserListModel user)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ConventionEntity conventionEntity;
            try
            {
                conventionEntity = unitOfWork.Conventions.GetByIdExtended(convention.Id);
                conventionEntity.Title = title;
                conventionEntity.UpdatedBy = unitOfWork.Users.GetById(user.Id);
                conventionEntity.UpdatedAt = DateTimeOffset.Now;
                unitOfWork.Conventions.Update(conventionEntity);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<ConventionDetailModel>(conventionEntity);
        }

        public async Task Delete(ConventionDetailModel convention)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            try
            {
                var conventionEntity = unitOfWork.Conventions.GetByIdExtended(convention.Id);
                unitOfWork.UpdatedTexts.Delete(conventionEntity.FormalText);
                unitOfWork.UpdatedTexts.Delete(conventionEntity.FormattedText);
                unitOfWork.Conventions.Delete(conventionEntity);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }
        }
    }
}