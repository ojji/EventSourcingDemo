namespace Board.Api.Model
{
    public class CreateProjectDto
    {
        public string ProjectName { get; }
        public string Description { get; }
        public string ProjectType { get; }
        public string ProjectAbbreviation { get; }

        public CreateProjectDto(string projectName, string description, string projectAbbreviation, string projectType)
        {
            ProjectName = projectName;
            Description = description;
            ProjectType = projectType;
            ProjectAbbreviation = projectAbbreviation;
        }
    }
}