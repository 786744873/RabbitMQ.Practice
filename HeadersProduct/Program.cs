using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HeadersProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchangeHeaders";

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
                    channel.ExchangeDeclare(exchange: exchangeName, type: "headers", durable: true, autoDelete: false, arguments: null);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<String, Object>()
                    {
                        {"user","wyt" },
                        {"password","wyt"}
                    };

                    Byte[] body = Encoding.UTF8.GetBytes("Hello World");

                    channel.BasicPublish(exchange: exchangeName, routingKey: String.Empty, basicProperties: properties, body: body);
                }
            }

            Console.Write("发布成功！");

            Console.ReadKey();
        }
    }
}
