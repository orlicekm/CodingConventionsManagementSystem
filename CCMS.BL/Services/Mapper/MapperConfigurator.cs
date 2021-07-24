using System;
using System.Linq;
using AutoMapper;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.Helpers;
using CCMS.BL.ViewModels.Observables;
using CCMS.DAL.Entities;
using CCMS.DAL.Entities.Enums;
using Octokit;

namespace CCMS.BL.Services.Mapper
{
    public class MapperConfigurator
    {
        internal IMapper CreateMapper()
        {
            return CreateConfiguration().CreateMapper();
        }

        private MapperConfiguration CreateConfiguration()
        {
            return new(config =>
            {
                ConfigureGitHubToModels(config);
                ConfigureDetailModelsToListModels(config);
                ConfigureListModelsToEntitiesAndBack(config);
                ConfigureDetailModelsToEntitiesAndBack(config);
            });
        }

        private void ConfigureDetailModelsToListModels(IMapperConfigurationExpression config)
        {
            config.CreateMap<ConventionDetailModel, ConventionListModel>()
                .ForMember(d => d.FormattedText, opt => opt.MapFrom(s => s.FormattedText.Text.Shrink()))
                .ForMember(d => d.FormalText, opt => opt.MapFrom(s => s.FormalText.Text))
                .ForMember(d => d.Properties, opt => opt.MapFrom(s => s.FormalText.Text.CountProperties()))
                .ForMember(d => d.Sections, opt => opt.MapFrom(s => s.FormalText.Text.CountSections()))
                .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments.Count));

            config.CreateMap<RepositoryDetailModel, RepositoryListModel>()
                .ForMember(d => d.Conventions, opt => opt.MapFrom(s => s.Conventions.Count))
                .ForMember(d => d.Checks, opt => opt.MapFrom(s => s.ConventionChecks.Count));

            config.CreateMap<UpdatedTextDetailModel, UpdatedTextListModel>();
        }

        private void ConfigureDetailModelsToEntitiesAndBack(IMapperConfigurationExpression config)
        {
            config.CreateMap<ConventionEntity, ConventionDetailModel>();
            config.CreateMap<ConventionDetailModel, ConventionEntity>();

            config.CreateMap<RepositoryEntity, RepositoryDetailModel>();
            config.CreateMap<RepositoryDetailModel, RepositoryEntity>();

            config.CreateMap<UpdatedTextEntity, UpdatedTextDetailModel>();
            config.CreateMap<UpdatedTextDetailModel, UpdatedTextEntity>();
        }

        private void ConfigureListModelsToEntitiesAndBack(IMapperConfigurationExpression config)
        {
            config.CreateMap<CommentEntity, CommentListModel>();
            config.CreateMap<CommentListModel, CommentEntity>();

            config.CreateMap<ConventionEntity, ConventionListModel>()
                .ForMember(d => d.FormattedText, opt => opt.MapFrom(s => s.FormattedText.Text.Shrink()))
                .ForMember(d => d.FormalText, opt => opt.MapFrom(s => s.FormalText.Text))
                .ForMember(d => d.Properties, opt => opt.MapFrom(s => s.FormalText.Text.CountProperties()))
                .ForMember(d => d.Sections, opt => opt.MapFrom(s => s.FormalText.Text.CountSections()))
                .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments.Count));
            config.CreateMap<ConventionListModel, ConventionEntity>()
                .ForMember(d => d.FormattedText, opt => opt.Ignore())
                .ForMember(d => d.FormalText, opt => opt.Ignore())
                .ForMember(d => d.FormalText, opt => opt.Ignore())
                .ForMember(d => d.Comments, opt => opt.Ignore());


            config.CreateMap<PatchEntity, PatchListModel>();
            config.CreateMap<PatchListModel, PatchEntity>();

            config.CreateMap<RepositoryEntity, RepositoryListModel>()
                .ForMember(d => d.Conventions, opt => opt.MapFrom(s => s.Conventions.Count))
                .ForMember(d => d.Checks, opt => opt.MapFrom(s => s.Checks.Count));
            config.CreateMap<RepositoryListModel, RepositoryEntity>()
                .ForMember(d => d.Conventions, opt => opt.Ignore())
                .ForMember(d => d.Checks, opt => opt.Ignore());

            config.CreateMap<UpdatedTextEntity, UpdatedTextListModel>();
            config.CreateMap<UpdatedTextListModel, UpdatedTextEntity>();

            config.CreateMap<UserListModel, UserEntity>();
            config.CreateMap<UserEntity, UserListModel>();

            config.CreateMap<RepositoryCheckEntity, RepositoryCheckListModel>();
            config.CreateMap<RepositoryCheckListModel, RepositoryCheckEntity>();

            config.CreateMap<ConventionCheckEntity, ConventionCheckListModel>();
            config.CreateMap<ConventionCheckListModel, ConventionCheckEntity>();

            config.CreateMap<ResultCheckEntity, ResultCheckListModel>();
            config.CreateMap<ResultCheckListModel, ResultCheckEntity>();
        }

        private void ConfigureGitHubToModels(IMapperConfigurationExpression config)
        {
            config.CreateMap<User, UserListModel>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.GitHubId, opt => opt.MapFrom(s => s.Id));

            config.CreateMap<Repository, RepositoryListModel>()
                .ForMember(d => d.License, opt => opt.MapFrom(s => s.License.Name))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.GitHubId, opt => opt.MapFrom(s => s.Id));

            config.CreateMap<Branch, BranchListModel>();
        }
    }
}