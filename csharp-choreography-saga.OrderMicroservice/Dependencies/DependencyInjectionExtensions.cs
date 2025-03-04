using csharp_choreography_saga.OrderMicroservice.Configurations;
using csharp_choreography_saga.OrderMicroservice.Entities;
using csharp_choreography_saga.OrderMicroservice.Persistence.Base;
using csharp_choreography_saga.OrderMicroservice.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_choreography_saga.OrderMicroservice.Dependencies
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.Configure<AppSetting>(builder.Configuration);

            builder
                .Services.AddControllers()
                .ConfigureApiBehaviorOptions(opt =>
                {
                    opt.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context
                            .ModelState.Where(ms => ms.Value!.Errors.Any())
                            .SelectMany(ms => ms.Value!.Errors.Select(e => e.ErrorMessage))
                            .ToList();

                        var errorMessage = string.Join("; ", errors);
                        var result = Result<object>.Fail(
                            errorMessage
                        );

                        return new OkObjectResult(result);
                    };
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            builder.Services.AddDbContext<AppDbContext>((serviceProvider, opt) =>
            {
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
            });

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
            });

            builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

            return services;
        }
    }
}
