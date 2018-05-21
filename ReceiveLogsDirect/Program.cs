using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogsDirect
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

                    //设置临时队列
                    String queueName = channel.QueueDeclare().QueueName;
                    //绑定多个队列
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: "info");
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: "info2");
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: "info3");

                    Console.WriteLine(" [*] Waiting for messages.");

                    //定义消费者规则
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
                    };

                    //将消费者与队列进行绑定
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
