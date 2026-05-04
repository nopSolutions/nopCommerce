namespace Nop.Services.Integration.RabbitMQ;

/// <summary>
/// Represents a RabbitMQ message publisher
/// </summary>
public partial interface IRabbitMqPublisher
{
    /// <summary>
    /// Publishes a message to the specified exchange
    /// </summary>
    /// <param name="exchange">Exchange name</param>
    /// <param name="routingKey">Routing key</param>
    /// <param name="messageBody">Message body (JSON string)</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PublishAsync(string exchange, string routingKey, string messageBody);
}
