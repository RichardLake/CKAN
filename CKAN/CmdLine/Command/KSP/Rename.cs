namespace CKAN.CmdLine.Command.KSP
{
    internal class Rename : ManagerCommand
    {
        

        public Rename(IUser user) : base(user)
        {
            IsCommand("rename", "Rename a KSP install");
            HasAdditionalArguments(2, "oldname newname");
        }

        public override int Run(string[] remaining_arguments)
        {
            //Contract.Requires(remaining_arguments.Length==1);
            var old_name = remaining_arguments[0];
            var new_name = remaining_arguments[1];
            var manager = GetManagerForOptions();

            if (!manager.GetInstances().ContainsKey(old_name))
            {
                User.RaiseMessage("Couldn't find install with name \"{0}\", aborting..", old_name);
                return Exit.BADOPT;
            }

            manager.RenameInstance(old_name, new_name);

            User.RaiseMessage("Successfully renamed \"{0}\" to \"{1}\"", old_name, new_name);
            return Exit.OK;
        }
    }
}