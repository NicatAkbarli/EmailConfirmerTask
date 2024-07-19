using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqConfirmer.Producer
{
    public class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "confirmQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.ConfirmSelect();

                for (int i = 0; i < 10; i++)
                {
                    var message = $"Message {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "confirmQueue",
                                         basicProperties: null,
                                         body: body);
                }

                if (channel.WaitForConfirms())
                {
                    Console.WriteLine("Bütün mesajlar uğurla göndərildi və təsdiqləndi.");
                }
                else
                {
                    Console.WriteLine("Mesajlardan bəziləri göndərilmədi və təsdiqlənmədi.");
                }
            }
        }
    }
}