using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration configuration;
        private readonly IConnection connection;
        private readonly IModel channel;

        public MessageBusClient(IConfiguration configuration)
        {
            this.configuration = configuration;
            var factory = new ConnectionFactory() { 
                HostName = this.configuration["RabbitMQHost"],
                Port = int.Parse(this.configuration["RabbitMQPort"])
            };
            try
            {
                connection = factory.CreateConnection();
                connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: this.configuration["RabbitMQExchange"], type: ExchangeType.Fanout);

                Console.WriteLine("--> Connected to the Message Bus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}.");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            if (connection.IsOpen)
            {
                var message = JsonSerializer.Serialize(platformPublishedDto);
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            } else
            {
                Console.WriteLine("--> RabbitMQ Connection closed, not sending.");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("--> MessageBus Disposing...");
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
            Console.WriteLine("--> MessageBus Disposed.");
        }


        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: configuration["RabbitMQExchange"], 
                routingKey: "", 
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> We have sent '{message}'.");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown.");
        }
    }
}
