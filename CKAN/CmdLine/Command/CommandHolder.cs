using System.Collections.Generic;
using System.Linq;
using ManyConsole;
using ManyConsole.Internal;

namespace CKAN.CmdLine.Command
{
    internal class CommandHolder : CommandBase
    {
        private readonly Dictionary<string, CommandBase> commands;


        public CommandHolder(IUser user,
            string name,
            string description,
            IEnumerable<CommandBase> commands)
            : base(user)
        {
            SkipsCommandSummaryBeforeRunning();
            IsCommand(name, description);
            AllowsAnyAdditionalArguments();
            this.commands = commands.ToDictionary(command => command.Command);
        }


        public override int Run(string[] remaining_arguments)
        {
            var text_writer = new MainClass.FakeTextWriter(User);
            if (remaining_arguments.Length == 0)
            {
                ConsoleHelp.ShowSummaryOfCommands(commands.Values, text_writer);
                return 0;
            }
            var command_name = remaining_arguments[0];
            if (command_name.Equals("help"))
            {
                if (remaining_arguments.Length > 1)
                {
                    CommandBase value;
                    if (commands.TryGetValue(remaining_arguments[1], out value))
                    {
                        ConsoleHelp.ShowCommandHelp(value, text_writer, true);
                        return Exit.BADOPT;
                    }
                }
                else
                {
                    ConsoleHelp.ShowSummaryOfCommands(commands.Values, text_writer);
                    return Exit.BADOPT;
                }
            }
            CommandBase command;
            if (commands.TryGetValue(command_name, out command))
            {
                ConsoleCommandDispatcher.DispatchCommand(commands.Values, remaining_arguments, text_writer, true);
            }
            else
            {
                throw new ConsoleHelpAsException(string.Format("Unknown command \"{0} {1}\"", Command, command_name));
            }

            return 0;
        }
    }
}