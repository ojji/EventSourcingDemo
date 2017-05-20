using System;

namespace Board.Api.Domain.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
    }
}