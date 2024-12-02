namespace EventBus.Messages;

public record IntegrationBaseEvent() : IIntegrationEvent
{
    public DateTime CreationDate { get; set; }
    public Guid Id { get; set; }
}