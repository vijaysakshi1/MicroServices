using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using MediatR;
using Newtonsoft.Json;
using Ordering.Application.Commands;
using Ordering.Core.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Ordering.API.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection connection;
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IOrderRepository repository;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IMediator mediator, IMapper mapper, IOrderRepository repository)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Consume()
        {
            IModel channel = connection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.BasketCheckoutQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceivedEvent;
            channel.BasicConsume(queue: EventBusConstants.BasketCheckoutQueue, autoAck: true, consumer: consumer);

        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.BasketCheckoutQueue)
            {
                string message = Encoding.UTF8.GetString(e.Body.Span);
                BasketCheckoutEvent basketCheckoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                CheckoutOrderCommand command = mapper.Map<CheckoutOrderCommand>(basketCheckoutEvent);
                Application.Responses.OrderResponse result = await mediator.Send(command);
            }
        }

        public void Disconnect()
        {
            connection.Dispose();
        }
    }
}
