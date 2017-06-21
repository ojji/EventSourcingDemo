namespace Board.Api.Model
{
    public class ChangeProjectDto
    {
        public string Description { get; }
        public string ProjectAbbreviation { get; }
        public string ProjectType { get; }

        public ChangeProjectDto(string description, string projectAbbreviation, string projectType)
        {
            Description = description;
            ProjectAbbreviation = projectAbbreviation;
            ProjectType = projectType;
        }
    }
}