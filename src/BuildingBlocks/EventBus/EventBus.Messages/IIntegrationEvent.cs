namespace EventBus.Messages;

public interface IIntegrationEvent
{
    DateTime CreationDate { get; set; }
    Guid Id { get; set; }
}