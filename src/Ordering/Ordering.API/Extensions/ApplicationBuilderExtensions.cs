using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.API.RabbitMQ;

namespace Ordering.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static EventBusRabbitMQConsumer listner { get; set; }
        public static IApplicationBuilder UseRabbitListner(this IApplicationBuilder app)
        {
            listner = app.ApplicationServices.GetService<EventBusRabbitMQConsumer>();
            IHostApplicationLifetime life = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            listner.Consume();
        }
        private static void OnStopping()
        {
            listner.Disconnect();
        }

    }
}
