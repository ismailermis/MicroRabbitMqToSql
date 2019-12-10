using Microsoft.Extensions.DependencyInjection;
using System;

namespace RabbitMq.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        private void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.Registerservices(services);
        }
    }
}
