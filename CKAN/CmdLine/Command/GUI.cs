namespace CKAN.CmdLine.Command
{
    public class GUI : CommandBase
    {
        public GUI(IUser user) : base(user)
        {
            IsCommand("gui", "Start the graphical user interface");
        }

        public override int Run(string[] remaining_arguments)
        {
            // TODO: Sometimes when the GUI exits, we get a System.ArgumentException,
            // but trying to catch it here doesn't seem to help. Dunno why.

            CKAN.GUI.Main();
            return Exit.OK;
        }
    }
}