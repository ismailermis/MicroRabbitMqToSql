using Common.IoC;
using Consumer.Domain.EventHandlers;
using Consumer.Domain.Events;
using Domain.Core.Bus;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Consumer.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = DependencyContainer.Registerservices();
            var eventBus = provider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<TransferCreatedEvent, TransferEventHandler>();
           Console.ReadKey();
        }
        
    }
}
