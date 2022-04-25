using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using DataAccessInterfaces;
using DataObjects;

namespace DataAccessLayer
{
    public class UserAccessor : IUserAccessor
    {
        public int AuthenticateUserWithUserNameAndPasswordHash(string userName, string passwordHash)
        {
            int result = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            var cmdText = "sp_authenticate_user";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);

            // set arguments
            cmd.Parameters["@UserID"].Value = userName;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;

            // open connection and execute command
            try
            {
                conn.Open();
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        public int UpdatePasswordHash(string userName, string oldPasswordHash, string newPasswordHash)
        {
            int rowsAffected = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_update_passwordHash";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@OldPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 100);

            // set arguments
            cmd.Parameters["@UserID"].Value = userName;
            cmd.Parameters["@OldPasswordHash"].Value = oldPasswordHash;
            cmd.Parameters["@NewPasswordHash"].Value = newPasswordHash;

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

        public User SelectUserByUserName(string userName)
        {
            User user = null;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_select_user_by_userID";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);

            // add arguments
            cmd.Parameters["@UserID"].Value = userName;

            try
            {
                // open connection and create reader
                conn.Open();
                var reader = cmd.ExecuteReader();

                // process results
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        user = new User()
                        {
                            UserName = reader.GetString(0),
                            GivenName = reader.GetString(1),
                            FamilyName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Active = reader.GetBoolean(4)
                        };
                    }
                }
                else
                {
                    throw new ApplicationException("User not found.");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return user;
        }

        public List<string> SelectRolesByUserName(string userName)
        {
            List<string> roles = new List<string>();

            // set up sql command
            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_user_roles_by_userID";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // create parameter and add argument
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters["@UserID"].Value = userName;

            try
            {
                // open connection and create reader
                conn.Open();
                var reader = cmd.ExecuteReader();

                // process results
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        roles.Add(reader.GetString(0));
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

            return roles;
        }

        public List<Game> SelectFavoritesByUserName(string userName)
        {
            List<Game> games = new List<Game>();

            // set up sql command
            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_favorite_games_by_userID";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // create parameter and add argument
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters["@UserID"].Value = userName;

            try
            {
                // open connection and create reader
                conn.Open();
                var reader = cmd.ExecuteReader();

                // process results
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        games.Add(new Game()
                        {
                            // GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name
                            // OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
                            GameID = reader.GetInt32(0),
                            PlayerWhite = reader.GetString(1),
                            WhiteElo = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            PlayerBlack = reader.GetString(3),
                            BlackElo = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            ECO = reader.GetString(5),
                            Opening = reader.GetString(7),
                            //Opening = new Opening()
                            //{
                            //    ECO = reader.GetString(5),
                            //    Name = reader.GetString(6),
                            //    Variation = reader.GetString(7)
                            //},
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

        public int InsertUserFavoriteGame(string userName, int gameID)
        {
            int rowsAffected = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_insert_user_favorite_game";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@GameID", SqlDbType.Int);

            // set arguments
            cmd.Parameters["@UserID"].Value = userName;
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

        public int RemoveUserFavoriteGame(string userName, int gameID)
        {
            int rowsAffected = 0;

            // set up sql command
            var conn = DBConnection.GetConnection();
            string cmdText = @"sp_delete_user_favorite_game";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add parameters
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@GameID", SqlDbType.Int);

            // set arguments
            cmd.Parameters["@UserID"].Value = userName;
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

        public List<Game> SelectFavoritesByUserNameAndECO(string userName, string eco)
        {
            List<Game> games = new List<Game>();

            // set up sql command
            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_select_favorite_games_by_userID_and_ECO";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // create parameter and add argument
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@ECO", SqlDbType.NChar, 3);

            cmd.Parameters["@UserID"].Value = userName;
            cmd.Parameters["@ECO"].Value = eco;

            try
            {
                // open connection and create reader
                conn.Open();
                var reader = cmd.ExecuteReader();

                // process results
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        games.Add(new Game()
                        {
                            // GameID, PlayerWhite, WhiteElo, PlayerBlack, BlackElo, [Game].ECO, [Opening].Name
                            // OpeningVariation, Termination, Outcome, TimeControl, DatePlayed
                            GameID = reader.GetInt32(0),
                            PlayerWhite = reader.GetString(1),
                            WhiteElo = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            PlayerBlack = reader.GetString(3),
                            BlackElo = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            ECO = reader.GetString(5),
                            Opening = reader.GetString(7),
                            //Opening = new Opening()
                            //{
                            //    ECO = reader.GetString(5),
                            //    Name = reader.GetString(6),
                            //    Variation = reader.GetString(7)
                            //},
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
    }
}
