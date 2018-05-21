using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
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
                    //声明队列
                    channel.QueueDeclare(
                        queue: "task_queue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    //限制一次推送一条消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine(" [*] Waiting for messages.");

                    //定义消费者规则
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (mdoel, ea) =>
                    {
                        Byte[] body = ea.Body;
                        String message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        Thread.Sleep(100);

                        Console.WriteLine(" [x] Done");

                        //ACK手动确认
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    //将消费者与队列进行绑定
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
