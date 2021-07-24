using System;
using System.Threading.Tasks;
using AutoMapper;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.DAL.Entities;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public class CommentDataService : BaseDataService
    {
        public CommentDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager) : base(mapper,
            unitOfWorkManager)
        {
        }

        public async Task<CommentListModel> Insert(string text, UserListModel user,
            ConventionDetailModel convention)
        {
            using var unitOfWork = UnitOfWorkManager.Get();
            await unitOfWork.CreateTransaction();
            var comment = new CommentListModel
            {
                Owner = user,
                CreatedAt = DateTimeOffset.Now,
                Text = text,
                Convention = Mapper.Map<ConventionListModel>(convention)
            };
            var commentEntity = Mapper.Map<CommentEntity>(comment);
            try
            {
                commentEntity.Owner = unitOfWork.Users.GetById(comment.Owner.Id);
                commentEntity.Convention = unitOfWork.Conventions.GetById(comment.Convention.Id);
                unitOfWork.Comments.Insert(commentEntity);

                await unitOfWork.Complete();
                await unitOfWork.Commit();
            }
            catch (Exception)
            {
                await unitOfWork.Rollback();
                throw;
            }

            return Mapper.Map<CommentListModel>(commentEntity);
        }
    }
}