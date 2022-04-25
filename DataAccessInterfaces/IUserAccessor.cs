using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessInterfaces
{
    public interface IUserAccessor
    {
        int AuthenticateUserWithUserNameAndPasswordHash(string userName, string passwordHash);
        int UpdatePasswordHash(string userName, string oldPasswordHash, string newPasswordHash);
        User SelectUserByUserName(string userName);
        List<string> SelectRolesByUserName(string userName);
        List<Game> SelectFavoritesByUserName(string userName);
        List<Game> SelectFavoritesByUserNameAndECO(string userName, string eco);
        int InsertUserFavoriteGame(string userName, int gameID);
        int RemoveUserFavoriteGame(string userName, int gameID);
    }
}
