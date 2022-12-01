using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders;
using Orders.Services;
using Payments;
using Saga;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMassTransit(x =>
    {
        x.AddMessageScheduler(new Uri("queue:scheduler"));
        x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
        x.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>(typeof(CheckoutStateMachineDefinition))
            .InMemoryRepository();
    }).AddOrders()
    .AddPayments();


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