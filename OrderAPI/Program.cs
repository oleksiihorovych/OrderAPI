using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderAPI.DataAccess;
// using OrderAPI.Services;
// using OrderAPI.Services.Abstractions;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DbAccess>(options =>
            options.UseInMemoryDatabase("OrdersDb"));

        builder.Services.AddControllers();
        builder.Logging.AddConsole();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Order API",
                Version = "v1",
                Description = "API for managing orders"
            });
        });

        // // Payment services

        // builder.Services.AddSingleton<IPaymentPublisher, PaymentPublisher>();
        // builder.Services.AddHostedService<PaymentConsumer>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
            c.RoutePrefix = string.Empty;
        });

        app.MapControllers();

        app.Run();
    }
}