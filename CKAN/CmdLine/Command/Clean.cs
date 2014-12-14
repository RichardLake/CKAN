namespace CKAN.CmdLine.Command
{
    internal class Clean : ManagerCommand
    {
        public Clean(IUser user) : base(user)
        {
            IsCommand("clean", "Clean away downloaded files from the cache");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            manager.CurrentInstance.CleanCache();
            return Exit.OK;
        }
    }
}