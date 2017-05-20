using System;

namespace Board.Api.Domain.Events
{
    public class ProjectTypeChanged : IDomainEvent
    {
        public Guid EventId { get; }
        public ProjectType NewProjectType { get; }

        public ProjectTypeChanged(ProjectType newProjectType)
        {
            EventId = Guid.NewGuid();
            NewProjectType = newProjectType;
        }
    }
}