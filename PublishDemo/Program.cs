using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PublishDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String routeKey = "wytRouteKey";
            String message = "Hello World!";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    //声明交换机（名称：log，类型：fanout（扇出））
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct",durable:true,autoDelete:false,arguments:null);

                    Byte[] body = Encoding.UTF8.GetBytes(message);

                    //消息推送
                    channel.BasicPublish(exchange: exchangeName, routingKey: routeKey, body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
