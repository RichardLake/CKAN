// Reference CKAN client
// Paul '@pjf' Fenwick
//
// License: CC-BY 4.0, LGPL, or MIT (your choice)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CKAN.CmdLine.Command;
using CKAN.CmdLine.Command.KSP;
using log4net;
using log4net.Config;
using log4net.Core;
using ManyConsole;
using List = CKAN.CmdLine.Command.KSP.List;

namespace CKAN.CmdLine
{
    internal class MainClass
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MainClass));

        /*
         * When the STAThread is applied, it changes the apartment state of the current thread to be single threaded. 
         * Without getting into a huge discussion about COM and threading,
         * this attribute ensures the communication mechanism between the current thread an
         * other threads that may want to talk to it via COM.  When you're using Windows Forms,
         * depending on the feature you're using, it may be using COM interop in order to communicate with
         * operating system components.  Good examples of this are the Clipboard and the File Dialogs. 
         */

        [STAThread]
        public static int Main(string[] args)
        {
            BasicConfigurator.Configure();
            LogManager.GetRepository().Threshold = Level.Warn;
            Log.Debug("CKAN started");

            // If we're starting with no options then invoke the GUI instead.

            if (args.Length == 0)
            {
                GUI.Main();
                return Exit.OK;
            }

            IUser user = new ConsoleUser();


            var commands = new List<CommandBase>
            {
                new Available(user),
                new Clean(user),
                new Command.GUI(user),
                new Install(user),
                new CommandHolder(user, "ksp", "Manage KSP installs", new CommandBase[]
                {
                    new Add(user),
                    new Default(user),
                    new List(user),
                    new Rename(user),
                    new Forget(user)
                }),
                new Command.List(user),
                new Remove(user),
                new CommandHolder(user, "repair", "Attemp various automatic repairs", new CommandBase[]
                {
                    new Command.Repair.Registry(user)
                }),
                new Scan(user),
                new Show(user),
                new Update(user),
                new Upgrade(user),
                new Command.Version(user)
            };

            if (args.Length > 1)
            {
                var command_holders =
                    commands.Where(command => command is CommandHolder).Select(command => command.Command);
                var second_to_last = args[args.Length - 2];
                //The help for command holders needs to be altered a bit. 
                // "ckan ksp help" is useless while "ckan help ksp" displays what we want
                var last = args[args.Length - 1];
                if (second_to_last.Equals("help") && command_holders.Contains(last))
                {
                    args[args.Length - 1] = second_to_last;
                    args[args.Length - 2] = last;
                }
            }

            return ConsoleCommandDispatcher.DispatchCommand(commands, args, new FakeTextWriter(user), true);
        }

        //It is this or pass in the console instead of using the IUser. 
        internal class FakeTextWriter : TextWriter
        {
            private IUser User { get; set; }
            private readonly List<string> message_buffer = new List<string>();

            public FakeTextWriter(IUser user)
            {
                User = user;
            }

            public override void Write(char[] buffer, int index, int count)
            {
                var message = new String(buffer.Skip(index).Take(count).ToArray());
                message_buffer.Add(message);
                if (!message.Contains("\n")) return;
                User.RaiseMessage(String.Join(String.Empty, message_buffer).TrimEnd('\n'));
                message_buffer.Clear();
            }


            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }
        }
    }
}