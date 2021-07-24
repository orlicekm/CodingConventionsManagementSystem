using System;
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
    public class UpdatedTextDataService : BaseDataService
    {
        private readonly PatchService patchService;

        public UpdatedTextDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager, PatchService patchService) :
            base(mapper,
                unitOfWorkManager)
        {
            this.patchService = patchService;
        }

        public async Task<ConventionDetailModel> UpdateFormatted(string text,
            ConventionDetailModel convention, UserListModel user)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ConventionEntity conventionEntity;
            try
            {
                var updatedTextEntity = unitOfWork.UpdatedTexts.GetByIdExtended(convention.FormattedText.Id);
                updatedTextEntity.UpdatedAt = DateTimeOffset.Now;
                updatedTextEntity.UpdatedBy = unitOfWork.Users.GetById(user.Id);
                var patchEntity = new PatchEntity
                {
                    CreatedAt = updatedTextEntity.UpdatedAt,
                    CreatedBy = updatedTextEntity.UpdatedBy,
                    Patch = patchService.GeneratePatch(updatedTextEntity.Text, text, true),
                    UpdatedTextEntity = updatedTextEntity
                };
                updatedTextEntity.Patches.Add(patchEntity);
                updatedTextEntity.Text = text.ToUnscriptedHtml().Replace("\n", "<br />");

                unitOfWork.UpdatedTexts.Update(updatedTextEntity);

                conventionEntity = unitOfWork.Conventions.GetByIdExtended(convention.Id);
                conventionEntity.UpdatedBy = updatedTextEntity.UpdatedBy;
                conventionEntity.UpdatedAt = updatedTextEntity.UpdatedAt;
                conventionEntity.FormattedText = updatedTextEntity;
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

        public async Task<ConventionDetailModel> UpdateFormal(string text,
            ConventionDetailModel convention, UserListModel user)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ConventionEntity conventionEntity;
            try
            {
                var updatedTextEntity = unitOfWork.UpdatedTexts.GetByIdExtended(convention.FormalText.Id);
                updatedTextEntity.UpdatedAt = DateTimeOffset.Now;
                updatedTextEntity.UpdatedBy = unitOfWork.Users.GetById(user.Id);
                var patchEntity = new PatchEntity
                {
                    CreatedAt = updatedTextEntity.UpdatedAt,
                    CreatedBy = updatedTextEntity.UpdatedBy,
                    Patch = patchService.GeneratePatch(updatedTextEntity.Text, text),
                    UpdatedTextEntity = updatedTextEntity
                };
                updatedTextEntity.Patches.Add(patchEntity);
                updatedTextEntity.Text = text.ToUnscriptedHtml();

                unitOfWork.UpdatedTexts.Update(updatedTextEntity);

                conventionEntity = unitOfWork.Conventions.GetByIdExtended(convention.Id);
                conventionEntity.UpdatedBy = unitOfWork.Users.GetById(user.Id);
                conventionEntity.UpdatedAt = DateTimeOffset.Now;
                conventionEntity.FormalText = updatedTextEntity;
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
    }
}