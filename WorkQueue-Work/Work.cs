using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkQueue_Work
{
    class Work
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@139.198.122.83:5672") };
            using (var connection = factory.CreateConnection())
            {
                Task task1 = Task.Run(()=> {
                    var channel = connection.CreateModel();                
                    //RabbitMQ不允许您使用不同的参数重新定义现有队列
                    channel.QueueDeclare(queue: "task_queue", //声明要消费的队列
                                         durable: true,// 队列持久化
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    //公平派遣
                    channel.BasicQos(0, 1, false);//在处理并确认上一条消息之前，不要将新消息发送给work

                    Console.WriteLine("work1 wait");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => //接受信息后的回调方法
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("work1 Received {0}", message);

                        Thread.Sleep(2 * 1000);

                        Console.WriteLine("work1 Done");
                        //手动发送ack
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
                    Console.ReadLine();
                });

                Task task2 = Task.Run(() => {

                    var channel = connection.CreateModel();
                    //using (var channel = connection.CreateModel())
                    //{                   
                    //RabbitMQ不允许您使用不同的参数重新定义现有队列
                    channel.QueueDeclare(queue: "task_queue", //声明要消费的队列
                                         durable: true,// 队列持久化
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    //公平派遣
                    channel.BasicQos(0, 1, false);//在处理并确认上一条消息之前，不要将新消息发送给work

                    Console.WriteLine("work2 wait");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => //接受信息后的回调方法
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("work2 Received {0}", message);

                        Thread.Sleep(2 * 1000);

                        Console.WriteLine("work2 Done");
                        //手动发送ack
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
                    Console.ReadLine();
                });

                Task.WaitAll(task1,task2);
                //}
            }
        }
    }
}
