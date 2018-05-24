using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ExceptionHandlerConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            String exchangeName = "wytExchange";
            String queueName = "wytQueue";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.133";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            IConnection connection;
            IModel channel=null;
            BasicDeliverEventArgs basicDeliverEventArgs=null;

            try
            {


                int i = 0;

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: String.Empty, arguments: null);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    if (i++ == 3)
                    {
                        basicDeliverEventArgs = ea;
                        throw new Exception("抛出异常");
                    }

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);

                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                Console.WriteLine("等待接收消息");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                channel.BasicAck(basicDeliverEventArgs.DeliveryTag, multiple: false);
            }
        }
    }
}
