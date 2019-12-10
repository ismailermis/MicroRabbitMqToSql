using Consumer.Application.Data;
using Consumer.Application.Interface;
using Consumer.Domain.EventHandlers;
using Consumer.Domain.Events;
using Domain.Core.Bus;
using Infra.DataLayer.Context;
using Infra.DataLayer.Interface;
using Infra.DataLayer.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Application.Interfaces;
using Producer.Application.Services;
using Producer.Domain.CommandHandlers;
using Producer.Domain.Commands;
using RabbitMQ.Client;
using RabbitMqBus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.IoC
{
    public static class DependencyContainer
    {
        public static IServiceProvider Registerservices()
        {
          

            var configurationRoot = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

            var services = new ServiceCollection();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseSqlServer("data source =150.1.10.199;Database=NORTHWND;User Id=sa;Password=Password1;MultipleActiveResultSets=true");

            });
            //Domain Bus
            services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory, GetRabbitMQConnection(),5);
            });
            
            services.AddTransient<TestRecProducerService, TestRecProducerService>();
            //Subsrictions
           services.AddTransient<TransferEventHandler>();
            //Domain Events
           services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();

            ////Domain Banking Commands
            services.AddTransient<ITestRecProducer, TestRecProducerService>();

            services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

            ////Applicaiton Services IStaticCacheManager
            //services.AddTransient<ICacheManager, CacheManager>();
            //services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<ITransferService, TransferService>();

            ////Data 
            //services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITestRecRepository, TestRecRepository>();
            //services.AddTransient<BankingDbContext>();
            //services.AddTransient<TransferDbContext>();

            ////Mongo DB
            //services.AddScoped<IMongoContext, MongoContext>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<ITransferLogMongoRepository, TransferLogMongoRepository>();
            //services.AddScoped<IProductRepository, ProductRepository>();
            return services.BuildServiceProvider();
        }

        public static IRabbitMQConnection GetRabbitMQConnection()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "springcloud";
            factory.Password = "123456";
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
            factory.UseBackgroundThreadsForIO = true;
            factory.RequestedHeartbeat = 2;

            return new RabbitMQConnection(factory,5);
        }
    }
}
