using System;
using System.Text;
using RabbitMQ.Client;

namespace WorkQueue_NewTask
{
    class NewTask
    {
        static void Main(string[] args)
        {
            //工厂创建连接 到rabbitmq host (host格式 amqp://user:pass@hostName:port/vhost)
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@139.198.122.83:5672") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue",
                                         durable: true, //队列持久化
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    
                    var message = "Start";
                    while (message != "exit")
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;//消息持久化

                        channel.BasicPublish(exchange: "",
                                               routingKey: "task_queue",
                                               basicProperties: properties,
                                               body: body);

                        Console.Write(" [NewTask] Sent {0},exit to quit.", message);
                        message = Console.ReadLine();
                    }
                }
            }
        }
    }
}
