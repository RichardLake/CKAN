namespace CKAN.CmdLine.Command
{
    internal class Version : CommandBase
    {
        public Version(IUser user) : base(user)
        {
            User = user;
            IsCommand("version", "Show the version of the CKAN client being used.");
        }

        public override int Run(string[] remaining_arguments)
        {
            User.RaiseMessage(Meta.Version());
            return Exit.OK;
        }
    }
}