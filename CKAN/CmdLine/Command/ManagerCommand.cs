using log4net;
using ManyConsole;

namespace CKAN.CmdLine.Command
{
    internal abstract class ManagerCommand : CommandBase
    {
        protected ILog Log;
        private string ksp_name;
        private string ksp_directory;

        protected ManagerCommand(IUser user) : base(user)
        {
            HasOption("ksp=", "KSP install to use", s => ksp_name = s);
            HasOption("kspdir=", "KSP dir to use", s => ksp_directory = s);
            Log = LogManager.GetLogger(GetType());
        }

        protected KSPManager GetManagerForOptions()
        {
            var manager = new KSPManager(User);
            if (ksp_directory != null && ksp_name != null)
            {                                
                throw new ConsoleHelpAsException("--ksp and --kspdir can't be specified at the same time");
            }
            if (ksp_name != null)
            {
                // Set a KSP directory by its alias.
                try
                {
                    manager.SetCurrentInstance(ksp_name);
                }
                catch (InvalidKSPInstanceKraken)
                {
                    User.RaiseMessage(
                        "Invalid KSP installation specified \"{0}\", use '--kspdir' to specify by path, or 'list-installs' to see known KSP installations",
                        ksp_name);
                    throw new ConsoleHelpAsException("Test");
                }
            }
            else if (ksp_directory != null)
            {
                // Set a KSP directory by its path
                manager.SetCurrentInstanceByPath(ksp_directory);
            }
            else
            {
                // Find whatever our preferred instance is.
                // We don't do this on `ksp/version` commands, they don't need it.
                var ksp_instance = manager.GetPreferredInstance();

                if (ksp_instance == null)
                {
                    throw new ConsoleHelpAsException("I don't know where KSP is installed.\n"
                                                     +
                                                     "Either specify a version or use 'ckan ksp help' for assistance on setting a default");
                }
            }
            Log.InfoFormat("Using KSP install at {0}", manager.CurrentInstance.GameDir());            
            return manager;
        }
    }
}