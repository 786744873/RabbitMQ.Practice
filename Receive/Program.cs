using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 ,UserName ="wyt",Password="wyt",VirtualHost="/wyt"};
            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    //声明一个队列（队列名，是否持久化，是否自动删除）
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    //定义消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    //消费者监听处理消息逻辑
                    consumer.Received += (model, ea) =>
                    {
                        Byte[] body = ea.Body;
                        String message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };

                    //将消费者绑定到队列（队列名称,是否自动进行消息确认机制，消费者z）
                    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }

    public class Receive
    {

    }
}
