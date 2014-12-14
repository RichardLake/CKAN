namespace CKAN.CmdLine.Command
{
    internal class Update : ManagerCommand
    {
        private string repo;

        public Update(IUser user) : base(user)
        {
            IsCommand("update", "Update list of available mods");
            HasOption("repo|r=", "CKAN repository to use (experimental!)", s => repo = s);
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            User.RaiseMessage("Downloading updates...");

            try
            {
                var current_instance = manager.CurrentInstance;
                var registry_manager = RegistryManager.Instance(current_instance);
                var updated = Repo.Update(registry_manager, current_instance.Version(), repo);
                User.RaiseMessage("Updated information on {0} available modules", updated);
            }
            catch (MissingCertificateKraken kraken)
            {
                // Handling the kraken means we have prettier output.
                User.RaiseMessage(kraken.ToString());
                return Exit.ERROR;
            }

            return Exit.OK;
        }
    }
}