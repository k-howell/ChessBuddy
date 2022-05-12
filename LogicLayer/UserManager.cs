using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using DataAccessInterfaces;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class UserManager : IUserManager
    {
        private IUserAccessor _userAccessor = null;

        public UserManager()
        {
            _userAccessor = new UserAccessor();
        }

        public UserManager(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public User GetUserByUserName(string userName)
        {
            User user;

            try
            {
                user = _userAccessor.SelectUserByUserName(userName);
            }
            catch (Exception)
            {
                throw;
            }

            return user;
        }

        public User AuthenticateUser(string userName, string password)
        {
            User authenticatedUser = null;
            string passwordHash = HashSha256(password);

            try
            {
                if(1 == _userAccessor.AuthenticateUserWithUserNameAndPasswordHash(userName, passwordHash)) {
                    authenticatedUser = GetUserByUserName(userName);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return authenticatedUser;
        }

        public bool ResetPassword(string userName, string oldPassword, string newPassword)
        {
            bool isUpdated;

            try
            {
                isUpdated = 1 == _userAccessor.UpdatePasswordHash(userName,
                                                                  HashSha256(oldPassword),
                                                                  HashSha256(newPassword));

                if(!isUpdated)
                {
                    throw new ApplicationException("Update failed.");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return isUpdated;
        }

        public List<string> GetUserRoles(string userName)
        {
            List<string> roles;

            try
            {
                roles = _userAccessor.SelectRolesByUserName(userName);
            }
            catch (Exception)
            {
                throw;
            }

            return roles;
        }

        public List<string> GetAllUserRoles()
        {
            List<string> roles;

            try
            {
                roles = _userAccessor.SelectAllRoles();
            }
            catch (Exception)
            {
                throw;
            }

            return roles;
        }

        public string HashSha256(string source)
        {
            string result = "";
            // byte array to hold hashed data
            byte[] data;
            // create sha256 hasher object
            using (SHA256 sha256Hasher = SHA256.Create())
            {
                // hash input
                data = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(source));
            }

            // create stringbuilder to create string from byte array
            var stringBuilder = new StringBuilder();

            // create characters from bytes and append them to string
            for(int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            result = stringBuilder.ToString();

            return result;
        }

        public User LoginUser(string userName, string password)
        {
            User loggedIn;

            try
            {
                if(userName == "")
                {
                    throw new ApplicationException("Missing username.");
                }
                if(password == "")
                {
                    throw new ApplicationException("Missing password.");
                }

                // password = HashSha256(password);
                if(AuthenticateUser(userName, password) != null)
                {
                    loggedIn = GetUserByUserName(userName);
                    loggedIn.Roles = GetUserRoles(userName);
                }
                else
                {
                    throw new ApplicationException("Incorrect username or password.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Login failed. Please check your login credentials.", ex);
            }

            return loggedIn;
        }

        public List<Game> GetUserFavorites(string userName)
        {
            List<Game> games;

            try
            {
                games = _userAccessor.SelectFavoritesByUserName(userName);
            }
            catch (Exception)
            {
                throw;
            }

            return games;
        }

        public int AddUserFavorite(string userName, int gameID)
        {
            int rowsAffected;

            try
            {
                rowsAffected = _userAccessor.InsertUserFavoriteGame(userName, gameID);
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected;
        }

        public int RemoveUserFavorite(string userName, int gameID)
        {
            int rowsAffected;

            try
            {
                rowsAffected = _userAccessor.RemoveUserFavoriteGame(userName, gameID);
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected;
        }

        public List<Game> GetUserFavoritesByECO(string userName, string eco)
        {
            List<Game> games;

            try
            {
                games = _userAccessor.SelectFavoritesByUserNameAndECO(userName, eco);
            }
            catch (Exception)
            {
                throw;
            }

            return games;
        }
    }
}
