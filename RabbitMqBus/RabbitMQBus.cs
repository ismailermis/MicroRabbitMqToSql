using Domain.Core.Bus;
using Domain.Core.Commands;
using Domain.Core.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqBus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRabbitMQConnection _persistentConnection;
        private readonly int _retryCount;
        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory, IRabbitMQConnection rabbitMQConnection,int retryCount=5)
        {
            _mediator = mediator;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _persistentConnection = rabbitMQConnection ?? throw new ArgumentNullException(nameof(rabbitMQConnection));
            _retryCount = retryCount;
        }

        public void Publish<T>(T @event) where T : Event
        {
            try
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }
                var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                   // _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });
                //var factory = new ConnectionFactory() { HostName = "rabbitmqX", Port = 5672 };
                //factory.UserName = "springcloud";
                //factory.Password = "123456";
                //factory.AutomaticRecoveryEnabled = true;
                //factory.TopologyRecoveryEnabled = true;
                //factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
                //factory.UseBackgroundThreadsForIO = true;
                //factory.RequestedHeartbeat = 2;
               // using (var connection = factory.CreateConnection())
                using (var channel = _persistentConnection.CreateModel())
                {
                   
                    policy.Execute(()=>
                    {
                        var eventName = @event.GetType().Name;
                        channel.QueueDeclare(eventName, false, false, false, null);
                        var message = JsonConvert.SerializeObject(@event);
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", eventName, null, body);
                    }
                    );
                }
                Console.WriteLine("publish");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error : " + ex.Message);

            }


        }
        //subscription = {MicroRabbit.Transfer.Domain.EventHandlers.TransferEventHandler}
        //Consumer.Domain.EventHandlers.TransferEventHandler}

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);
            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }
            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"HAndler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventName].Add(handlerType);
            StartBasicConsum<T>();
        }

        private void StartBasicConsum<T>() where T : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "springcloud",
                Password = "123456",
                DispatchConsumersAsync = true
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Recived;
            channel.BasicConsume(eventName, true, consumer);
        }
        private async Task Consumer_Recived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body);
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        // var handler   = Activator.CreateInstance(subscription); //old code
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;
                        var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }

            }
        }

        public void Dispose()
        {
            //channel?.Dispose();
            //connection?.Dispose();
        }
    }
}
