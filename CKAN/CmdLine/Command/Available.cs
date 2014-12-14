using System;
using System.Linq;

namespace CKAN.CmdLine.Command
{
    internal class Available : ManagerCommand
    {
        public Available(IUser user) : base(user)
        {
            IsCommand("availible", "List available mods");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();

            var current_instance = manager.CurrentInstance;
            var available =
                RegistryManager.Instance(current_instance).registry.Available(current_instance.Version());

            User.RaiseMessage("Mods available for KSP {0}", current_instance.Version());
            User.RaiseMessage("");

            var width = User.WindowWidth;

            foreach (var entry in available.Select(module =>
                String.Format("* {0} ({1}) - {2}", module.identifier, module.version, module.name)))
            {
                User.RaiseMessage(width > 0 ? entry.PadRight(width).Substring(0, width - 1) : entry);
            }

            return Exit.OK;
        }
    }
}