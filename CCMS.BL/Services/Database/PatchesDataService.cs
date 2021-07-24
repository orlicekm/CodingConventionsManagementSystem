using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.List;
using CCMS.DAL.Entities;
using CCMS.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace CCMS.BL.Services.Database
{
    public class PatchesDataService : BaseDataService
    {
        public PatchesDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager) : base(mapper,
            unitOfWorkManager)
        {
        }


        public async Task<ICollection<PatchListModel>> Get(UpdatedTextListModel updatedText)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            ICollection<PatchEntity> patches;
            try
            {
                patches = unitOfWork.Patches.Query
                    .Include(c => c.CreatedBy).Where(p => p.UpdatedTextEntity.Id == updatedText.Id).ToList();

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<ICollection<PatchListModel>>(patches);
        }
    }
}