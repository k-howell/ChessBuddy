using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessInterfaces
{
    public interface IGameAccessor
    {
        Game SelectGame(int gameID);
        List<Game> SelectGamesByECO(string eco);
        List<Game> SelectAllGames();
        List<Move> SelectMovesByGameID(int gameID);
        List<Opening> SelectAllOpenings();
        int UpdateGame(Game oldGame, Game newGame); // needs all old/new fields
        int InsertGame();
        int InsertVariation(string eco, string name);
        int DeleteGame(int gameID);
    }
}
