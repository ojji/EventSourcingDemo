using System;
using System.Linq;
using System.Threading.Tasks;
using Board.Api.Domain;
using Board.Api.Domain.Commands;
using Board.Api.Domain.Repositories;
using Board.Api.Domain.Services;
using Board.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Board.Api.Controllers
{
    [Route("/api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectManagerService _projectManagerService;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectManagerService projectManagerService,
            IProjectRepository projectRepository, 
            ILogger<ProjectsController> logger)
        {
            _projectManagerService = projectManagerService;
            _projectRepository = projectRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            return Json(_projectRepository.GetAll().Select(p => new { p.ProjectId, p.ProjectName, p.ProjectType }));
        }

        [HttpGet("{id}")]
        public IActionResult GetProjectById(Guid id)
        {
            var project = _projectRepository.GetProjectById(id);
            if (project != null)
            {
                return Json(project);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody]CreateProjectDto newProject)
        {
            if (newProject == null)
            {
                return BadRequest();
            }

            // We have to check whether the project name and the project abbreviation is unique.
            // NOTE: there is a race condition window with two commands with the same values,
            // but the chances of happening is too low. However if this becomes a problem in the future,
            // we could create an event handler to the create command which checks if there is a created
            // project with the name or abbreviation already and in that case could issue compensating commands.
            if (_projectRepository.GetProjectByName(newProject.ProjectName) != null)
            {
                return BadRequest("The project name is already taken");
            }

            if (_projectRepository.GetProjectByAbbreviation(newProject.ProjectAbbreviation) != null)
            {
                return BadRequest("The project abbreviation is already taken");
            }

            var commandResult = await _projectManagerService.Handle(
                new CreateProjectCommand(
                    newProject.ProjectName,
                    newProject.ProjectAbbreviation,
                    newProject.Description,
                    newProject.ProjectType.ToProjectTypeEnum()
                ));

            if (commandResult.IsSuccessful)
            {
                var location = GetNewResourceLocation(Request, commandResult.AffectedAggregate);
                return Created(location, null);
            }

            _logger.LogError(commandResult.FailReasons[0]);
            return BadRequest(commandResult.FailReasons);
        }

        private Uri GetNewResourceLocation(HttpRequest request, Guid newResourceId)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.Add(new PathString($"/{newResourceId}"))
            };
            if (request.Host.Port.HasValue)
            {
                builder.Port = request.Host.Port.Value;
            }
            return builder.Uri;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody]ChangeProjectDto updatedProject)
        {
            var commandResult =  await _projectManagerService.Handle(
                new UpdateProjectCommand(
                    id, 
                    updatedProject.Description,
                    updatedProject.ProjectType.ToProjectTypeEnum()));

            if (commandResult.IsSuccessful)
            {
                return NoContent();
            }

            _logger.LogError(commandResult.FailReasons[0]);
            return BadRequest(commandResult.FailReasons);
        }
    }
}