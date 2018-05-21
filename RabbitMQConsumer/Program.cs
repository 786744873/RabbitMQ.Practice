using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Recivie();
        }

        public static void Recivie()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("hello", false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("已接收： {0}", message);
                    };
                    Console.ReadKey();
                }
            }
        }
    }
}
