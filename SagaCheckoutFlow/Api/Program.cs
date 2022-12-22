using System;
using Api;
using Common;
using Delivery;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders;
using Orders.Services;
using Payments;
using Saga;
using Serilog;
using Serilog.Events;
using Warehouse;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    BuildAndRun(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


void BuildAndRun(string[] strings)
{
    var builder = WebApplication.CreateBuilder(strings);

    var massTransitOptions = builder.Services.GetOptions<MassTransitOptions>("MassTransit");

    builder.Host.UseSerilog();
    builder.Services.AddControllers();

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddMassTransit(x =>
        {
            x.ConfigureWarehouse();
            x.ConfigureDelivery();
            x.ConfigurePayments();
            x.ConfigureSaga();
            x.AddMessageScheduler(new Uri("queue:scheduler"));

            switch (massTransitOptions.Transport)
            {
                default:
                case MassTransitOptions.TransportsTypes.InMemory:
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseInMemoryScheduler("scheduler");
                        cfg.ConfigureEndpoints(context);
                    });
                    break;
                case MassTransitOptions.TransportsTypes.RabbitMq:
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                        cfg.UseInMemoryScheduler("scheduler");
                        cfg.ConfigureEndpoints(context);
                    });
                    break;
            }
        }).AddOrders()
        .AddPayments()
        .AddDelivery()
        .AddSaga()
        .AddWarehouse();


    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}