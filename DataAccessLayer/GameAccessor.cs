using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessInterfaces;
using DataObjects;

namespace DataAccessLayer
{
    public class GameAccessor : IGameAccessor
    {
        public int DeleteGame(int gameID)
        {
            int rowsAffected = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_delete_game";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@GameID", SqlDbType.Int);

            // set arguments
            cmd.Parameters["@GameID"].Value = gameID;

            // open connection and execute command
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return rowsAffected;
        }

        public int InsertGame() // future implementation parsing .pgn files to add to database
        {
            throw new NotImplementedException();
        }

        public int InsertVariation(string eco, string name)
        {
            int rowsAffected = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_insert_opening_variation";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@ECO", SqlDbType.NChar, 3);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);

            // set arguments
            cmd.Parameters["@ECO"].Value = eco;
            cmd.Parameters["@Name"].Value = name;

            // open connection and execute command
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return rowsAffected;
        }

        public List<Game> SelectAllGames()
        {
            List<Game> games = new List<Game>();

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_all_games";

            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        games.Add(new Game()
                        {
                            // GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, ECO, [Opening].Name,
			                // OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
                            GameID = reader.GetInt32(0),
                            PlayerWhite = reader.GetString(1),
                            WhiteElo = reader.IsDBNull(2) ? 0 : reader.GetInt32(2), // should be null rather than 0, but I can't, despite WhiteElo being nullable int
                            PlayerBlack = reader.GetString(3),
                            BlackElo = reader.IsDBNull(4) ? 0 : reader.GetInt32(4), // same as above.  I guess 0 is fine for unrated players.
                            //Opening = new Opening()
                            //{
                            //    ECO = reader.GetString(5),
                            //    Name = reader.GetString(6),
                            //    Variation = reader.GetString(7)
                            //},
                            ECO = reader.GetString(5),
                            Opening = reader.GetString(7),
                            Termination = reader.GetString(8),
                            Outcome = reader.GetString(9),
                            TimeControl = reader.GetString(10),
                            DatePlayed = reader.GetDateTime(11)
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return games;
        }

        public List<Opening> SelectAllOpenings()
        {
            List<Opening> openings = new List<Opening>();

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_all_variations";

            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        openings.Add(new Opening()
                        {
                            // [OpeningVariation].ECO,
                            // [Opening].Name,
                            // [OpeningVariation].Name
                            ECO = reader.GetString(0),
                            Name = reader.GetString(1),
                            Variation = reader.GetString(2)
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return openings;
        }

        public List<Game> SelectGamesByECO(string eco)
        {
            List<Game> games = new List<Game>();

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_games_by_ECO";

            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ECO", SqlDbType.NChar, 3);
            cmd.Parameters["@ECO"].Value = eco;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        games.Add(new Game()
                        {
                            // GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, ECO, [Opening].Name,
                            // OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
                            GameID = reader.GetInt32(0),
                            PlayerWhite = reader.GetString(1),
                            WhiteElo = reader.IsDBNull(2) ? 0 : reader.GetInt32(2), // should be null rather than 0, but I can't, despite WhiteElo being nullable int
                            PlayerBlack = reader.GetString(3),
                            BlackElo = reader.IsDBNull(4) ? 0 : reader.GetInt32(4), // same as above.  I guess 0 is fine for unrated players.
                            //Opening = new Opening()
                            //{
                            //    ECO = reader.GetString(5),
                            //    Name = reader.GetString(6),
                            //    Variation = reader.GetString(7)
                            //},
                            ECO = reader.GetString(5),
                            Opening = reader.GetString(7),
                            Termination = reader.GetString(8),
                            Outcome = reader.GetString(9),
                            TimeControl = reader.GetString(10),
                            DatePlayed = reader.GetDateTime(11)
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return games;
        }

        public List<Move> SelectMovesByGameID(int gameID)
        {
            List<Move> moves = new List<Move>();

            // connection
            var conn = DBConnection.GetConnection();

            // command text
            string commandText = @"sp_select_moves_by_gameID";

            // command
            var cmd = new SqlCommand(commandText, conn);

            // command type
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@GameID", SqlDbType.Int);

            // parameter values
            cmd.Parameters["@GameID"].Value = gameID;

            try
            {
                // open connection
                conn.Open();

                // execute command and capture results
                var reader = cmd.ExecuteReader();

                // process results
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // string Origin { get; set; }
                        // string Destination { get; set; }
                        // PieceType PieceType { get; set; }
                        // Color Color { get; set; }
                        // Piece Promotion { get; set; }
                        // Piece Capture { get; set; }
                        // string Notation { get; set; }
                        // bool IsCheck { get; set; }
                        // bool IsCastle { get; set; }
                        moves.Add(new Move()
                        {
                            Color = reader.GetString(2) == "White" ? Color.White : Color.Black,
                            PieceType = reader.GetString(3) == "Pawn" ? PieceType.Pawn :
                                        reader.GetString(3) == "Knight" ? PieceType.Knight :
                                        reader.GetString(3) == "Bishop" ? PieceType.Bishop :
                                        reader.GetString(3) == "Rook" ? PieceType.Rook :
                                        reader.GetString(3) == "Queen" ? PieceType.Queen : PieceType.King,
                            Notation = reader.GetString(4),
                            Origin = reader.GetString(5),
                            Destination = reader.GetString(6),
                            Capture = reader.IsDBNull(7) ? PieceType.None : // I despise this solution, but can't set to null despite Capture being nullable PieceType?
                                        reader.GetString(7) == "Pawn" ? PieceType.Pawn :
                                        reader.GetString(7) == "Knight" ? PieceType.Knight :
                                        reader.GetString(7) == "Bishop" ? PieceType.Bishop :
                                        reader.GetString(7) == "Rook" ? PieceType.Rook : PieceType.Queen,
                            Promotion = reader.IsDBNull(8) ? PieceType.None :
                                        reader.GetString(8) == "Queen" ? PieceType.Queen :
                                        reader.GetString(8) == "Knight" ? PieceType.Knight :
                                        reader.GetString(8) == "Bishop" ? PieceType.Bishop :
                                        reader.GetString(8) == "Rook" ? PieceType.Rook : PieceType.Pawn,
                            IsCheck = reader.GetBoolean(9),
                            IsCastle = reader.GetBoolean(10),
                            IsEnPassant = reader.GetBoolean(11)
                        });
                    }
                    // GameID[int]
                    // Turn[int]
                    // Color[nchar](5)
                    // Piece[nvarchar](6)
                    // Notation[nvarchar](10)
                    // Origin[nchar](2)
                    // Destination[nchar](2)
                    // Capture[nvarchar](6)
                    // Promotion[nvarchar](6)
                    // IsCheck[bit]
                    // IsMate[bit]
                    // IsCastle[bit]
                    // IsEnPassant[bit]
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return moves;
        }

        public int UpdateGame(Game oldGame, Game newGame)
        {
            int rowsAffected = 0;

            var conn = DBConnection.GetConnection();
            var cmdText = "sp_update_game";
            /*
                @GameID[int],
	            @OldPlayerWhite[nvarchar](20),
	            @OldWhiteElo[int],
	            @OldPlayerBlack[nvarchar](20),
	            @OldBlackElo[int],
	            @OldECO[nchar](3),
	            @OldOpeningVariation[nvarchar](50),
	            @OldTermination[nvarchar](20),
	            @OldOutcome[nvarchar](7),
	            @OldTimeControl[nvarchar](10),
	            @OldDatePlayed[date],
	
	            @NewPlayerWhite[nvarchar](20),
	            @NewWhiteElo[int],
	            @NewPlayerBlack[nvarchar](20),
	            @NewBlackElo[int],
	            @NewECO[nchar](3),
	            @NewOpeningVariation[nvarchar](50),
	            @NewTermination[nvarchar](20),
	            @NewOutcome[nvarchar](7),
	            @NewTimeControl[nvarchar](10),
	            @NewDatePlayed[date]
            */
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GameID", oldGame.GameID);
            cmd.Parameters.AddWithValue("@OldPlayerWhite", oldGame.PlayerWhite);
            cmd.Parameters.AddWithValue("@OldWhiteElo", oldGame.WhiteElo);
            cmd.Parameters.AddWithValue("@OldPlayerBlack", oldGame.PlayerBlack);
            cmd.Parameters.AddWithValue("@OldBlackElo", oldGame.BlackElo);
            cmd.Parameters.AddWithValue("@OldECO", oldGame.ECO);
            cmd.Parameters.AddWithValue("@OldOpeningVariation", oldGame.Opening);
            cmd.Parameters.AddWithValue("@OldTermination", oldGame.Termination);
            cmd.Parameters.AddWithValue("@OldOutcome", oldGame.Outcome);
            cmd.Parameters.AddWithValue("@OldTimeControl", oldGame.TimeControl);
            cmd.Parameters.AddWithValue("@OldDatePlayed", oldGame.DatePlayed);

            cmd.Parameters.Add("@NewPlayerWhite", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@NewWhiteElo", SqlDbType.Int);
            cmd.Parameters.Add("@NewPlayerBlack", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@NewBlackElo", SqlDbType.Int);
            cmd.Parameters.Add("@NewECO", SqlDbType.NChar, 3);
            cmd.Parameters.Add("@NewOpeningVariation", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@NewTermination", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@NewOutcome", SqlDbType.NVarChar, 7);
            cmd.Parameters.Add("@NewTimeControl", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@NewDatePlayed", SqlDbType.Date);

            cmd.Parameters["@NewPlayerWhite"].Value = newGame.PlayerWhite;
            cmd.Parameters["@NewWhiteElo"].Value = newGame.WhiteElo;
            cmd.Parameters["@NewPlayerBlack"].Value = newGame.PlayerBlack;
            cmd.Parameters["@NewBlackElo"].Value = newGame.BlackElo;
            cmd.Parameters["@NewECO"].Value = newGame.ECO;
            cmd.Parameters["@NewOpeningVariation"].Value = newGame.Opening;
            cmd.Parameters["@NewTermination"].Value = newGame.Termination;
            cmd.Parameters["@NewOutcome"].Value = newGame.Outcome;
            cmd.Parameters["@NewTimeControl"].Value = newGame.TimeControl;
            cmd.Parameters["@NewDatePlayed"].Value = newGame.DatePlayed;

            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rowsAffected;
        }
    }
}