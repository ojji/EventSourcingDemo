using System;

namespace Board.Api.Domain.Events
{
    public class ProjectDescriptionChanged : IDomainEvent
    {
        public Guid EventId { get; }
        public Guid ProjectId { get; }
        public string OldDescription { get; }
        public string NewDescription { get; }

        public ProjectDescriptionChanged(Guid projectId, string oldDescription, string newDescription)
        {
            EventId = Guid.NewGuid();
            ProjectId = projectId;
            OldDescription = oldDescription;
            NewDescription = newDescription;
        }
    }
}