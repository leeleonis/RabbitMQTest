using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //創建連接工廠
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "root",//用戶名
                Password = "admin1234",//密碼
                HostName = "127.0.0.1"//rabbitmq ip
            };

            //創建連接
            var connection = factory.CreateConnection();
            //創建通道
            var channel = connection.CreateModel();

            //事件基本消費者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);

                Console.WriteLine($"收到消息： {message}");

                // Console.WriteLine($"收到該消息[{ea.DeliveryTag}] 延遲10s發送回執");
                // Thread.Sleep(10000);
                //確認該消息已被消費
                channel.BasicAck(ea.DeliveryTag, false);
                // Console.WriteLine($"已發送回執[{ea.DeliveryTag}]");
            };
            //啟動消費者 設置為手動應答消息
            channel.BasicConsume("hello", false, consumer);
            Console.WriteLine("消費者已啟動");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();

        }
    }
}
