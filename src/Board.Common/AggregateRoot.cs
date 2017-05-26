using System;
using System.Collections.Generic;
using System.Reflection;
using Board.Common.Events;

namespace Board.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<string> _violatedRules = new List<string>();
        private readonly List<IDomainEvent> _uncommitedEvents = new List<IDomainEvent>();

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }
        public string[] ViolatedRules => _violatedRules.ToArray();
        public bool IsValid => _violatedRules.Count == 0;

        public List<IDomainEvent> GetUncommitedEvents()
        {
            return _uncommitedEvents;
        }

        public void ApplyEvent<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            var methodToInvoke = GetType()
                .GetMethod("ApplyEvent", new[] { @event.GetType() });
            
            if (methodToInvoke == null) { throw new InvalidOperationException($"The aggregate root does not know how to apply this event type: <{@event.GetType()}>");}
            methodToInvoke.Invoke(this, new Object[] { @event });
            Version++;
        }

        protected void Publish(IDomainEvent @event)
        {
            _uncommitedEvents.Add(@event);
            Version++;
        }

        public void MarkAsCommited()
        {
            _uncommitedEvents.Clear();
        }

        protected void AddViolatedRule(string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                _violatedRules.Add(reason);
            }
        }
    }
}