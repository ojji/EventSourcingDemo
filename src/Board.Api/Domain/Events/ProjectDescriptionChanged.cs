using System;

namespace Board.Api.Domain.Events
{
    public class ProjectDescriptionChanged : IDomainEvent
    {
        public Guid EventId { get; }
        public string NewDescription { get; }

        public ProjectDescriptionChanged(string newDescription)
        {
            EventId = Guid.NewGuid();
            NewDescription = newDescription;
        }
    }
}