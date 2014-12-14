namespace CKAN.CmdLine.Command.KSP
{
    internal class Default : CommandBase
    {
        public Default(IUser user) : base(user)
        {
            User = user;
            IsCommand("default", "Set the default KSP install");
            HasAdditionalArguments(1, "name");            
        }

        public override int Run(string[] remaining_arguments)
        {
            //Contract.Requires(remaining_arguments.Length==1);
            var manager = new KSPManager(User);
            var name = remaining_arguments[0];
            if (!manager.GetInstances().ContainsKey(name))
            {
                User.RaiseMessage("Couldn't find install with name \"{0}\", aborting..", name);
                return Exit.BADOPT;
            }

            manager.SetAutoStart(name);
            User.RaiseMessage("Successfully set \"{0}\" as the default KSP installation", name);
            return Exit.OK;
        }
    }
}