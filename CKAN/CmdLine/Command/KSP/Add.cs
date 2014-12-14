
namespace CKAN.CmdLine.Command.KSP
{
    internal class Add : CommandBase
    {
        public Add(IUser user) : base(user)
        {
            IsCommand("add", "Add a KSP install");
            HasAdditionalArguments(2, "name path");            
        }


        public override int Run(string[] remaining_arguments)
        {
            //Contract.Requires(remaining_arguments.Length==2);
            var manager = new KSPManager(User);
            var name = remaining_arguments[0];
            var path = remaining_arguments[1];

            if (manager.GetInstances().ContainsKey(name))
            {
                User.RaiseMessage("Install with name \"{0}\" already exists, aborting..", name);
                return Exit.BADOPT;
            }

            try
            {
                manager.AddInstance(name, path);
                User.RaiseMessage("Added \"{0}\" with root \"{1}\" to known installs", name, path);
                return Exit.OK;
            }
            catch (NotKSPDirKraken ex)
            {
                User.RaiseMessage("Sorry, {0} does not appear to be a KSP directory", ex.path);
                return Exit.BADOPT;
            }
        }
    }
}