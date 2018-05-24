using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutCustomerB
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.129";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "fanout", durable: true, autoDelete: false, arguments: null);

                    String queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "", arguments: null);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        Byte[] body = ea.Body;
                        String message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }
            }
        }
    }
}
