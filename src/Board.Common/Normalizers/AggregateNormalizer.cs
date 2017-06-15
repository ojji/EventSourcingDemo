using System;
using System.Reflection;
using Board.Common.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace Board.Common.Normalizers
{
    public abstract class AggregateNormalizer<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<AggregateNormalizer<TAggregate>> _logger;
        public long Version { get; protected set; }

        protected AggregateNormalizer(IEventStoreConnection eventStoreConnection, ILogger<AggregateNormalizer<TAggregate>> logger)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
        }

        public void Init()
        {
            _logger.LogDebug("--------------- SUBSCRIBE -----------------\nVersion: {0}\nStreamName: {1}\nCatchupPosition: {2}\n--------------- SUBSCRIBE -----------------", Version, GetStreamName(), GetCatchupPosition());

            _eventStoreConnection.SubscribeToStreamFrom(
                GetStreamName(),
                GetCatchupPosition(),
                CatchUpSubscriptionSettings.Default,
                (subscription, @event) =>
                {
                    _logger.LogDebug("++++++++ NEW EVENT ++++++++\n{0}\n++++++++ NEW EVENT ++++++++", @event.Event.EventType);
                    var methodToInvoke = GetType()
                        .GetMethod("ApplyEvent", new[]
                        {
                            Type.GetType(@event.Event.EventType)
                        });
                    if (methodToInvoke == null)
                    {
                        _logger.LogError($"The view normalizer doesnt know how to handle this event: {@event.Event.EventType}");
                        throw new InvalidOperationException($"The view normalizer doesnt know how to handle this event: {@event.Event.EventType}");
                    }

                    Version++; // bump up read model version

                    methodToInvoke.Invoke(this, new object[]
                    {
                        @event.Event.ToDomainEvent()
                    });
                });
        }

        private long? GetCatchupPosition()
        {
            if (Version == 0)
            {
                return null;
            }
            return Version - 1;
        }

        private string GetStreamName()
        {
            return $"$ce-{typeof(TAggregate).FullName}";
        }
    }
}
