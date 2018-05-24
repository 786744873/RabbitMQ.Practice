using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ColonyProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String queueName = "wytQueue";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.130";
            factory.Port = 6572;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: String.Empty, arguments: null);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    Byte[] body = Encoding.UTF8.GetBytes("Hello World");

                    channel.BasicPublish(exchange: exchangeName, routingKey: String.Empty,basicProperties: properties, body: body);
                }
            }
        }
    }
}
