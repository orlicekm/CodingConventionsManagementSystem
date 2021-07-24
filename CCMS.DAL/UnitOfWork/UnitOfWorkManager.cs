using System;

namespace CCMS.DAL.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IServiceProvider serviceProvider;

        public UnitOfWorkManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IUnitOfWork Get()
        {
            return (IUnitOfWork) serviceProvider.GetService(typeof(IUnitOfWork));
        }
    }
}