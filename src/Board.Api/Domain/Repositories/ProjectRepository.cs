using System;
using System.Collections.Generic;
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

        public ProjectRepository(IConnectionMultiplexer redisConnection, ILogger<ProjectRepository> logger)
        {
            _redisConnection = redisConnection;
            _logger = logger;
        }

        public IEnumerable<ProjectReadModel> GetAll()
        {
            var db = _redisConnection.GetDatabase();
            foreach (var projectId in db.SetMembers(RedisKeys.SetKey))
            {
                yield return GetProjectById(Guid.Parse(projectId));
            }
        }

        public ProjectReadModel GetProjectById(Guid id)
        {
            var db = _redisConnection.GetDatabase();
            if (db.KeyExists(RedisKeys.GetHashKey(id)))
            {
                return db.HashGetAll(RedisKeys.GetHashKey(id)).FromRedis<ProjectReadModel>();
            }
            return null;
        }

        public long GetCurrentVersion()
        {
            var redisDatabase = _redisConnection.GetDatabase();
            redisDatabase.StringSet(RedisKeys.VersionKey, 0, null, When.NotExists);
            return (long)redisDatabase.StringGet(RedisKeys.VersionKey);
        }

        public void Save(ProjectReadModel readModel, bool justCreated = false)
        {
            var redisDatabase = _redisConnection.GetDatabase();

            var transaction = redisDatabase.CreateTransaction();
            transaction.StringIncrementAsync(RedisKeys.VersionKey);
            transaction.HashSetAsync(RedisKeys.GetHashKey(readModel.ProjectId), readModel.ToRedis());
            if (justCreated)
            {
                transaction.SetAddAsync(RedisKeys.SetKey, readModel.ProjectId.ToString());
            }

            if (!transaction.Execute())
            {
                _logger.LogError("Could not save the readmodel to the database.");
            }
        }

        private static class RedisKeys
        {
            public static string GetHashKey(Guid projectId)
            {
                return $"{SetKey}:{projectId}";
            }

            public const string SetKey = "projects";
            public const string VersionKey = "projects:version";
        }
    }
}