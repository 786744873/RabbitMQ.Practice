using RabbitMQ.Client;
using System;
using System.Text;

namespace ExceptionHandlerProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String queueName = "wytQueue";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.130";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: String.Empty, arguments: null);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 10; i++)
                    {
                        Byte[] body = Encoding.UTF8.GetBytes("Hello World -- " + i);

                        channel.BasicPublish(exchange: exchangeName, routingKey: String.Empty, basicProperties: properties, body: body);
                    }
                }
            }

            Console.WriteLine("发送完成");
            Console.ReadKey();
        }
    }
}
