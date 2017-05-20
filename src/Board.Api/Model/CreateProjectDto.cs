namespace Board.Api.Model
{
    public class CreateProjectDto
    {
        public string ProjectName { get; }
        public string Description { get; }
        public string ProjectType { get; }

        public CreateProjectDto(string projectName, string description, string projectType)
        {
            ProjectName = projectName;
            Description = description;
            ProjectType = projectType;
        }
    }
}