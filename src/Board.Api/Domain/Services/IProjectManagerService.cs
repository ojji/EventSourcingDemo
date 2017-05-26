using Board.Api.Domain.Commands;
using Board.Common.Commands;

namespace Board.Api.Domain.Services
{
    public interface IProjectManagerService : ICommandHandler<CreateProjectCommand>,
        ICommandHandler<UpdateProjectCommand>
    {
        
    }
}