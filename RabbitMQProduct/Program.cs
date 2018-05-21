﻿using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.IO;
using System.Text;

namespace RabbitMQProduct
{
    class Program
    {
        static void Main(string[] args)
        {
            Send();
        }

        public static void Send()
        {
            try
            {
                var factory = new ConnectionFactory();
                factory.HostName = "localhost";//RabbitMQ服务在本地运行
                factory.UserName = "guest";//用户名
                factory.Password = "guest";//密码

                using (IConnection connection =factory.CreateConnection())
                {
                    using (IModel channel =connection.CreateModel())
                    {
                        channel.QueueDeclare("hello", false, false, false, null);//创建一个名称为hello的消息队列
                        string message = "Hello World";//传递的消息内容
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", "hello", null, body);//开始传递
                        Console.WriteLine("已发送： {0}", message);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally {

            }
        }
    }
}
