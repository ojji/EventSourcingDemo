using System;
using System.Collections.Generic;
using System.Linq;
using Board.Api.Domain.ReadModels;
using Board.Common.Utils;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Board.Api.Domain.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly ILogger<ProjectRepository> _logger;
        private readonly ProjectKeyConfiguration _redisKeys;

        public ProjectRepository(IConnectionMultiplexer redisConnection, ILogger<ProjectRepository> logger, ProjectKeyConfiguration redisKeys)
        {
            _redisConnection = redisConnection;
            _logger = logger;
            _redisKeys = redisKeys;
        }

        public IEnumerable<ProjectReadModel> GetAll()
        {
            var db = _redisConnection.GetDatabase();
            foreach (var projectId in db.SetMembers(_redisKeys.SetKey))
            {
                yield return GetProjectById(Guid.Parse(projectId));
            }
        }

        public ProjectReadModel GetProjectById(Guid id)
        {
            var db = _redisConnection.GetDatabase();
            if (db.KeyExists(_redisKeys.GetHashKey(id)))
            {
                return db.HashGetAll(_redisKeys.GetHashKey(id)).FromRedis<ProjectReadModel>();
            }
            return null;
        }

        public ProjectReadModel GetProjectByName(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return null;
            }

            var db = _redisConnection.GetDatabase();
            var redisSearchTerm = projectName.ToLowerInvariant();
            var projectId = db.SortedSetRangeByValue(
                _redisKeys.ProjectNameIndexKey, 
                redisSearchTerm,
                $"{redisSearchTerm}\xff")
                .Where(prj => string.Equals(GetOriginalValue(RemoveGuidFromString(prj)),
                        projectName, StringComparison.OrdinalIgnoreCase))
                .Select(prj => GetGuidFromString(prj)).FirstOrDefault();

            return projectId == null ? null : GetProjectById(Guid.Parse(projectId));
        }

        public ProjectReadModel GetProjectByAbbreviation(string projectAbbreviation)
        {
            if (string.IsNullOrWhiteSpace(projectAbbreviation))
            {
                return null;
            }
            var db = _redisConnection.GetDatabase();
            var searchTerm = projectAbbreviation.ToLowerInvariant();
            var projectId = 
                db.SortedSetRangeByValue(_redisKeys.ProjectAbbreviationIndexKey, searchTerm, $"{searchTerm}\xff")
                .Where(prj => string.Equals(GetOriginalValue(RemoveGuidFromString(prj)),
                        projectAbbreviation, StringComparison.OrdinalIgnoreCase))
                .Select(prj => GetGuidFromString(prj)).FirstOrDefault();

            return projectId == null ? null : GetProjectById(Guid.Parse(projectId));
        }

        public long GetCurrentVersion()
        {
            var redisDatabase = _redisConnection.GetDatabase();
            redisDatabase.StringSet(_redisKeys.VersionKey, 0, null, When.NotExists);
            return (long)redisDatabase.StringGet(_redisKeys.VersionKey);
        }

        public void Save(ProjectReadModel readModel, bool justCreated = false)
        {
            var redisDatabase = _redisConnection.GetDatabase();

            var transaction = redisDatabase.CreateTransaction();
            transaction.StringIncrementAsync(_redisKeys.VersionKey);
            transaction.HashSetAsync(_redisKeys.GetHashKey(readModel.ProjectId), readModel.ToRedis());
            if (justCreated)
            {
                // guid set
                transaction.SetAddAsync(_redisKeys.SetKey, readModel.ProjectId.ToString());
                // name index
                transaction.SortedSetAddAsync(_redisKeys.ProjectNameIndexKey, $"{readModel.ProjectName.ToLowerInvariant()}:{readModel.ProjectName}:{readModel.ProjectId}", 0);
                // abbr index
                transaction.SortedSetAddAsync(_redisKeys.ProjectAbbreviationIndexKey, $"{readModel.ProjectAbbreviation.ToLowerInvariant()}:{readModel.ProjectAbbreviation}:{readModel.ProjectId}", 0);
            }

            if (!transaction.Execute())
            {
                _logger.LogError("Could not save the readmodel to the database.");
            }
        }
        
        private string GetOriginalValue(string valueWithLowerInvariant)
        {
            // the string should be in a "xxx:XXX" format and we have to return the second half
            return valueWithLowerInvariant.Substring(valueWithLowerInvariant.Length / 2 + 1);
        }

        private string RemoveGuidFromString(string stringWithGuid)
        {
            return stringWithGuid.Remove(stringWithGuid.Length - ":00000000-0000-0000-0000-000000000000".Length);
        }

        private string GetGuidFromString(string stringWithGuid)
        {
            return stringWithGuid.Substring(stringWithGuid.Length - "00000000-0000-0000-0000-000000000000".Length);
        }
    }
}