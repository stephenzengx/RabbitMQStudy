using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PubSub_ReceiveLog
{
    class ReceiveLog
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@139.198.122.83:5672") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
                    
                    //当我们不向QueueDeclare()提供任何参数时，我们会 使用生成的名称创建一个非持久的、独占的、自动删除的队列：
                    var queueName = channel.QueueDeclare().QueueName;
                    //交换和队列之间的这种关系称为绑定
                    channel.QueueBind(queue: queueName,
                                      exchange: "logs",
                                      routingKey: "");

                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    };

                    //Start a Basic content-class consumer.
                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
