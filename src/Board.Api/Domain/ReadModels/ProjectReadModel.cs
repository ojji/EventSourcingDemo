using System;

namespace Board.Api.Domain.ReadModels
{
    public class ProjectReadModel
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectType { get; set; }
    }
}