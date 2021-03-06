﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PublishDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            String queueName = "wytQueue";
            String exchangeName = "wytExchange";
            String routeKeyName = "wytRouteKey";
            String message = "Hello World!";

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.63.129";
            factory.Port = 5672;
            factory.VirtualHost = "/wyt";
            factory.UserName = "wyt";
            factory.Password = "wyt";

            using (IConnection connection=factory.CreateConnection())
            {
                using (IModel channel=connection.CreateModel())
                {
                    //声明交换机（名称：log，类型：fanout（扇出））
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct",durable:false,autoDelete:false,arguments:null);

                    //channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    //channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routeKeyName, arguments: null);

                    Byte[] body = Encoding.UTF8.GetBytes(message);

                    //消息推送
                    channel.BasicPublish(exchange: exchangeName, routingKey: routeKeyName, body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
