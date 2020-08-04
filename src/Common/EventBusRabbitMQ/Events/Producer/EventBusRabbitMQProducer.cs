using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EventBusRabbitMQ.Events.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent checkoutEvent)
        {
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var message = JsonConvert.SerializeObject(checkoutEvent);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, ebentArgs) =>
                {
                    Console.WriteLine("Sent RabitMQ");
                    //implement acks handle
                };

                channel.ConfirmSelect();
            }
        }
    }
}
