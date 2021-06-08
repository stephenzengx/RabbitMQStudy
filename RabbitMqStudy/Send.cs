using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqStudy
{
    class Send
    {
        static void Main(string[] args)
        {
            //工厂创建连接 到rabbitmq host (host格式 amqp://user:pass@hostName:port/vhost)
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@139.198.122.83:5672") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    string message = "Hello World!";
                    while (message != "exit")
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                               routingKey: "hello",
                                               basicProperties: null,
                                               body: body);
                        Console.WriteLine(" [x] Sent {0},exit to quit.", message);
                        message = Console.ReadLine();
                    }                  
                }
            }
        }
    }
}
