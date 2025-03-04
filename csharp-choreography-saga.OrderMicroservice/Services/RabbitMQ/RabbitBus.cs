using csharp_choreography_saga.OrderMicroservice.Configurations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace csharp_choreography_saga.OrderMicroservice.Services.RabbitMQ
{
    public interface IBus
    {
        void Send<T>(string exchange, string queue, string routingKey, T message);
    }

    public class RabbitBus : IBus
    {
        internal readonly AppSetting _appSetting;

        public RabbitBus(IConfiguration configuration)
        {
            _appSetting = configuration.Get<AppSetting>()!;
        }

        public void Send<T>(string exchange, string queue, string routingKey, T message)
        {
            IConnection connection = CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.QueueBind(
                queue: queue,
                exchange: exchange,
                routingKey: routingKey,
                arguments: null
            );

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: messageBody
            );
        }

        private IConnection CreateConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = _appSetting.RabbitMQ.HostName,
                UserName = _appSetting.RabbitMQ.UserName,
                Password = _appSetting.RabbitMQ.Password,
                VirtualHost = "/"
            };
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
            connectionFactory.RequestedHeartbeat = TimeSpan.FromSeconds(15);
            connectionFactory.DispatchConsumersAsync = true;

            return connectionFactory.CreateConnection();
        }
    }
}
