using System;
using System.Threading.Tasks;

namespace Board.Common.Commands
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
        public Guid AffectedAggregate { get; }
        public string[] FailReasons { get; }
        public bool IsSuccessful => FailReasons == null;

        private CommandResult(Guid affectedAggregate)
        {
            AffectedAggregate = affectedAggregate;
        }

        private CommandResult(string[] failReasons)
        {
            FailReasons = failReasons;
        }

        public static CommandResult Success(Guid affectedAggregate)
        {
            return new CommandResult(affectedAggregate);
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
