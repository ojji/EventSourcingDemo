using System;
using System.Collections.Generic;
using System.Linq;
using Board.Api.Domain.ReadModels;
using Board.Api.Domain.Repositories;
using Board.Common.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Xunit;

[assembly:CollectionBehavior(DisableTestParallelization = true)]

namespace Board.Api.Tests
{
    public class ProjectRepositoryTests
    {
        public static ProjectKeyConfiguration ProjectRepositoryTestKeys = new ProjectKeyConfiguration
        {
            SetKey = "test:projects",
            VersionKey = "test:projects:version",
            ProjectNameIndexKey = "test:projects:name:index",
            ProjectAbbreviationIndexKey = "test:projects:abbr:index"
        };

        [CollectionDefinition("Project repository tests")]
        public class ProjectRepositoryTestsCollection : ICollectionFixture<ProjectRepositoryFixture>
        {
            
        }
   
        [Collection("Project repository tests")]
        public class GetAll
        {
            private readonly ProjectRepositoryFixture _projectRepositoryFixture;

            public GetAll(ProjectRepositoryFixture projectRepositoryFixture)
            {
                _projectRepositoryFixture = projectRepositoryFixture;
            }

            [Fact]
            public void It_should_return_every_project()
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetAll().ToArray();
                Assert.Equal(ProjectRepositoryFixture.TestProjects.Count, result.Length);
                Assert.Contains(result, project => project.ProjectId.Equals(Guid.Parse("11111111-1111-1111-1111-111111111111")));
            }
        }

        [Collection("Project repository tests")]
        public class GetProjectByAbbreviation
        {
            private readonly ProjectRepositoryFixture _projectRepositoryFixture;

            public GetProjectByAbbreviation(ProjectRepositoryFixture projectRepositoryFixture)
            {
                _projectRepositoryFixture = projectRepositoryFixture;
            }

            [Theory]
            [InlineData((string)null)]
            [InlineData("")]
            [InlineData(" ")]
            public void When_the_project_abbreviation_is_null_or_empty_it_should_return_null(string abbreviation)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectByAbbreviation(abbreviation);
                Assert.Null(result);
            }

