using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events;

public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _domainsEvents = new List<BaseEvent>();
    
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainsEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainsEvents.Remove(domainEvent);
    }

    public void ClearDomainEvent()
    {
        _domainsEvents.Clear();
    }

    public IReadOnlyCollection<BaseEvent> GetDomainEvents()
    {
        return _domainsEvents.AsReadOnly();
    }
}