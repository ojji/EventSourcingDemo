﻿using System;
using Board.Common.Events;

namespace Board.Api.Domain.Events
{
    public class ProjectCreated : IDomainEvent
    {
        public Guid EventId { get; }

        public Guid ProjectId { get; }
        public string ProjectName { get; }
        public string Description { get; }
        public ProjectType ProjectType { get; }

        public ProjectCreated(Guid projectId, string projectName, string description, ProjectType projectType)
        {
            EventId = Guid.NewGuid();
            ProjectId = projectId;
            ProjectName = projectName;
            Description = description;
            ProjectType = projectType;
        }
    }
}