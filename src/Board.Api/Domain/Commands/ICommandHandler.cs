using System.Threading.Tasks;

namespace Board.Api.Domain.Commands
{
    public interface ICommand
    {
        
    }
    public interface ICommandHandler<in T> where T: ICommand
    {
        Task<CommandResult> Handle(T command);
    }

    public class CommandResult
    {
        public string[] FailReasons { get; }
        public bool IsSuccessful => FailReasons == null;

        private CommandResult()
        {
        }

        private CommandResult(string[] failReasons)
        {
            FailReasons = failReasons;
        }

        public static CommandResult Success()
        {
            return new CommandResult();
        }

        public static CommandResult Fail(string reason)
        {
            return new CommandResult(new[]{reason});
        }

        public static CommandResult Fail(string[] reasons)
        {
            return new CommandResult(reasons);
        }
    }
}
