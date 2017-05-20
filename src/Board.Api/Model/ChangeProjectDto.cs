namespace Board.Api.Model
{
    public class ChangeProjectDto
    {
        public string Description { get; }
        public string ProjectType { get; }

        public ChangeProjectDto(string description, string projectType)
        {
            Description = description;
            ProjectType = projectType;
        }
    }
}