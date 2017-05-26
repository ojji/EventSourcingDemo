using System;

namespace Board.Common.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
    }
}