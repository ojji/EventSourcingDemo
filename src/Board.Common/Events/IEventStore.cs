using System;
using System.Threading.Tasks;

namespace Board.Common.Events
{
    public interface IEventStore
    {
        Task<TAggregate> LoadAggregateAsync<TAggregate>(Guid id) where TAggregate : AggregateRoot, new();
        Task SaveAggregateAsync<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot;
    }
}