using CCMS.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.DAL
{
    public static class DalServiceConfigurator
    {
        public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<CCMSDBContext>(options =>
                    options.UseSqlServer(configuration["ConnectionString"])
                        .ConfigureWarnings(w => w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)),
                ServiceLifetime.Transient);

            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
            return services;
        }
    }
}