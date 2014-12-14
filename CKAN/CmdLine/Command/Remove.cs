using ManyConsole;

namespace CKAN.CmdLine.Command
{
    internal class Remove : ManagerCommand
    {
        public Remove(IUser user) : base(user)
        {
            IsCommand("remove", "Remove an installed mod");
            HasAdditionalArguments(null, "module_list");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var module_names = remaining_arguments;

            if (module_names == null || module_names.Length <= 0)
            {
                throw new ConsoleHelpAsException("Need at least one module");
            }

            try
            {
                var installer = ModuleInstaller.GetInstance(manager.CurrentInstance, User);
                installer.UninstallList(module_names);
                return Exit.OK;
            }
            catch (ModNotInstalledKraken kraken)
            {
                User.RaiseMessage("I can't do that, {0} isn't installed.", kraken.mod);
                User.RaiseMessage("Try `ckan list` for a list of installed mods.");
                return Exit.BADOPT;
            }
        }
    }
}