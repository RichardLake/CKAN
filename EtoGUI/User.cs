using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CKAN;

namespace EtoGUI
{
    internal class User : IUser
    {
        public event DisplayYesNoDialog AskUser;
        public event DisplaySelectionDialog AskUserForSelection;
        public event DisplayMessage Message;
        public event DisplayError Error;
        public event ReportProgress Progress;
        public event DownloadsComplete DownloadsComplete;

        public int WindowWidth => -1;
        public bool Headless => false;

        public int RaiseSelectionDialog(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void RaiseMessage(string message, params object[] url)
        {
            throw new NotImplementedException();
        }

        public void RaiseProgress(string message, int percent)
        {
            throw new NotImplementedException();
        }

        public bool RaiseYesNoDialog(string question)
        {
            throw new NotImplementedException();
        }

        public void RaiseError(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void RaiseDownloadsCompleted(Uri[] file_urls, string[] file_paths, Exception[] errors)
        {
            throw new NotImplementedException();
        }
    }
}
