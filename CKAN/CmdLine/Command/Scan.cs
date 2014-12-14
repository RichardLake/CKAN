namespace CKAN.CmdLine.Command
{
    internal class Scan : ManagerCommand
    {
        public Scan(IUser user) : base(user)
        {
            IsCommand("scan", "Scan for manually installed KSP mods");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            User.RaiseMessage("Scaning {0}", manager.CurrentInstance.GameData());
            manager.CurrentInstance.ScanGameData();
            User.RaiseMessage("Scan completed");
            return Exit.OK;
        }
    }
}