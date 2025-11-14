using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Reflection;

public static class AutoMapperServiceCollectionExtensions
{
    public static IServiceCollection AddAutoMapperDependency(this IServiceCollection services)
    {
        // Automatically load all profiles in the current assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Or register from specific assemblies if needed:
        // services.AddAutoMapper(typeof(MyMappingProfile).Assembly);

        return services;
    }
}

