using System;
using CKAN;
using Eto.Forms;

namespace EtoGUI
{
    internal static class StartUp
    {
        [STAThread]
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var user = new User();
            var manager = new KSPManager(user);
            var app = new Program();
            app.Run(new MainForm(manager));
        }
    }
}