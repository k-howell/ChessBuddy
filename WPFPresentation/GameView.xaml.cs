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
using DataObjects;
using LogicLayer;

namespace WPFPresentation
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Window
    {
        BoardManager _boardManager;
        UserManager _userManager = new UserManager();
        User _user;
        Game _game;
        // GameManager _gameManager = new GameManager();
        public GameView(User user, Game game)
        {
            InitializeComponent();
            _user = user;
            _game = game;
            _boardManager = new BoardManager(game);
            ctrlBoard.ItemsSource = _boardManager.Board;

            // Meta data labels
            lblPlayerWhite.Content = "White Player: " + game.PlayerWhite;
            lblEloWhite.Content = "Elo: " + game.WhiteElo;

            lblECO.Content = "Opening: " + game.ECO;
            lblOpening.Content = game.Opening;

            lblPlayerBlack.Content = "Black Player: " + game.PlayerBlack;
            lblEloBlack.Content = "Elo: " + game.BlackElo;

            // user favorites
            if(_user.Favorites == null)
            {
                _user.Favorites = _userManager.GetUserFavorites(_user.UserName);
            }
            _user.Favorites.ForEach((g) => {
                if(g.GameID == game.GameID)
                {
                    btnFavoriteGame.Content = "Remove from Favorites";
                    return;
                }
            });
        }

        // default constructor for future playing on board features without reading from a game record
        public GameView(User user)
        {
            InitializeComponent();
            _user = user;
            _boardManager = new BoardManager();
            ctrlBoard.ItemsSource = _boardManager.Board;
        }

        private void btnFlipBoard_Click(object sender, RoutedEventArgs e)
        {
            _boardManager.FlipBoard();
            // no need to call UpdateDisplay() because display is updated when FlipBoard calls Add()
        }

        private void btnPrevMove_Click(object sender, RoutedEventArgs e)
        {
            _boardManager.PrevGameMove();
            UpdateDisplay();
        }

        private void btnNextMove_Click(object sender, RoutedEventArgs e)
        {
            _boardManager.NextGameMove();
            UpdateDisplay();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Space)
            {
                _boardManager.NextGameMove();
                UpdateDisplay();
            }
            if (e.Key == Key.Left)
            {
                _boardManager.PrevGameMove();
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            // only solution I could find to force the ItemsControl to update the viewmodel
            // it only seems to update when .Add or .Remove is called on the bound datamodel
            // but pieces often move without adding or removing a piece from the board
            ctrlBoard.ItemsSource = null;
            ctrlBoard.ItemsSource = _boardManager.Board;
        }

        private void btnResetBoard_Click(object sender, RoutedEventArgs e)
        {
            _boardManager.ResetBoard();
            UpdateDisplay();
        }

        private void btnFavoriteGame_Click(object sender, RoutedEventArgs e)
        {
            if(btnFavoriteGame.Content.ToString() == "Add to Favorites")
            {
                // add game to user favorites
                _userManager.AddUserFavorite(_user.UserName, _game.GameID);
                _user.Favorites = _userManager.GetUserFavorites(_user.UserName);

                // toggle button to remove
                btnFavoriteGame.Content = "Remove from Favorites";
            }
            else
            {
                // remove game from user favorites
                _userManager.RemoveUserFavorite(_user.UserName, _game.GameID);
                _user.Favorites = _userManager.GetUserFavorites(_user.UserName);

                // toggle button to add
                btnFavoriteGame.Content = "Add to Favorites";
            }
        }
    }
}
