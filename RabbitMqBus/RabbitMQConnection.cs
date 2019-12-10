using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RabbitMqBus
{
    public class RabbitMQConnection: IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _retryCount;
        IConnection _connection;
        bool _disposed;
        readonly object sync_root = new object();
        public RabbitMQConnection(IConnectionFactory connectionFactory, int retryCount =5)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
        }
        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to performa this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                if (_connection != null)
                    _connection.Dispose();
            }
            catch (IOException ex)
            {
                //_logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            lock (sync_root)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => {
                      var test = ex.Message;
                    });

                policy.Execute(() => {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                    _connection.CallbackException += _connection_CallbackException;
                    _connection.ConnectionBlocked += _connection_ConnectionBlocked;

                    
                    return true;
                }
                else
                {
                    return false;

                }
            }
        }
        private void _connection_ConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            //_logger.LogWarning("A RabbitMQ connection throw exception. Trying to  re connect...");

            TryConnect();
        }

        private void _connection_CallbackException(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            //_logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            //_logger.LogWarning("A RabbitMQ connection is shutdown. Trying to  re connect...");

            TryConnect();
        }
    }
}
