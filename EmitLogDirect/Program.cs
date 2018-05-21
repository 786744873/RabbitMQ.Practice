using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmitLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.UserName = "wyt";
            factory.Password = "wyt";
            factory.VirtualHost = "/wyt";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                    var severity = (args.Length > 0) ? args[0] : "info";
                    var message = (args.Length > 1)
                                  ? string.Join(" ", args.Skip(1).ToArray())
                                  : "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    //推送消息
                    channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);

                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
