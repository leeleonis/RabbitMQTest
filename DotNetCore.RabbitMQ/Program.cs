using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
			//創建連接工廠
			ConnectionFactory factory = new ConnectionFactory
	        {
		        UserName = "root",//用户名
		        Password = "admin1234",//密码
		        HostName = "127.0.0.1"//rabbitmq ip
	        };

			//創建連接
			var connection = factory.CreateConnection();
			//创建通道
	        var channel = connection.CreateModel();
			//聲明一個隊列
			channel.QueueDeclare("hello", false, false, false, null);

			Console.WriteLine("\nRabbitMQ連接成功，請輸入消息，輸入exit退出！");

	        string input;
	        do
	        {
		        input = Console.ReadLine();

		        var sendBytes = Encoding.UTF8.GetBytes(input);
				//发布消息
				channel.BasicPublish("", "hello", null, sendBytes);

			} while (input.Trim().ToLower()!="exit");
	        channel.Close();
	        connection.Close();

		}
    }
}
