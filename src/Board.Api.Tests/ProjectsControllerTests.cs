using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board.Api.Controllers;
using Board.Api.Domain.Commands;
using Board.Api.Domain.ReadModels;
using Board.Api.Domain.Repositories;
using Board.Api.Domain.Services;
using Board.Api.Model;
using Board.Common.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Board.Api.Tests
{
    public class ProjectsControllerTests
    {
        public static readonly List<ProjectReadModel> MockedProjects =
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
                    ProjectName = "Project2",
                    Description = "Project2 desc",
                    ProjectAbbreviation = "PRJ2",
                    IsCompleted = false,
                    ProjectType = "kanban"
                }
            };

        public class GetProjects
        {
            [Fact]
            public void Should_return_every_project_as_a_json()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr => pr.GetAll()).Returns(MockedProjects);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<JsonResult>(subject.GetProjects());
                var projects = ((IEnumerable<object>)result.Value).ToList();
                Assert.Equal(2, projects.Count);
            }
        }

        public class GetProjectId
        {
            [Fact]
            public void When_the_id_is_invalid_it_should_return_not_found_statuscode()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr => pr.GetProjectById(It.IsAny<Guid>())).Returns((ProjectReadModel)null);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<NotFoundResult>(subject.GetProjectById(Guid.Empty));
                Assert.Equal(404, result.StatusCode);
            }

            [Fact]
            public void When_the_id_is_valid_it_should_return_the_project_json()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                    pr.GetProjectById(Guid.Parse("11111111-1111-1111-1111-111111111111")))
                    .Returns(MockedProjects[0]);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result =
                    Assert.IsType<JsonResult>(subject.GetProjectById(Guid.Parse("11111111-1111-1111-1111-111111111111")));

                var content = Assert.IsType<ProjectReadModel>(result.Value);
                Assert.Equal("Project1", content.ProjectName);
            }
        }

        public class CreateProject
        {
            [Fact]
            public async Task When_the_new_project_is_null_it_should_return_a_BadRequest()
            {
                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    Mock.Of<IProjectRepository>(),
                    Mock.Of<ILogger<ProjectsController>>());

                Assert.IsType<BadRequestResult>(await subject.CreateProject(null));
            }

            [Fact]
            public async Task When_the_project_breaks_domain_rules_it_should_return_a_BadRequest()
            {
                var projectManagerService = new Mock<IProjectManagerService>();
                projectManagerService.Setup(pms => pms.Handle(It.IsAny<CreateProjectCommand>()))
                    .Returns(Task.FromResult(CommandResult.Fail("business rule broken")));

                var subject = new ProjectsController(
                    projectManagerService.Object,
                    Mock.Of<IProjectRepository>(),
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<BadRequestObjectResult>(await subject.CreateProject(new CreateProjectDto("whatever", "description", "we", "kanban")));
                Assert.Equal("business rule broken", ((string[])result.Value)[0]);
            }

            [Fact]
            public async Task When_the_project_is_valid_is_should_return_a_CreatedResult_with_the_new_project_id()
            {
                var projectManagerService = new Mock<IProjectManagerService>();
                projectManagerService.Setup(pms => pms.Handle(It.IsAny<CreateProjectCommand>()))
                    .Returns(Task.FromResult(CommandResult.Success(Guid.Parse("11111111-1111-1111-1111-111111111111"))));
                
                var subject = new ProjectsController(
                    projectManagerService.Object,
                    Mock.Of<IProjectRepository>(),
                    Mock.Of<ILogger<ProjectsController>>());

                subject.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                };

                subject.ControllerContext.HttpContext.Request.Scheme = "http";
                subject.ControllerContext.HttpContext.Request.Host = HostString.FromUriComponent("localhost:5000");
                subject.ControllerContext.HttpContext.Request.Path = "/";

                var result =
                    Assert.IsType<CreatedResult>(
                        await subject.CreateProject(new CreateProjectDto("Valid project", "Desc", "VP", "Kanban")));

                Assert.EndsWith("/11111111-1111-1111-1111-111111111111", result.Location);
            }

            [Fact]
            public async Task When_the_project_name_is_already_taken_it_should_return_a_BadRequest()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr => pr.GetProjectByName("Project1"))
                    .Returns(new ProjectReadModel()
                    {
                        ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                    });


                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());


                var result = Assert.IsType<BadRequestObjectResult>(
                    await subject.CreateProject(
                        new CreateProjectDto("Project1", "Desc", "VP", "Kanban")
                        ));
                Assert.Contains("project name", (string)result.Value);
            }

            [Fact]
            public async Task When_the_project_abbreviation_is_already_taken_it_should_return_a_BadRequest()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr => pr.GetProjectByAbbreviation("prj-1"))
                    .Returns(new ProjectReadModel
                    {
                        ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                    });

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<BadRequestObjectResult>(
                    await subject.CreateProject(
                        new CreateProjectDto("Project1", "Desc", "prj-1", "Kanban")
                        ));
                Assert.Contains("project abbreviation", (string)result.Value);
            }
        }

        public class CheckProjectName
        {
            [Theory]
            [InlineData((string)null)]
            [InlineData("")]
            [InlineData(" ")]
            public void When_the_project_name_is_null_or_empty_it_should_return_BadRequest(string projectName)
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByName(It.IsAny<string>()))
                    .Returns((ProjectReadModel)null);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<BadRequestObjectResult>(subject.CheckProjectName(projectName));
                Assert.Contains("project name", (string)result.Value);
            }

            [Fact]
            public void When_the_project_name_is_not_taken_it_should_return_available()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByName(It.IsAny<string>()))
                    .Returns((ProjectReadModel)null);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var jsonResult = Assert.IsType<JsonResult>(subject.CheckProjectName("available project name"));
                var result = Assert.IsType<ProjectNameAvailableDto>(jsonResult.Value);
                Assert.Equal("available project name", result.ProjectName);
                Assert.True(result.IsAvailable);
            }

            [Fact]
            public void When_the_project_name_is_taken_it_should_return_unavailable()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByName("Taken project name"))
                    .Returns(new ProjectReadModel
                    {
                        ProjectId = Guid.Empty,
                        ProjectName = "Taken project name"
                    });

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var jsonResult = Assert.IsType<JsonResult>(subject.CheckProjectName("Taken project name"));
                var result = Assert.IsType<ProjectNameAvailableDto>(jsonResult.Value);
                Assert.Equal("Taken project name", result.ProjectName);
                Assert.False(result.IsAvailable);
            }
        }

        public class CheckProjectAbbreviation
        {
            [Theory]
            [InlineData((string)null)]
            [InlineData("")]
            [InlineData(" ")]
            public void When_the_project_abbreviation_is_null_or_empty_it_should_return_BadRequest(string projectAbbreviation)
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByAbbreviation(It.IsAny<string>()))
                    .Returns((ProjectReadModel)null);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var result = Assert.IsType<BadRequestObjectResult>(subject.CheckProjectAbbreviation(projectAbbreviation));
                Assert.Contains("project abbreviation", (string)result.Value);
            }

            [Fact]
            public void When_the_project_abbreviation_is_not_taken_it_should_return_available()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByAbbreviation("PRJ1"))
                    .Returns((ProjectReadModel)null);

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var jsonResult = Assert.IsType<JsonResult>(subject.CheckProjectAbbreviation("PRJ1"));
                var result = Assert.IsType<ProjectAbbreviationAvailableDto>(jsonResult.Value);
                Assert.Equal("PRJ1", result.ProjectAbbreviation);
                Assert.True(result.IsAvailable);
            }

            [Fact]
            public void When_the_project_abbreviation_is_taken_it_should_return_unavailable()
            {
                var projectRepository = new Mock<IProjectRepository>();
                projectRepository.Setup(pr =>
                        pr.GetProjectByAbbreviation("TAKENPRJ1"))
                    .Returns(new ProjectReadModel
                    {
                        ProjectId = Guid.Empty,
                        ProjectAbbreviation = "TakenPRJ1"
                    });

                var subject = new ProjectsController(
                    Mock.Of<IProjectManagerService>(),
                    projectRepository.Object,
                    Mock.Of<ILogger<ProjectsController>>());

                var jsonResult = Assert.IsType<JsonResult>(subject.CheckProjectAbbreviation("TAKENPRJ1"));
                var result = Assert.IsType<ProjectAbbreviationAvailableDto>(jsonResult.Value);
                Assert.Equal("TAKENPRJ1", result.ProjectAbbreviation);
                Assert.False(result.IsAvailable);
            }
        }
    }
}