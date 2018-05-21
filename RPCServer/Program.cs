using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPCServer
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

            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    //每次推送一条消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    //定义消费者规则
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        string response = null;

                        Byte[] body = ea.Body;
                        IBasicProperties props = ea.BasicProperties;
                        IBasicProperties replayProps= channel.CreateBasicProperties();
                        replayProps.CorrelationId = props.CorrelationId;

                        try
                        {
                            String message = Encoding.UTF8.GetString(body);
                            Int32 n = Int32.Parse(message);
                            Console.WriteLine(" [.] fib({0})", message);
                            response = fib(n).ToString();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                            response = "";
                        }
                        finally
                        {
                            Byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replayProps, body: responseBytes);

                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    };

                    channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        private static int fib(int n)
        {
            if (n==0||n==1)
            {
                return n;
            }
            return fib(n - 1) + fib(n - 2);
        }
    }
}
