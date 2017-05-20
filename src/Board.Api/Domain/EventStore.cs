using System;
using System.Linq;
using System.Threading.Tasks;
using Board.Api.Domain.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Microsoft.Extensions.Logging;

namespace Board.Api.Domain
{
    public class EventStore : IEventStore
    {
        private readonly ILogger<EventStore> _logger;
        private readonly IEventStoreConnection _eventStoreConnection;

        public EventStore(IEventStoreConnection eventStoreConnection, ILogger<EventStore> logger)
        {
            _logger = logger;
            _eventStoreConnection = eventStoreConnection ?? throw new ArgumentNullException(nameof(eventStoreConnection));
        }

        public async Task<TAggregate> LoadAggregateAsync<TAggregate>(Guid id) where TAggregate : AggregateRoot, new()
        {
            var aggregateRehydrated = new TAggregate();
            long currentPosition = StreamPosition.Start;
            StreamEventsSlice slice;
            string streamName = GetStreamNameForAggregate<TAggregate>(id);
            _logger.LogInformation("Trying to read events from: {0}", streamName);
            do
            {
                slice = await _eventStoreConnection.ReadStreamEventsForwardAsync(
                    streamName,
                    currentPosition, 
                    1, 
                    false);
                
                currentPosition = slice.NextEventNumber;

                aggregateRehydrated.ApplyEvent(slice.Events[0].Event.ToDomainEvent());
            } while (!slice.IsEndOfStream);
            _logger.LogInformation("Aggregate {0} ({1}) loaded.", aggregateRehydrated.Id, aggregateRehydrated.GetType().FullName);

            return aggregateRehydrated;
        }

        public async Task SaveAggregateAsync<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot
        {
            var eventsToSave = aggregate.GetUncommitedEvents();
            var uncommitedEventCount = eventsToSave.Count;

            _logger.LogInformation("EventStore SaveAggregate on {0} called, uncommited events: {1}", aggregate.Id, uncommitedEventCount);
            if (uncommitedEventCount == 0)
            {
                return;
            }

            try
            {
                await _eventStoreConnection.AppendToStreamAsync(GetStreamNameForAggregate<TAggregate>(aggregate.Id), 
                    aggregate.Version - (uncommitedEventCount + 1), // event expectedversion starts at -1, version from 0
                    eventsToSave.Select(e => e.ToEventData()));

                aggregate.MarkAsCommited();
            }
            catch (WrongExpectedVersionException versionException)
            {
                throw;
            }
        }

        private string GetStreamNameForAggregate<TAggregate>(Guid id) where TAggregate : AggregateRoot
        {
            return $"{typeof(TAggregate).FullName}-{id}";
        }
    }
}