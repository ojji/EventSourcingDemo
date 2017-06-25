using System;

namespace Board.Api.Domain.Repositories
{
    public class ProjectKeyConfiguration
    {
        public string SetKey { get; set; }
        public string VersionKey { get; set; }
        public string ProjectAbbreviationIndexKey { get; set; }
        public string ProjectNameIndexKey { get; set; }

        public string GetHashKey(Guid projectId)
        {
            return $"{SetKey}:{projectId}";
        }

        public static ProjectKeyConfiguration Default = new ProjectKeyConfiguration
        {
            SetKey = "projects",
            VersionKey = "projects:version",
            ProjectNameIndexKey = "projects:name:index",
            ProjectAbbreviationIndexKey = "projects:abbr:index"
        };
    }
}