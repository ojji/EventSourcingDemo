using System;
using Board.Common.Events;

namespace Board.Api.Domain.Events
{
    public class ProjectTypeChanged : IDomainEvent
    {
        public Guid EventId { get; }
        public Guid ProjectId { get; }
        public ProjectType OldProjectType { get; }
        public ProjectType NewProjectType { get; }

        public ProjectTypeChanged(Guid projectId, ProjectType oldProjectType, ProjectType newProjectType)
        {
            EventId = Guid.NewGuid();
            ProjectId = projectId;
            OldProjectType = oldProjectType;
            NewProjectType = newProjectType;
        }
    }
}