using Board.Api.Domain.Events;
using Board.Api.Domain.ReadModels;
using Board.Api.Domain.Repositories;
using Board.Common.Events;
using Board.Common.Normalizers;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace Board.Api.Domain.Normalizers
{
    public class RedisProjectViewNormalizer :
        AggregateNormalizer<Project>,
        IDomainEventHandler<ProjectCreated>,
        IDomainEventHandler<ProjectDescriptionChanged>,
        IDomainEventHandler<ProjectTypeChanged>
    {
        private readonly ILogger<RedisProjectViewNormalizer> _logger;
        private readonly ProjectRepository _projectRepository;

        public RedisProjectViewNormalizer(IEventStoreConnection eventStoreConnection, ILogger<RedisProjectViewNormalizer> logger, ProjectRepository projectRepository) : base(eventStoreConnection, (ILogger<AggregateNormalizer<Project>>) logger) 
        {
            _logger = logger;
            _projectRepository = projectRepository;
            Version = _projectRepository.GetCurrentVersion();
        }
        
        #region Domain event handlers
        public void ApplyEvent(ProjectCreated @event)
        {
            var readModel = new ProjectReadModel
            {
                ProjectId = @event.ProjectId,
                ProjectName = @event.ProjectName,
                Description = @event.Description,
                ProjectType = @event.ProjectType.ToProjectTypeString(),
                IsCompleted = false
            };

            _projectRepository.Save(readModel, justCreated: true);
        }

        public void ApplyEvent(ProjectDescriptionChanged @event)
        {
            var readModel = _projectRepository.GetProjectById(@event.ProjectId);
            readModel.Description = @event.NewDescription;
            _projectRepository.Save(readModel);
        }

        public void ApplyEvent(ProjectTypeChanged @event)
        {
            var readModel = _projectRepository.GetProjectById(@event.ProjectId);
            readModel.ProjectType = @event.NewProjectType.ToProjectTypeString();
            _projectRepository.Save(readModel);
        } 
        #endregion
    }
}