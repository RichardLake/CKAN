using System;
using System.Linq;
using ManyConsole;

namespace CKAN.CmdLine.Command
{
    internal class Install : ManagerCommand
    {
        private string ckan_file;
        private bool no_recommends;
        private bool with_suggests;
        private bool with_all_suggests;

        public Install(IUser user) : base(user)
        {
            IsCommand("install", "Install a KSP mod");
            AllowsAnyAdditionalArguments("module_list");

            HasOption("c|ckanfile=", "Local CKAN file to process", s => ckan_file = s);
            HasOption("no-recommends", "Do not install recommended modules", s => no_recommends = true);
            HasOption("with-suggests", "Install suggested modules", s => with_suggests = true);
            HasOption("with-all-suggests", "Install suggested modules all the way down", s => with_all_suggests = true);
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var current_instance = manager.CurrentInstance;
            var modules = remaining_arguments.ToList();

            if (ckan_file != null)
            {
                // Oooh! We're installing from a CKAN file.
                Log.InfoFormat("Installing from CKAN file {0}", ckan_file);

                var mod = CkanModule.FromFile(ckan_file);

                // We'll need to make some registry changes to do this.
                var registry_manager = RegistryManager.Instance(current_instance);

                // Remove this version of the module in the registry, if it exists.
                registry_manager.registry.RemoveAvailable(mod);

                // Sneakily add our version in...
                registry_manager.registry.AddAvailable(mod);

                // Add our module to the things we should install...
                modules.Add(mod.identifier);
                // And continue with our install as per normal.
            }

            if (modules.Count == 0)
            {
                // What? No files specified?
                throw new ConsoleHelpAsException("Need at least one module to install");
            }

            // Prepare  Can these all be done in the new() somehow?
            var install_ops = new RelationshipResolverOptions
            {
                with_all_suggests = with_all_suggests,
                with_suggests = with_suggests,
                with_recommends = !no_recommends
            };

            // Install everything requested. :)
            try
            {
                var installer = ModuleInstaller.GetInstance(current_instance, User);
                installer.InstallList(modules, install_ops);
            }
            catch (ModuleNotFoundKraken ex)
            {
                User.RaiseMessage(
                    "Module {0} required, but not listed in index, or not available for your version of KSP", ex.module);
                User.RaiseMessage("If you're lucky, you can do a `ckan update` and try again.");
                User.RaiseMessage("Try `ckan install --no-recommends` to skip installation of recommended modules");
                return Exit.ERROR;
            }
            catch (BadMetadataKraken ex)
            {
                User.RaiseMessage("Bad metadata detected for module {0}", ex.module);
                User.RaiseMessage(ex.Message);
                return Exit.ERROR;
            }
            catch (TooManyModsProvideKraken ex)
            {
                User.RaiseMessage("Too many mods provide {0}. Please pick from the following:\n", ex.requested);

                foreach (var mod in ex.modules)
                {
                    User.RaiseMessage("* {0} ({1})", mod.identifier, mod.name);
                }

                User.RaiseMessage(String.Empty); // Looks tidier.

                return Exit.ERROR;
            }
            catch (FileExistsKraken ex)
            {
                if (ex.owning_module != null)
                {
                    User.RaiseMessage(
                        "\nOh no! We tried to overwrite a file owned by another mod!\n" +
                        "Please try a `ckan update` and try again.\n\n" +
                        "If this problem re-occurs, then it maybe a packaging bug.\n" +
                        "Please report it at:\n\n" +
                        "https://github.com/KSP-CKAN/CKAN-meta/issues/new\n\n" +
                        "Please including the following information in your report:\n\n" +
                        "File           : {0}\n" +
                        "Installing Mod : {1}\n" +
                        "Owning Mod     : {2}\n" +
                        "CKAN Version   : {3}\n",
                        ex.filename, ex.installing_module, ex.owning_module,
                        Meta.Version()
                        );
                }
                else //TODO Check this. It seems to allow it just fine.
                {
                    User.RaiseMessage(
                        "\n\nOh no!\n\n" +
                        "It looks like you're trying to install a mod which is already installed,\n" +
                        "or which conflicts with another mod which is already installed.\n\n" +
                        "As a safety feature, the CKAN will *never* overwrite or alter a file\n" +
                        "that it did not install itself.\n\n" +
                        "If you wish to install {0} via the CKAN,\n" +
                        "then please manually uninstall the mod which owns:\n\n" +
                        "{1}\n\n" + "and try again.\n",
                        ex.installing_module, ex.filename
                        );
                }

                User.RaiseMessage("Your GameData has been returned to its original state.\n");
                return Exit.ERROR;
            }
            catch (InconsistentKraken ex)
            {
                // The prettiest Kraken formats itself for us.
                User.RaiseMessage(ex.InconsistenciesPretty);
                return Exit.ERROR;
            }
            catch (CancelledActionKraken)
            {
                User.RaiseMessage("Installation cancelled at User request.");
                return Exit.ERROR;
            }
            catch (MissingCertificateKraken kraken)
            {
                // Another very pretty kraken.
                User.RaiseMessage(kraken.ToString());
                return Exit.ERROR;
            }
            catch (DownloadErrorsKraken)
            {
                User.RaiseMessage("One or more files failed to download, stopped.");
                return Exit.ERROR;
            }

            return Exit.OK;
        }
    }
}