            [Theory]
            [InlineData("PR")]
            [InlineData("Something else")]
            public void When_the_project_abbreviation_does_not_exist_it_should_return_null(string abbreviation)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectByAbbreviation(abbreviation);
                Assert.Null(result);
            }

            [Theory]
            [InlineData("PRJ1:", "33333333-3333-3333-3333-333333333333")]
            [InlineData("PRJ1::", "22222222-2222-2222-2222-222222222222")]
            public void When_the_project_abbreviation_ends_with_a_colon_it_should_return_the_project_with_the_correct_abbreviation(string abbreviation, string expectedId)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectByAbbreviation(abbreviation);
                Assert.NotNull(result);
                Assert.Equal(Guid.Parse(expectedId), result.ProjectId);
            }

            [Theory]
            [InlineData("PRJ1")]
            [InlineData("PrJ1")]
            [InlineData("prj1")]
            public void It_should_be_case_insensitive(string abbreviation)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);
                var result = subject.GetProjectByAbbreviation(abbreviation);
                Assert.NotNull(result);
                Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), result.ProjectId);
            }
        }

        [Collection("Project repository tests")]
        public class GetProjectById
        {
            private readonly ProjectRepositoryFixture _projectRepositoryFixture;

            public GetProjectById(ProjectRepositoryFixture projectRepositoryFixture)
            {
                _projectRepositoryFixture = projectRepositoryFixture;
            }

            [Fact]
            public void When_the_project_id_does_not_exist_it_should_return_null()
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectById(Guid.Empty);
                Assert.Null(result);
            }

            [Fact]
            public void It_should_return_the_project_with_the_requested_id()
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectById(Guid.Parse("11111111-1111-1111-1111-111111111111"));
                Assert.NotNull(result);
                Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), result.ProjectId);
                Assert.Equal("Project1", result.ProjectName);
            }
        }

        [Collection("Project repository tests")]
        public class GetProjectByName
        {
            private readonly ProjectRepositoryFixture _projectRepositoryFixture;

            public GetProjectByName(ProjectRepositoryFixture projectRepositoryFixture)
            {
                _projectRepositoryFixture = projectRepositoryFixture;
            }

            [Theory]
            [InlineData((string)null)]
            [InlineData("")]
            [InlineData(" ")]
            public void When_the_project_name_is_null_or_empty_it_should_return_null(string projectName)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectByName(projectName);
                Assert.Null(result);
            }

            [Theory]
            [InlineData("Pro")]
            [InlineData("Something else")]
            public void When_the_project_name_does_not_exist_it_should_return_null(string projectName)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);

                var result = subject.GetProjectByName(projectName);
                Assert.Null(result);
            }

            [Theory]
            [InlineData("Project1:", "33333333-3333-3333-3333-333333333333")]
            [InlineData("Project1::", "22222222-2222-2222-2222-222222222222")]
            public void When_the_project_name_ends_with_a_colon_it_should_return_the_project_with_the_correct_name(string projectName, string expectedId)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);
                var result = subject.GetProjectByName(projectName);
                Assert.NotNull(result);
                Assert.Equal(Guid.Parse(expectedId), result.ProjectId);
            }

            [Theory]
            [InlineData("Project1")]
            [InlineData("project1")]
            [InlineData("proJEct1")]
            public void It_should_be_case_insensitive(string projectName)
            {
                var subject = new ProjectRepository(_projectRepositoryFixture.RedisConnection,
                    Mock.Of<ILogger<ProjectRepository>>(),
                    _projectRepositoryFixture.Configuration);
                var result = subject.GetProjectByName(projectName);
                Assert.NotNull(result);
                Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), result.ProjectId);
            }
        }
    }

    #region Test database setup code
    public class ProjectRepositoryFixture : IDisposable
    {
        public ProjectKeyConfiguration Configuration => ProjectRepositoryTests.ProjectRepositoryTestKeys;

        public static readonly List<ProjectReadModel> TestProjects =
            new List<ProjectReadModel>
            {
                new ProjectReadModel
                {
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ProjectName = "Project1",
                    Description = "Project1 desc",
                    ProjectAbbreviation = "PRJ1",
                    IsCompleted = false,
                    ProjectType = "kanban"
                },
                new ProjectReadModel
                {
                    ProjectId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ProjectName = "Project1::",
                    Description = "Project1 alternative alternatve desc",
                    ProjectAbbreviation = "PRJ1::",
                    IsCompleted = false,
                    ProjectType = "scrum"
                },
                new ProjectReadModel
                {
                    ProjectId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ProjectName = "Project1:",
                    Description = "Project1 alternative desc",
                    ProjectAbbreviation = "PRJ1:",
                    IsCompleted = false,
                    ProjectType = "scrum"
                },
                new ProjectReadModel
                {
                    ProjectId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    ProjectName = "Project2",
                    Description = "Project2 desc",
                    ProjectAbbreviation = "PRJ2",
                    IsCompleted = false,
                    ProjectType = "kanban"
                }
            };

        public ProjectRepositoryFixture()
        {
            RedisMapper.RegisterType<ProjectReadModel>();
            RedisConnection = ConnectionMultiplexer.Connect("localhost");
            DeleteTestDatabase();
            SetupTestDatabase();
        }

        private void SetupTestDatabase()
        {
            ProjectRepository repository = new ProjectRepository(RedisConnection,
                Mock.Of<ILogger<ProjectRepository>>(),
                Configuration);

            foreach (var projectReadModel in TestProjects)
            {
                repository.Save(projectReadModel, justCreated: true);
            }
        }

        private void DeleteTestDatabase()
        {
            var db = RedisConnection.GetDatabase();

            foreach (var projectReadModel in TestProjects)
            {
                db.KeyDelete(Configuration.GetHashKey(projectReadModel.ProjectId));
            }

            db.KeyDelete(Configuration.VersionKey);
            db.KeyDelete(Configuration.SetKey);
            db.KeyDelete(Configuration.ProjectAbbreviationIndexKey);
            db.KeyDelete(Configuration.ProjectNameIndexKey);
        }

        public IConnectionMultiplexer RedisConnection { get; }

        public void Dispose()
        {
            DeleteTestDatabase();
            RedisConnection.Dispose();
        }
    } 
    #endregion
}