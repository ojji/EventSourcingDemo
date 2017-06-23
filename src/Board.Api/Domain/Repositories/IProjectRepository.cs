using System;
using System.Collections.Generic;
using Board.Api.Domain.ReadModels;

namespace Board.Api.Domain.Repositories
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectReadModel> GetAll();
        ProjectReadModel GetProjectById(Guid id);
        long GetCurrentVersion();

        void Save(ProjectReadModel readModel, bool justCreated = false);
    }
}