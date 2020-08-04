using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading;

namespace EventBusRabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory connectFactory;
        private IConnection connection;
        private bool disposed;

        public RabbitMQConnection(IConnectionFactory connectFactory)
        {
            this.connectFactory = connectFactory ?? throw new ArgumentNullException(nameof(connectFactory));
            if (!IsConnected)
            {
                TryConnect();
            }
        }

        public bool IsConnected => (connection != null && connection.IsOpen && !disposed);

        public IModel CreateModel()
        {
            if (!IsConnected) throw new InvalidOperationException("No Rabbit Connection");

            return connection.CreateModel();
        }

        public void Dispose()
        {
            if (disposed) return;
            try
            {
                connection.Dispose();
                disposed = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool TryConnect()
        {
            try
            {
                connection = connectFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                Thread.Sleep(2000);
                connection = connectFactory.CreateConnection();
            }

            if (IsConnected) return true;
            else return false;
        }
    }
}
