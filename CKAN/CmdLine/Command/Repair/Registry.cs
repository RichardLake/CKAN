namespace CKAN.CmdLine.Command.Repair
{
    internal class Registry : ManagerCommand
    {
        public Registry(IUser user) : base(user)
        {
            IsCommand("registry", "Try to repair the CKAN registry");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var current_instance = manager.CurrentInstance;
            current_instance.Registry.Repair();
            current_instance.RegistryManager.Save();
            User.RaiseMessage("Registry repairs attempted. Hope it helped.");
            return Exit.OK;
        }
    }
}