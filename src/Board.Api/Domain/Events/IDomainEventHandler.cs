namespace Board.Api.Domain.Events
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        void ApplyEvent(TEvent @event);
    }
}