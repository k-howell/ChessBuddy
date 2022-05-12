using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IGameManager
    {
        List<Game> RetrieveAllGames();
        List<Game> RetrieveGamesByECO(string eco);
        List<Opening> RetrieveAllOpenings();
        List<Move> GetMovesForGame(Game game);
        int RemoveGame(Game game);
        int UpdateGame(Game oldGame, Game newGame);
        Game RetrieveGame(int gameID);
    }
}
