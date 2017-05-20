using Board.Api.Domain.Commands;

namespace Board.Api.Domain.Services
{
    public interface IProjectManagerService : ICommandHandler<CreateProjectCommand>, 
        ICommandHandler<UpdateProjectCommand>
    {
        
    }
}
