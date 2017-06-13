using System;
using System.Linq;
using System.Threading.Tasks;
using Board.Api.Domain;
using Board.Api.Domain.Commands;
using Board.Api.Domain.Repositories;
using Board.Api.Domain.Services;
using Board.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Board.Api.Controllers
{
    [Route("/api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectManagerService _projectManagerService;
        private readonly ProjectRepository _projectRepository;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectManagerService projectManagerService,
            ProjectRepository projectRepository, 
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
            var commandResult = await _projectManagerService.Handle(
                new CreateProjectCommand(
                    newProject.ProjectName,
                    newProject.Description,
                    newProject.ProjectType.ToProjectTypeEnum()
                ));

            if (commandResult.IsSuccessful)
            {
                return Ok();
            }

            _logger.LogError(commandResult.FailReasons[0]);
            return BadRequest(commandResult.FailReasons);
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