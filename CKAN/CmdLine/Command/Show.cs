using System.Linq;

namespace CKAN.CmdLine.Command
{
    internal class Show : ManagerCommand
    {
        private string name;

        public Show(IUser user) : base(user)
        {
            IsCommand("show", "Show information about a mod");
            HasAdditionalArguments(1, "module");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var registry_manager = RegistryManager.Instance(manager.CurrentInstance);
            name = remaining_arguments.First();
            var module = registry_manager.registry.InstalledModule(name);


            if (module == null)
            {
                User.RaiseMessage("{0} not installed.", name);
                User.RaiseMessage("Try `ckan list` to show installed modules");
                return Exit.BADOPT;
            }

            // TODO: Print *lots* of information out; I should never have to dig through JSON

            User.RaiseMessage("{0} version {1}", module.Module.name, module.Module.version);
            User.RaiseMessage("\n== Files ==\n");

            var files = module.Files;

            foreach (var file in files)
            {
                User.RaiseMessage(file);
            }

            return Exit.OK;
        }
    }
}