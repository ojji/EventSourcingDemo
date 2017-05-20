using System;

namespace Board.Api.Model
{
    public class ViewProjectDto
    {
        public Guid ProjectId { get; }
        public string ProjectName { get; }
        public string Description { get; }
        public string ProjectType { get; }

        public ViewProjectDto(Guid projectId, string projectName, string description, string projectType)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            Description = description;
            ProjectType = projectType;
        }
    }
}