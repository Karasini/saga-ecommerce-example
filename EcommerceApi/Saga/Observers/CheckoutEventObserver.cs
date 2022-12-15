using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Saga.Observers;

internal class CheckoutEventObserver : IEventObserver<CheckoutState>
{
    private readonly ILogger<CheckoutEventObserver> _logger;

    public CheckoutEventObserver(ILogger<CheckoutEventObserver> logger)
    {
        _logger = logger;
    }

    public Task PreExecute(BehaviorContext<CheckoutState> context)
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("PreExecution of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    public Task PreExecute<T>(BehaviorContext<CheckoutState, T> context) where T : class
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("PreExecution of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    public Task PostExecute(BehaviorContext<CheckoutState> context)
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("PostExecution of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    public Task PostExecute<T>(BehaviorContext<CheckoutState, T> context) where T : class
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("PostExecution of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    public Task ExecuteFault(BehaviorContext<CheckoutState> context, Exception exception)
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("ExecutionFault of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    public Task ExecuteFault<T>(BehaviorContext<CheckoutState, T> context, Exception exception) where T : class
    {
        EnrichLog(context.Saga);
        _logger.LogInformation("ExecutionFault of {Event}", context.Event.Name);
        return Task.CompletedTask;
    }

    private void EnrichLog(CheckoutState saga)
    {
        _logger.BeginScope("Checkout Saga {OrderId} {CurrentState}", saga.OrderId, saga.CurrentState);
    }
}