using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LogicLayer;
using DataObjects;

namespace WPFPresentation
{
    /// <summary>
    /// Interaction logic for UpdatePasswordWindow.xaml
    /// </summary>
    public partial class UpdatePasswordWindow : Window
    {
        UserManager _userManager = null;
        User _user = null;
        bool _isNewUser;

        public UpdatePasswordWindow(UserManager userManager, User user, string instructions, bool isNewUser = false)
        {
            InitializeComponent();

            txtInstructions.Text = instructions;
            _userManager = userManager;
            _user = user;
            _isNewUser = isNewUser;

            if(_isNewUser)
            {
                newUserUpdate();
            }
            else
            {
                pwdOldPassword.Focus();
            }
        }

        private void newUserUpdate()
        {
            pwdOldPassword.Password = "newuser";
            pwdOldPassword.IsEnabled = false;
            pwdNewPassword.Focus();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (pwdNewPassword.Password != pwdRetypePassword.Password)
            {
                MessageBox.Show("Passwords do not match.  Please re-enter a new password.");
                pwdNewPassword.Password = "";
                pwdRetypePassword.Password = "";
                pwdNewPassword.Focus();

                return;
            }
            if (pwdNewPassword.Password == "")
            {
                MessageBox.Show("Please enter a new password.");
                pwdNewPassword.Password = "";
                pwdRetypePassword.Password = "";
                pwdNewPassword.Focus();

                return;
            }
            try
            {
                string oldPassword = pwdOldPassword.Password;
                string newPassword = pwdNewPassword.Password;

                if (_userManager.ResetPassword(_user.UserName, oldPassword, newPassword))
                {
                    MessageBox.Show("Password updated.");
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed.\n\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
