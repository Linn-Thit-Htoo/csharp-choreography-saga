using csharp_choreography_saga.StockMicroservice.Configurations;
using csharp_choreography_saga.StockMicroservice.Entities;
using csharp_choreography_saga.StockMicroservice.Persistence.Base;
using csharp_choreography_saga.StockMicroservice.Services.RabbitMQ;
using csharp_choreography_saga.StockMicroservice.Services.Stock;
using csharp_choreography_saga.StockMicroservice.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_choreography_saga.StockMicroservice.Dependencies
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencie(this IServiceCollection services, WebApplicationBuilder builder)
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

            builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddHostedService<RabbitMQService>();
            builder.Services.AddSingleton<IBus, RabbitBus>();

            return services;
        }
    }
}
