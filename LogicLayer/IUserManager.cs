using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IUserManager
    {
        User LoginUser(string userName, string password);
        User AuthenticateUser(string userName, string passwordHash);
        string HashSha256(string source);
        User GetUserByUserName(string userName);
        List<string> GetUserRoles(string userName);
        List<Game> GetUserFavorites(string userName);
        List<Game> GetUserFavoritesByECO(string userName, string eco);
        bool ResetPassword(string userName, string oldPassword, string newPassword);
        int AddUserFavorite(string userName, int gameID);
        int RemoveUserFavorite(string userName, int gameID);
        List<string> GetAllUserRoles();
    }
}
