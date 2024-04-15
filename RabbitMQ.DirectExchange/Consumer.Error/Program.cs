﻿// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

    var queueName = channel.QueueDeclare().QueueName;

    channel.QueueBind(queue: queueName,
                             exchange: "direct_logs",
                             routingKey: "error");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (sender, e) =>
    {
        var body = e.Body;
        var message = Encoding.UTF8.GetString(body.ToArray());
        Console.WriteLine(" Received message: {0}", message);
    };

    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine($"Subscribed to the queue '{queueName}'");

    Console.ReadLine();
}