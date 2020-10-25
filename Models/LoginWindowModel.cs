using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LandOfRails_Launcher.Helper;

namespace LandOfRails_Launcher.Models
{
    class LoginWindowModel : BaseViewModel
    {
        private string _EMailText;
        public string EMailText
        {
            get { return _EMailText; }
            set { SetProperty(ref _EMailText, value); }
        }

        public string password;

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get { return _loginCommand ?? (_loginCommand = new CommandHandler(() => checkLogin(), () => true)); }
        }

        public LoginWindowModel()
        {

        }

        private bool checkLogin()
        {

            return true;
        }

    }
}
