using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HeadersConsumerA
{
    class Program
    {
        static void Main(string[] args)
        {



            String exchangeName = "wytExchangeHeaders";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.129";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            bool flag = true;
            string pattern = "";
            while (flag)
            {
                Console.WriteLine("请选择headers匹配模式  1(any)/2(all)");
                pattern = Console.ReadLine();
                if (pattern == "1" || pattern == "2")
                    flag = false;
                else
                    Console.Write("请做出正确的选择");
            }
            Console.WriteLine("正等待接受消息");
            //根据声明使用的队列
            var headersType = pattern == "1" ? "any" : "all";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Headers, durable: true, autoDelete: false, arguments: null);

                    String queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: String.Empty, arguments: new Dictionary<String, Object>
                    {
                        {"x-math",headersType },
                        {"user","wyt" },
                        {"password","wyt" }
                    });

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var msg = Encoding.UTF8.GetString(ea.Body);

                        Console.WriteLine($"{msg}");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);


                    Console.ReadKey();

                }
            }
        }
    }
}
