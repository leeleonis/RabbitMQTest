using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ProducerWithFanoutExchange
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string exchangeName = "TestFanoutChange";
            string queueName1 = "hello1";
            string queueName2 = "hello2";
            string routeKey = "";

            //創建連接工廠
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "root",//用户名
                Password = "admin1234",//密码
                HostName = "127.0.0.1"//rabbitmq ip
            };

            //創建連接
            var connection = factory.CreateConnection();
            //創建通道
            var channel = connection.CreateModel();

            //定義一個Direct類型交換機
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, false, null);

            //定義隊列1
            channel.QueueDeclare(queueName1, false, false, false, null);
            //定義隊列2
            channel.QueueDeclare(queueName2, false, false, false, null);

            //將隊列綁定到交換機
            channel.QueueBind(queueName1, exchangeName, routeKey, null);
            channel.QueueBind(queueName2, exchangeName, routeKey, null);

            //生成兩個隊列的消費者
            ConsumerGenerator(queueName1);
            ConsumerGenerator(queueName2);

            Console.WriteLine($"\nRabbitMQ連接成功，\n\n請輸入消息，輸入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //發布消息
                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }

        /// <summary>
        /// 根據隊列名稱生成消費者
        /// </summary>
        /// <param name="queueName"></param>
        private static void ConsumerGenerator(string queueName)
        {
            //創建連接工廠
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "admin",//用戶名
                Password = "admin",//密碼
                HostName = "192.168.157.130"//rabbitmq ip
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

                Console.WriteLine($"Queue:{queueName}收到消息： {message}");
                //確認該消息已被消費
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //啟動消費者 設置為手動應答消息
            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine($"Queue:{queueName}，消費者已啟動");
        }
    }
}