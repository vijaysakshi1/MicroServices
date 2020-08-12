using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

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
            using IModel channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            string message = JsonConvert.SerializeObject(checkoutEvent);
            byte[] body = Encoding.UTF8.GetBytes(message);

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

            //using (IModel channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            //    string message = JsonConvert.SerializeObject(checkoutEvent);
            //    byte[] body = Encoding.UTF8.GetBytes(message);

            //    IBasicProperties properties = channel.CreateBasicProperties();
            //    properties.Persistent = true;
            //    properties.DeliveryMode = 2;

            //    channel.ConfirmSelect();
            //    channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
            //    channel.WaitForConfirmsOrDie();

            //    channel.BasicAcks += (sender, ebentArgs) =>
            //    {
            //        Console.WriteLine("Sent RabitMQ");
            //        //implement acks handle
            //    };

            //    channel.ConfirmSelect();
            //}
        }
    }
}
