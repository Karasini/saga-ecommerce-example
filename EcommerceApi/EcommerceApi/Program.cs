using System;
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

    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddMassTransit(x =>
        {
            var schedulerEndpoint = new Uri("queue:scheduler");
            
            x.ConfigureWarehouse();
            x.ConfigureDelivery();
            x.ConfigureSaga();
            x.AddMessageScheduler(schedulerEndpoint);

            // x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
            x.UsingRabbitMq((context,cfg) =>
            {
                cfg.Host("localhost", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.UseMessageScheduler(schedulerEndpoint);
                cfg.ConfigureEndpoints(context);
            });

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