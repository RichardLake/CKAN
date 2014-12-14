using System.Linq;

namespace CKAN.CmdLine.Command.KSP
{
    internal class Forget : CommandBase
    {      
        public Forget(IUser user) : base(user)
        {            
            IsCommand("forget", "Forget a KSP install");
            HasAdditionalArguments(1,"name");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = new KSPManager(User);
            var name = remaining_arguments.First();

            if (!manager.GetInstances().ContainsKey(name))
            {
                User.RaiseMessage("Couldn't find install with name \"{0}\", aborting..", name);
                return Exit.BADOPT;
            }

            manager.RemoveInstance(name);

            User.RaiseMessage("Successfully removed \"{0}\"", name);
            return Exit.OK;
        }
    }
}