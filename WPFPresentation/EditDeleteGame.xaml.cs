using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EditDeleteGame.xaml
    /// </summary>
    public partial class EditDeleteGame : Window
    {
        Game _game;
        List<Opening> _openings;
        GameManager _gameManager = new GameManager();
        public EditDeleteGame(Game game)
        {
            InitializeComponent();
            _game = game;
            populateControls();
        }

        private void populateControls()
        {
            txtGameID.Text = _game.GameID.ToString();
            txtPlayerWhite.Text = _game.PlayerWhite;
            txtWhiteElo.Text = _game.WhiteElo.ToString();
            txtPlayerBlack.Text = _game.PlayerBlack;
            txtBlackElo.Text = _game.BlackElo.ToString();
            cboECO.SelectedItem = _game.ECO;
            txtOpening.Text = _game.Opening;
            txtTermination.Text = _game.Termination;
            cboOutcome.SelectedItem = _game.Outcome;
            txtTimeControl.Text = _game.TimeControl;
            dteDatePlayed.SelectedDate = _game.DatePlayed;

            txtGameID.IsEnabled = false;
            dteDatePlayed.SelectedDateFormat = DatePickerFormat.Short;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // validate fields
            /*
                txtGameID
                txtPlayerWhite
                txtWhiteElo
                txtPlayerBlack
                txtBlackElo
                cboECO
                txtOpening
                txtTermination
                cboOutcome
                txtTimeControl
                dteDatePlayed
            */
            int whiteElo;
            int blackElo;

            if (txtPlayerWhite.Text.ToString() == "")
            {
                MessageBox.Show("You must enter a name for Player White.");
                txtPlayerWhite.Focus();
                return;
            }
            if (txtWhiteElo.Text.ToString() == "")
            {
                MessageBox.Show("You must enter an Elo for Player White.");
                txtWhiteElo.Focus();
                return;
            }
            try
            {
                whiteElo = int.Parse(txtWhiteElo.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("White player's Elo must be a whole number.");
                txtWhiteElo.SelectAll();
                txtWhiteElo.Focus();
                return;
            }
            if (txtPlayerWhite.Text.ToString() == "")
            {
                MessageBox.Show("You must enter a name for Player Black.");
                txtPlayerBlack.Focus();
                return;
            }
            if (txtBlackElo.Text.ToString() == "")
            {
                MessageBox.Show("You must enter an Elo for Player Black.");
                txtWhiteElo.Focus();
                return;
            }
            try
            {
                blackElo = int.Parse(txtBlackElo.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Black player's Elo must be a whole number.");
                txtBlackElo.SelectAll();
                txtBlackElo.Focus();
                return;
            }
            if (txtOpening.Text.ToString() == "")
            {
                MessageBox.Show("You must enter an Opening name.");
                txtOpening.Focus();
                return;
            }
            if (txtTermination.Text.ToString() == "")
            {
                MessageBox.Show("You must enter a termination type.\n\nCheckmate and resignation should be 'Normal'.");
                txtTermination.Focus();
                return;
            }
            if (txtTimeControl.Text.ToString() == "" || !Regex.Match(txtTimeControl.Text.ToString(), @"^(\d+\+\d+)$").Success)
            {
                MessageBox.Show("You must enter a Time Control.\n\nFormat should be '[total seconds]+[increment seconds]'.\ne.g. 180+2");
                txtTimeControl.Focus();
                return;
            }

            // create new game object
            Game newGame = new Game()
            {
                PlayerWhite = txtPlayerWhite.Text,
                WhiteElo = whiteElo,
                PlayerBlack = txtPlayerBlack.Text,
                BlackElo = blackElo,
                ECO = cboECO.SelectedItem.ToString(),
                Opening = txtOpening.Text,
                Termination = txtTermination.Text,
                Outcome = cboOutcome.SelectedItem.ToString(),
                TimeControl = txtTimeControl.Text,
                DatePlayed = dteDatePlayed.SelectedDate
            };

            // run update with old and new game objects
            try
            {
                _gameManager.UpdateGame(_game, newGame);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update game.\n\n" + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("You are about to delete this game.\n\nAre you sure? This cannot be undone.",
                            "Are you sure?",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _gameManager.RemoveGame(_game);
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete game.\n\n" + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?",
                            "Discarding Changes",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _openings = _gameManager.RetrieveAllOpenings();
                this.cboECO.ItemsSource = from o in _openings
                                                    orderby o.ECO
                                                    select  o.ECO;
                this.cboOutcome.ItemsSource = new string[] { "1-0", "0-1", "1/2-1/2" };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opening List Not Retrieved.\n\n" + ex.Message);
            }
        }
    }
}
