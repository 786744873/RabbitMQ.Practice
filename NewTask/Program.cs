using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "wyt", Password = "wyt", VirtualHost = "/wyt" };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //声明一个队列（队列名，是否持久化，是否排他，是否自动删除,）
                    channel.QueueDeclare(
                        queue: "task_queue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    String message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    //标记消息是否是持久化的
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    //消息推送（）
                    channel.BasicPublish(
                    exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public static string GetMessage(string[] args)
        {
            return args.Length > 0 ? String.Join(" ", args) : "Hello World";
        }
    }
}
