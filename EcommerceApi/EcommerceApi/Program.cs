using System;
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
    .MinimumLevel.Debug()
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
            x.ConfigureWarehouse();
            x.AddMessageScheduler(new Uri("queue:scheduler"));
            x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
            x.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>(typeof(CheckoutStateMachineDefinition))
                .InMemoryRepository();
        }).AddOrders()
        .AddPayments()
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