using System;

namespace CKAN.CmdLine.Command.KSP
{
    internal class List : CommandBase
    {
        public List(IUser user) : base(user)
        {
            IsCommand("list", "List KSP installs");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = new KSPManager(User);
            User.RaiseMessage("Listing all known KSP installations:");
            User.RaiseMessage(String.Empty);

            var count = 1;
            foreach (var instance in manager.GetInstances())
            {
                User.RaiseMessage("{0}) \"{1}\" - {2}", count, instance.Key, instance.Value.GameDir());
                count++;
            }

            return Exit.OK;
        }
    }
}