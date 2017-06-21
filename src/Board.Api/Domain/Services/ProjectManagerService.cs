using System;
using System.Threading.Tasks;
using Board.Api.Domain.Commands;
using Board.Common.Commands;
using Board.Common.Events;

namespace Board.Api.Domain.Services
{
    public class ProjectManagerService : IProjectManagerService
    {
        private readonly IEventStore _eventStore;

        public ProjectManagerService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<CommandResult> Handle(CreateProjectCommand command)
        {
            var project = new Project();
            project.Create(command.ProjectName, command.ProjectAbbreviation, command.Description, command.ProjectType);

            try
            {
                if (project.IsValid)
                {
                    await _eventStore.SaveAggregateAsync(project);
                    return CommandResult.Success(project.Id);
                }
                return CommandResult.Fail(project.ViolatedRules);
            }
            catch (Exception ex)
            {
                return CommandResult.Fail(ex.Message);
            }
        }

        public async Task<CommandResult> Handle(UpdateProjectCommand command)
        {
            try
            {
                var project = await _eventStore.LoadAggregateAsync<Project>(command.ProjectId);
                if (project.Description != command.Description)
                {
                    project.ChangeDescription(command.Description);
                }
                if (project.ProjectType != command.ProjectType)
                {
                    project.ChangeProjectType(command.ProjectType);
                }

                if (project.IsValid)
                {
                    await _eventStore.SaveAggregateAsync(project);
                    return CommandResult.Success(project.Id);
                }

                return CommandResult.Fail(project.ViolatedRules);
            }
            catch (Exception ex)
            {
                return CommandResult.Fail(ex.Message);
            }
        }
    }
}