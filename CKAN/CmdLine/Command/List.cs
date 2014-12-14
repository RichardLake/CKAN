using System.Collections.Generic;
using System.Linq;

namespace CKAN.CmdLine.Command
{
    internal class List : ManagerCommand
    {
        public List(IUser user) : base(user)
        {
            IsCommand("list", "Returns a list of installed modules");
        }

        public override int Run(string[] remaining_arguments)
        {
            var manager = GetManagerForOptions();
            var ksp = manager.CurrentInstance;

            User.RaiseMessage("\nKSP found at {0}\n", ksp.GameDir());
            User.RaiseMessage("KSP Version: {0}\n", ksp.Version());

            var registry = RegistryManager.Instance(ksp).registry;

            User.RaiseMessage("Installed Modules:\n");

            var installed = new SortedDictionary<string, CKAN.Version>(registry.Installed());

            // Skip virtuals for now.
            foreach (var mod in installed.Where(mod => !(mod.Value is ProvidesVersion)))
            {
                var current_version = mod.Value;

                var bullet = GetBullet(current_version, registry, mod, ksp);

                User.RaiseMessage("{0} {1} {2}", bullet, mod.Key, mod.Value);
            }

            User.RaiseMessage(
                string.Format(
                    "\nLegend: {0} - Up to date. {1} - Incompatible. {2} - Upgradable. {3} - Unknown. {4} - Autodetected",
                    UpToDate, Incompatible, Upgradeable, Unknown, Autodetected));

            return Exit.OK;
        }

        private const string UpToDate = "✓";
        private const string Upgradeable = "↑";
        private const string Incompatible = "✗";
        private const string Autodetected = "-";
        private const string Unknown = "?";

        private string GetBullet(CKAN.Version current_version, Registry registry, KeyValuePair<string, CKAN.Version> mod,
            CKAN.KSP ksp)
        {
            const string bullet = "*";

            if (current_version is DllVersion)
            {
                return Autodetected;
            }
            try
            {
                // Check if upgrades are available, and show appropriately.
                var latest = registry.LatestAvailable(mod.Key, ksp.Version());

                Log.InfoFormat("Latest {0} is {1}", mod.Key, latest);

                if (latest == null)
                {
                    return Incompatible;
                }
                if (latest.version.IsEqualTo(current_version))
                {
                    return UpToDate;
                }
                if (latest.version.IsGreaterThan(mod.Value))
                {
                    return Upgradeable;
                }
            }
            catch (ModuleNotFoundKraken)
            {
                Log.InfoFormat("{0} is installed, but no longer in the registry",
                    mod.Key);
                return Unknown;
            }
            return bullet;
        }
    }
}