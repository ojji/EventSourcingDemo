using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Board.Common.Events
{
    public static class EventConverterExtensions
    {
        public static EventData ToEventData<TDomainEvent>(this TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            var eventBody = JsonConvert.SerializeObject(@event);
            EventData eventData = new EventData(@event.EventId, @event.GetType().FullName, true, Encoding.UTF8.GetBytes(eventBody), null);

            return eventData;
        }

        public static IDomainEvent ToDomainEvent(this RecordedEvent eventData)
        {
            var eventType = Type.GetType(eventData.EventType);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventData.Data), eventType) as IDomainEvent;
        }
    }
}