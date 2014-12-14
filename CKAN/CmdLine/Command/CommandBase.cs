using log4net;
using log4net.Core;
using ManyConsole;

namespace CKAN.CmdLine.Command
{
    public abstract class CommandBase : ConsoleCommand
    {
        protected IUser User { get; set; }

        protected CommandBase(IUser user)
        {
            User = user;
            SkipsCommandSummaryBeforeRunning();
            HasOption("v|verbose", "Show more of what's going on when running.",
                s => LogManager.GetRepository().Threshold = Level.Verbose);
            HasOption("d|debug", "Show debugging level messages. Implies verbose.",
                s => LogManager.GetRepository().Threshold = Level.Debug);
        }
    }
}