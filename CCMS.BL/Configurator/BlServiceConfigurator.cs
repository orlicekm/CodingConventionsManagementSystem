using System;
using System.Linq;
using System.Reflection;
using CCMS.BL.Services.Mapper;
using CCMS.DAL;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.BL.Configurator
{
    public static class BlServiceConfigurator
    {
        public static IServiceCollection AddBl(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDal(configuration);

            services.AddSingleton(_ => new MapperConfigurator().CreateMapper());
            services.AddScoped<IMessenger, Messenger>();

            ConfigureByType(services);

            return services;
        }

        private static void ConfigureByType(IServiceCollection services)
        {
            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace.StartsWith("CCMS.BL") && !t.Namespace.Contains("Base") && !t.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                if (typeof(IScoped).IsAssignableFrom(type))
                    services.AddScoped(type);
                if (typeof(IScoped<>).IsAssignableFromGeneric(type))
                    services.AddSingleton(
                        typeof(IScoped<>).GetGenericInterfaceType(type), type);

                if (typeof(ISingleton).IsAssignableFrom(type))
                    services.AddSingleton(type);
                if (typeof(ISingleton<>).IsAssignableFromGeneric(type))
                    services.AddSingleton(
                        typeof(ISingleton<>).GetGenericInterfaceType(type), type);
            }
        }

        private static bool IsAssignableFromGeneric(this Type genericType, Type type)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
        }

        private static Type GetGenericInterfaceType(this Type genericType, Type type)
        {
            return type.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType).GetGenericArguments()
                .First();
        }
    }
}