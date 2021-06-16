using RabbitMQ.Client;
using System;
using System.Text;

namespace ProducerWithExchange
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string exchangeName = "TestChange";
            string queueName = "hello";
            string routeKey = "helloRouteKey";

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
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null);

            //定義一個隊列
            channel.QueueDeclare(queueName, false, false, false, null);

            //將隊列綁定到交換機
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            Console.WriteLine($"\nRabbitMQ連接成功,Exchange：{exchangeName}，Queue：{queueName}，Route：{routeKey}，\n\n請輸入消息，輸入exit退出！");

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
    }
}