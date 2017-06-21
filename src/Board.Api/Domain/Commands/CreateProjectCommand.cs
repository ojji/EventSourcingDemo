using Board.Common.Commands;

namespace Board.Api.Domain.Commands
{
    public class CreateProjectCommand : ICommand
    {
        public string ProjectName { get; }
        public string ProjectAbbreviation { get; }
        public string Description { get; }
        public ProjectType ProjectType { get; }

        public CreateProjectCommand(string projectName, string projectAbbreviation, string description, ProjectType projectType)
        {
            ProjectName = projectName;
            ProjectAbbreviation = projectAbbreviation;
            Description = description;
            ProjectType = projectType;
        }
    }
}