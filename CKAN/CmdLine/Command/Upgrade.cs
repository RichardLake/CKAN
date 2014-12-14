using System.Collections.Generic;
using System.Text.RegularExpressions;
using ManyConsole;

namespace CKAN.CmdLine.Command
{
    internal class Upgrade : ManagerCommand
    {
        private string ckan_file;
        //private bool no_recommends;
        //private bool with_suggests;
        //private bool with_all_suggests;
        public Upgrade(IUser user) : base(user)
        {
            IsCommand("upgrade", "Upgrade an installed mod");
            HasOption("ckanfile=", "Local CKAN file to process", s => ckan_file = s);
            //HasOption("no-recommends", "Do not install recommended modules", s=>no_recommends = true);
            //HasOption("with-suggests", "Install suggested modules", s => with_suggests = true);
            //HasOption("with-all-suggests", "Install suggested modules all the way down", s => with_all_suggests = true);            
            HasAdditionalArguments(null, "module_list");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var current_instance = manager.CurrentInstance;

            if (ckan_file != null)
            {
                User.RaiseMessage("\nUnsupported option at this time.");
                return Exit.BADOPT;
            }
            var modules = remaining_arguments;

            if (modules.Length == 0)
            {
                // What? No files specified?
                throw new ConsoleHelpAsException("Need at least one module");
            }

            var to_upgrade = new List<CkanModule>();

            foreach (var mod in modules)
            {
                var match = Regex.Match(mod, @"^(?<mod>[^=]*)=(?<version>.*)$");

                if (match.Success)
                {
                    var ident = match.Groups["mod"].Value;
                    var version = match.Groups["version"].Value;


                    var module = current_instance.Registry.GetModuleByVersion(ident, version);

                    if (module == null)
                    {
                        User.RaiseMessage("Cannot install {0}, version {1} not available", ident, version);
                        return Exit.ERROR;
                    }

                    to_upgrade.Add(module);
                }
                else
                {
                    to_upgrade.Add(
                        current_instance.Registry.LatestAvailable(mod, current_instance.Version())
                        );
                }
            }


            User.RaiseMessage("\nUpgrading modules...\n");
            // TODO: These instances all need to go.
            ModuleInstaller.GetInstance(current_instance, User).Upgrade(to_upgrade, new NetAsyncDownloader(User));
            User.RaiseMessage("\nDone!\n");
            return Exit.OK;
        }
    }
}