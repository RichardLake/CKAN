using CKAN;
using Eto.Drawing;
using Eto.Forms;

namespace EtoGUI
{
    public class Program : Application
    {
        
    }
    internal class MainForm : Form
    {
        private KSPManager Manager { get; set; }

        public MainForm(KSPManager manager)
        {
            Manager = manager;
            ClientSize = new Size(600, 400);
            Title = "CKAN";
        }
    }
}