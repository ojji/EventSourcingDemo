using System;
using Board.Api.Domain.Events;
using Board.Common;
using Board.Common.Events;

namespace Board.Api.Domain
{
    public class Project : AggregateRoot, 
        IDomainEventHandler<ProjectCreated>,
        IDomainEventHandler<ProjectDescriptionChanged>,
        IDomainEventHandler<ProjectTypeChanged>
    {
        public string Name { get; private set; }
        public string ProjectAbbreviation { get; private set; }
        public ProjectType ProjectType { get; private set; }
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }

        public void Create(string projectName, string projectAbbreviation, string description, ProjectType projectType)
        {
            if (Version != 0)
            {
                AddViolatedRule("The project was already created.");
            }
            if (string.IsNullOrWhiteSpace(projectName))
            {
                AddViolatedRule("The project name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(projectAbbreviation))
            {
                AddViolatedRule("The project abbreviation cannot be empty.");
            }
            if (!IsValid)
            {
                return;
            }
            Id = Guid.NewGuid();
            Publish(new ProjectCreated(Id, projectName, projectAbbreviation, description, projectType));
        }
        
        public void ChangeDescription(string newDescription)
        {
            if (Description != newDescription)
            {
                Publish(new ProjectDescriptionChanged(Id, Description, newDescription));
            }
        }

        public void ChangeProjectType(ProjectType newProjectType)
        {
            // there could be a rule like if there are already items/sprints/whatevers on the board, the project type cannot be changed anymore
            if (!ProjectType.Equals(newProjectType))
            {
                Publish(new ProjectTypeChanged(Id, ProjectType, newProjectType));
            }
        }

        #region Event handlers
        public void ApplyEvent(ProjectCreated @event)
        {
            Id = @event.ProjectId;
            Name = @event.ProjectName;
            ProjectAbbreviation = @event.ProjectAbbreviation;
            Description = @event.Description;
            ProjectType = @event.ProjectType;
            IsCompleted = false;
        }

        public void ApplyEvent(ProjectDescriptionChanged @event)
        {
            Description = @event.NewDescription;
        }

        public void ApplyEvent(ProjectTypeChanged @event)
        {
            ProjectType = @event.NewProjectType;
        }

        #endregion
    }

    public enum ProjectType
    {
        Scrum,
        Kanban
    }

    public static class ProjectExtensions
    {
        public static ProjectType ToProjectTypeEnum(this string projectTypeString)
        {
            ProjectType projectType;
            if (!Enum.TryParse(projectTypeString, true, out projectType))
            {
                throw new ArgumentOutOfRangeException(nameof(projectTypeString));
            }
            return projectType;
        }

        public static string ToProjectTypeString(this ProjectType projectTypeEnum)
        {
            return projectTypeEnum.ToString().ToLowerInvariant();
        }
    }
}
