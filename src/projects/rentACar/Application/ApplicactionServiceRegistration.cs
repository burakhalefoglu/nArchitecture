using Application.Features.Brands.Profiles;
using Application.Features.Brands.Rules;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ApplicactionServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<BrandBusinessRules>();
        //services.AddScoped<IBrandRepository, BrandRepository>();

        return services;
    }
}