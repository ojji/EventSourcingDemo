using System.Collections.Generic;

namespace Board.Api.Model
{
    public class ProjectNameAvailableDto
    {
        public string ProjectName { get; set; }
        public bool IsAvailable { get; set; }
    }
}