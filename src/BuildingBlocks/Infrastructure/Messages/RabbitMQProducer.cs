using System.Runtime.Serialization;
using System.Text;
using Contracts.Common.Interfaces;
using Contracts.Messages;
using RabbitMQ.Client;

namespace Infrastructure.Messages;

public class RabbitMQProducer : IMessageProducer
{
    private readonly ISerializeService _serializable;

    public RabbitMQProducer(ISerializeService serializable)
    {
        _serializable = serializable;
    }

    public async Task SendMessageAsync<T>(T message)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        // Create a connection and channel
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Declare the queue
        await channel.QueueDeclareAsync("orders", exclusive: false);

        // Serialize the message
        var jsonData = _serializable.Serialize(message); // Ensure _serializable is properly initialized
        var body = Encoding.UTF8.GetBytes(jsonData);

        // Publish the message
        await channel.BasicPublishAsync(exchange: "", routingKey: "orders", body: body);
    }

}