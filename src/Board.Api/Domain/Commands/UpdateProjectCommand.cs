using System;
using Board.Common.Commands;

namespace Board.Api.Domain.Commands
{
    public class UpdateProjectCommand : ICommand
    {
        public Guid ProjectId { get; }
        public string Description { get; }
        public ProjectType ProjectType { get; }

        public UpdateProjectCommand(Guid projectId, string description, ProjectType projectType)
        {
            ProjectId = projectId;
            Description = description;
            ProjectType = projectType;
        }
    }
}