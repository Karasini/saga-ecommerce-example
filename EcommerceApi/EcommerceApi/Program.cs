using MassTransit;
using Orders.Services;
using Saga;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
    x.AddSagaStateMachine<CheckoutStateMachine, CheckoutState>(typeof(CheckoutStateMachineDefinition)).InMemoryRepository();
}).AddScoped<IOrdersService, OrdersService>();


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