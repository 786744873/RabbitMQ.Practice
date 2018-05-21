using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "wyt", Password = "wyt", VirtualHost = "/wyt" };
            using (IConnection connection= factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    //声明一个队列（队列名，是否持久化，是否自动删除）
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    string message = "hello world";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
    }

    public class Send
    {

    }
}
