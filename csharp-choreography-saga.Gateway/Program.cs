using csharp_choreography_saga.Gateway.Dependencies;
using Ocelot.Middleware;

namespace csharp_choreography_saga.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDependencies(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseHealthChecks("/health");

        app.UseOcelot();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
