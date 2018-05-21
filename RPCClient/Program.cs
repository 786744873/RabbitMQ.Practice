using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RPCClient rpcClient = new RPCClient();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");

            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
            Console.ReadKey();
        }
    }

    public class RPCClient
    {
        private IConnection connection;
        private IModel channel;
        private string replayQueueName;
        private QueueingBasicConsumer consumer;

        public RPCClient()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.UserName = "wyt";
            factory.Password = "wyt";
            factory.VirtualHost = "/wyt";

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replayQueueName = channel.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: replayQueueName, autoAck: true, consumer: consumer);
        }

        public string Call(string message)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replayQueueName;
            props.CorrelationId = corrId;

            Byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId==corrId)
                {
                    return Encoding.UTF8.GetString(ea.Body);
                }
            }
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
