using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace PubSub_EmtiLog
{
    class EmitLog
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@139.198.122.83:5672") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明交换机 fanout,direct,topic
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    string message = "Hello World!";
                    while (message != "exit")
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "logs",
                                               routingKey: "",
                                               basicProperties: null,
                                               body: body);
                        Console.WriteLine(" [x] Sent {0},exit to quit.", message);
                        message = Console.ReadLine();
                    }
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
