using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductAckDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String routeKeyName = "wytRouteKey";
            String message = "Task --";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.129";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true, autoDelete: false, arguments:null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 100; i++)
                    {
                        Byte[] body = Encoding.UTF8.GetBytes(message+i);

                        channel.BasicPublish(exchange: exchangeName, routingKey: routeKeyName, basicProperties: properties, body: body);
                    }
                }
            }
        }
    }
}
