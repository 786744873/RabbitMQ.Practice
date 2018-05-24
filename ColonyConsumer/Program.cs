using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ColonyConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String queueName = "wytQueue";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.133";
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

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);

                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                }
            }
        }
    }
}
