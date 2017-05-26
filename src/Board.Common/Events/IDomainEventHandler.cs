namespace Board.Common.Events
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        void ApplyEvent(TEvent @event);
    }
}