using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String message = "Hello World!";

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
                    channel.ExchangeDeclare(exchange: exchangeName, type: "fanout", durable: true, autoDelete: false, arguments: null);


                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 10; i++)
                    {
                        Byte[] body = Encoding.UTF8.GetBytes(message + i);
                        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: properties, body: body);
                    }

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();


        }
    }
}
