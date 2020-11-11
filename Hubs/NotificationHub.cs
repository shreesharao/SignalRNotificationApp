using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Terarecon.Eureka.Cardiac.NotificationService.Models;
using System;
using System.Collections.Generic;

namespace Terarecon.Eureka.Cardiac.NotificationService.Hubs
{

    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly List<ResultEventArgs> _notifications;

        public NotificationHub(ILogger<NotificationHub> logger, IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _notifications = LoadNotifications();
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            ConnectionFactory factory = new ConnectionFactory();

            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "rabbitserver";
            //factory.HostName = "localhost";
            factory.Port = 5672;
            factory.ClientProvidedName = "Terarecon.Eureka.Cardiac.ImageService.Consumer";

            factory.DispatchConsumersAsync = true;

            IConnection conn = factory.CreateConnection();
            _channel = conn.CreateModel();

            _channel.ExchangeDeclare("cardiac.image.dx", ExchangeType.Direct, true);
            _channel.QueueDeclare("image.ai.notification.q", true, false, false, null);
            _channel.QueueBind("image.ai.notification.q", "cardiac.image.dx", "image.ai.notification.service", null);
        }
        public async Task SendAsync()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = System.Text.Encoding.UTF8.GetString(body);
                try
                {
                    var command = JsonSerializer.Deserialize<Command<object>>(msg);

                    if (command != null)
                    {
                        var eArgs = JsonSerializer.Deserialize<ResultEventArgs>(command.Args.ToString());
                        _notifications.Add(eArgs);
                        var result = JsonSerializer.Serialize(eArgs);
                        _logger.LogInformation(result);
                        for (int i = 0; i < 3; i++)
                        {
                            await _hubContext.Clients.All.SendAsync("broadcastMessage", $"{result}");
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            // this consumer tag identifies the subscription
            // when it has to be cancelled
            var consumerTag = _channel.BasicConsume("image.ai.notification.q", false, consumer);
        }

        public async Task SendHistoryAsync()
        {
            var result = JsonSerializer.Serialize(_notifications);
            _logger.LogInformation($"sending notification history - {result}");
            await _hubContext.Clients.All.SendAsync("broadcastMessage", $"{result}");
        }
        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("welcomeMessage", "Connection Successful");
        }

        private List<ResultEventArgs> LoadNotifications()
        {
            var list = new List<ResultEventArgs>(){
                new ResultEventArgs(){
                    PatientId = "11788766887033",
                    PatientName = "Fall 5",
                    StudyInstanceUid = "1.2.276.0.50.192168001099.8829267.14547392.4",
                    StudyDescription = "MRT Oberbauch",
                    SeriesInstanceUid = "1.2.276.0.50.192168001099.8829267.14547392.385",
                    SeriesNumber = 1101,
                    SopInstanceUid = "1.2.276.0.50.192168001099.8829267.14547392.419"
                },
                new ResultEventArgs(){
                    PatientId = "11788766887033",
                    PatientName = "Fall 5",
                    StudyInstanceUid = "1.2.276.0.50.192168001099.8829267.14547392.4",
                    StudyDescription = "MRA",
                    SeriesInstanceUid = "1.2.276.0.50.192168001092.11517584.14547392.2",
                    SeriesNumber = 1,
                    SopInstanceUid = "1.2.276.0.50.192168001092.11517584.14547392.3"
                },
                 new ResultEventArgs(){
                    PatientId = "11788766887033",
                    PatientName = "Fall 5",
                    StudyInstanceUid = "1.2.276.0.50.192168001099.8829267.14547392.4",
                    StudyDescription = "MVP",
                    SeriesInstanceUid = "1.2.276.0.50.192168001092.11517584.14547392.3",
                    SeriesNumber = 4,
                    SopInstanceUid = "1.2.276.0.50.192168001092.11517584.14547392.10"
                },
            };
            return list;
        }
    }
}