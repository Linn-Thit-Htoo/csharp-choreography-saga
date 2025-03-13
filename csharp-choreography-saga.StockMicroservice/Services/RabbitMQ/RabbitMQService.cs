using System.Text;
using csharp_choreography_saga.StockMicroservice.Configurations;
using csharp_choreography_saga.StockMicroservice.Models;
using csharp_choreography_saga.StockMicroservice.Services.Stock;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace csharp_choreography_saga.StockMicroservice.Services.RabbitMQ;

public class RabbitMQService : BackgroundService
{
    private readonly AppSetting _appSetting;
    private readonly ILogger<RabbitMQService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMQService(
        ILogger<RabbitMQService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<AppSetting> options
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _appSetting = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IConnection connection = CreateConnection();
        var channel = connection.CreateModel();

        foreach (var item in _appSetting.RabbitMQ.QueueList)
        {
            channel.ExchangeDeclare(item.Exchange, ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(item.Queue, true, false, false);
            channel.QueueBind(item.Queue, item.Exchange, item.RoutingKey, null);
            channel.BasicQos(0, 1, false);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (ea.RoutingKey.Equals("orderdirect"))
                {
                    var requestModel = JsonConvert.DeserializeObject<OrderCreatedEvent>(content);
                    var scope = _scopeFactory.CreateScope();
                    var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();
                    var bus = scope.ServiceProvider.GetRequiredService<IBus>();

                    bool isStockReductionSuccessful = await stockService.ReduceStockAsync(
                        requestModel!
                    );

                    if (!isStockReductionSuccessful)
                    {
                        // publish to compensate the order
                        CompensateOrderEvent compensateOrderEvent =
                            new() { OrderId = requestModel!.OrderId };
                        bus.Send(
                            "DirectExchange",
                            "OrderQueue",
                            "orderfaildirect",
                            compensateOrderEvent
                        );
                    }
                }

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(item.Queue, false, consumer);
        }
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
