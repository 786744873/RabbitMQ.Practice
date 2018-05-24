using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TopicProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchangeTopic";
            String routeKeyName1 = "black.critical.high";
            String routeKeyName2 = "red.critical.high";
            String routeKeyName3 = "white.critical.high";
            
            String message1 = "black-critical-high!";
            String message2 = "red-critical-high!";
            String message3 = "white-critical-high!";

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
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true, autoDelete: false, arguments: null);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    Byte[] body1 = Encoding.UTF8.GetBytes(message1);
                    Byte[] body2 = Encoding.UTF8.GetBytes(message2);
                    Byte[] body3 = Encoding.UTF8.GetBytes(message3);

                    //消息推送
                    channel.BasicPublish(exchange: exchangeName, routingKey:routeKeyName1,basicProperties: properties, body: body1);
                    channel.BasicPublish(exchange: exchangeName, routingKey: routeKeyName2, basicProperties: properties, body: body2);
                    channel.BasicPublish(exchange: exchangeName, routingKey: routeKeyName3, basicProperties: properties, body: body3);

                    Console.WriteLine(" [x] Sent {0}", message1);
                    Console.WriteLine(" [x] Sent {0}", message2);
                    Console.WriteLine(" [x] Sent {0}", message3);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
