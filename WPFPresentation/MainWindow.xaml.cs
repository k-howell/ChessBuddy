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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataObjects;
using LogicLayer;

namespace WPFPresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserManager _userManager = null;
        GameManager _gameManager = null;
        User _user = null;
        public MainWindow()
        {
            _userManager = new UserManager();
            _gameManager = new GameManager();

            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(btnLogin.Content.ToString() == "Login")
            {
                var userName = this.txtUserName.Text;
                var pwd = this.pwdPassword.Password;

                try
                {
                    _user = _userManager.LoginUser(userName, pwd);
                    string instructions = "On first login, all new users must choose a personal password.";

                    if(_user != null && pwd == "newuser")
                    {
                        var updateWindow = new UpdatePasswordWindow(
                                _userManager, _user, instructions, true);

                        bool? result = updateWindow.ShowDialog();

                        if (result == true)
                        {
                            updateUIforLogin();
                        }
                        else
                        {
                            _user = null;
                            updateUIforLogout();
                            txtUserName.Text = "";
                            pwdPassword.Password = "";
                            txtUserName.Focus();
                            MessageBox.Show("You did not update your password.  You will be logged out.");
                        }
                    }
                    else if(_user != null)
                    {
                        updateUIforLogin();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.InnerException.Message,
                                        "Login Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                    pwdPassword.Password = "";
                    txtUserName.Focus();
                    txtUserName.Select(0, Int32.MaxValue);
                }
            }
            else
            {
                updateUIforLogout();
            }
        }
        private void hideAllSearchUI()
        {
            foreach (var tab in tabsetMain.Items)
            {
                ((TabItem)tab).Visibility = Visibility.Collapsed;
            }
            tabsetMain.Visibility = Visibility.Collapsed;
            btnSearch.Visibility = Visibility.Hidden;
            lblECO.Visibility = Visibility.Hidden;
            cboECO.Visibility = Visibility.Hidden;
        }

        private void showSearchUI()
        {
            foreach (var role in _user.Roles)
            {
                switch (role)
                {
                    case "Guest":
                        tabSearch.Visibility = Visibility.Visible;
                        tabFavorites.Visibility = Visibility.Visible;
                        tabSearch.IsSelected = true;
                        break;
                    case "Admin":
                        tabSearch.Visibility = Visibility.Visible;
                        tabFavorites.Visibility = Visibility.Visible;
                        tabEdit.Visibility = Visibility.Visible;
                        tabSearch.IsSelected = true;
                        break;
                }
            }

            tabsetMain.Visibility = Visibility.Visible;

            lblECO.Visibility = Visibility.Visible;
            cboECO.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Visible;
        }

        private void updateUIforLogin()
        {
            // hide login input
            txtUserName.Text = "";
            pwdPassword.Password = "";
            txtUserName.Visibility = Visibility.Hidden;
            pwdPassword.Visibility = Visibility.Hidden;
            lblUserName.Visibility = Visibility.Hidden;
            lblPassword.Visibility = Visibility.Hidden;

            // show change password button
            btnChangePassword.Visibility = Visibility.Visible;

            // change login button to logout
            btnLogin.Content = "Log Out";
            btnLogin.IsDefault = false;

            // show search UI
            showSearchUI();

            // update status
            this.staMessage.Content = "Welcome! Click search, then double click a game to review it.";
        }

        private void updateUIforLogout()
        {
            _user = null;
            // show login input
            txtUserName.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Visible;
            lblUserName.Visibility = Visibility.Visible;
            lblPassword.Visibility = Visibility.Visible;

            // hide change password button
            btnChangePassword.Visibility = Visibility.Hidden;

            // change logout button to login
            btnLogin.Content = "Login";
            btnLogin.IsDefault = true;
            txtUserName.Focus();

            // hide search UI
            hideAllSearchUI();

            // update status
            this.staMessage.Content = "Please log in to continue.";
        }

        private void datGames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var game = (Game)datGames.SelectedItem;
            if(game != null) // double clicking blank space will try to pass a null SelectedItem
            {
                game.Moves = _gameManager.GetMovesForGame(game);

                var gameView = new GameView(_user, game);
                gameView.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            switch(tabsetMain.SelectedIndex)
            {
                case 0: // Search tab
                    if(cboECO.SelectedItem.ToString() == "All Games")
                    {
                        try
                        {
                            datGames.ItemsSource = _gameManager.RetrieveAllGames();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            datGames.ItemsSource = _gameManager.RetrieveGamesByECO(cboECO.SelectedItem.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    fixGameColumns(datGames);
                    break;
                case 1: // Favorites tab
                    try
                    {
                        if (_user.Favorites == null)
                        {
                            _user.Favorites = _userManager.GetUserFavorites(_user.UserName);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (cboECO.SelectedItem.ToString() != "All Games")
                    {
                        List<Game> games = new List<Game>();
                        _user.Favorites.ForEach((g) =>
                        {
                            if(g.ECO == cboECO.SelectedItem.ToString())
                            {
                                games.Add(g);
                            }
                        });

                        datFavorites.ItemsSource = games;
                    }
                    else
                    {
                        datFavorites.ItemsSource = _user.Favorites;
                    }

                    fixGameColumns(datFavorites);
                    break;
                case 2: // Edit/Delete Mode
                    if (cboECO.SelectedItem.ToString() == "All Games")
                    {
                        try
                        {
                            datEdit.ItemsSource = _gameManager.RetrieveAllGames();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            datEdit.ItemsSource = _gameManager.RetrieveGamesByECO(cboECO.SelectedItem.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    fixGameColumns(datEdit);
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hideAllSearchUI();
            this.txtUserName.Focus();
            this.staMessage.Content = "Please log in to continue.";
            List<Opening> allECO = _gameManager.RetrieveAllOpenings();
            List<string> allECOcode = new List<string>();

            allECOcode.Add("All Games");
            allECO.ForEach((eco) =>
            {
                if(!allECOcode.Contains(eco.ECO))
                {
                    allECOcode.Add(eco.ECO);
                }
            });

            cboECO.ItemsSource = allECOcode;
            cboECO.SelectedIndex = 0;
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string instructions = "Please enter your current password and a new password.";
            var updateWindow = new UpdatePasswordWindow(
                                _userManager, _user, instructions);
            updateWindow.ShowDialog();
        }

        private void tabFavorites_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_user.Favorites == null)
                {
                    _user.Favorites = _userManager.GetUserFavorites(_user.UserName);
                }
                datFavorites.ItemsSource = _user.Favorites;
                fixGameColumns(datFavorites);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void datFavorites_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var game = (Game)datFavorites.SelectedItem;
            if (game != null) // double clicking blank space will try to pass a null SelectedItem
            {
                game.Moves = _gameManager.GetMovesForGame(game);

                var gameView = new GameView(_user, game);
                gameView.ShowDialog();
            }
        }

        private void fixGameColumns(DataGrid table)
        {
            if (table.Columns[0].Header.ToString() != "Player White")
            {
                //table.Columns[0].Header = "Game ID";
                table.Columns.RemoveAt(0);
                table.Columns[0].Header = "Player White";
                table.Columns[1].Header = "White Elo";
                table.Columns[2].Header = "Player Black";
                table.Columns[3].Header = "Black Elo";
                table.Columns.RemoveAt(10);
            }
        }

        private void datEdit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Edit Delete Window
            var game = (Game)datEdit.SelectedItem;

            var editDeleteWindow = new EditDeleteGame(game);

            var result = (bool)editDeleteWindow.ShowDialog();

            if (result)
            {
                datEdit.ItemsSource = _gameManager.RetrieveAllGames();
                fixGameColumns(datEdit);
            }
        }
    }
}