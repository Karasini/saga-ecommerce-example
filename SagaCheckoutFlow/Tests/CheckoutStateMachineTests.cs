using MassTransit.SagaStateMachine;
using MassTransit.Visualizer;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Saga;

namespace Tests
{
    [TestFixture]
    public class CheckoutStateMachineTests
    {
        [Test]
        public void ShowTheGraph()
        {
            var orderStateMachine = new CheckoutStateMachine(new NullLogger<CheckoutStateMachine>());

            var graph = orderStateMachine.GetGraph();

            var generator = new StateMachineGraphvizGenerator(graph);

            var dots = generator.CreateDotFile();

            Console.WriteLine(dots);

        }
    }
}