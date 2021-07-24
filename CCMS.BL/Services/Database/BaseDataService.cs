using AutoMapper;
using CCMS.BL.Configurator;
using CCMS.BL.Services.Base;
using CCMS.DAL.UnitOfWork;

namespace CCMS.BL.Services.Database
{
    public abstract class BaseDataService : IService, ISingleton
    {
        protected readonly IMapper Mapper;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        protected BaseDataService(IMapper mapper, IUnitOfWorkManager unitOfWorkManager)
        {
            Mapper = mapper;
            UnitOfWorkManager = unitOfWorkManager;
        }
    }
}