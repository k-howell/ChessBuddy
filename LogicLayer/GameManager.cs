using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessInterfaces;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class GameManager : IGameManager
    {
        private IGameAccessor _gameAccessor = null;

        public GameManager()
        {
            _gameAccessor = new GameAccessor();
        }

        public GameManager(IGameAccessor gameAccessor)
        {
            _gameAccessor = gameAccessor;
        }

        public List<Game> RetrieveAllGames()
        {
            List<Game> games;

            try
            {
                games = _gameAccessor.SelectAllGames();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return games;
        }

        public List<Move> GetMovesForGame(Game game)
        {
            List<Move> moves = null;

            try
            {
                moves = _gameAccessor.SelectMovesByGameID(game.GameID);
            }
            catch (Exception)
            {
                throw;
            }

            return moves;
        }

        public int RemoveGame(Game game)
        {
            int rowsAffected;

            try
            {
                rowsAffected = _gameAccessor.DeleteGame(game.GameID);
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected;
        }

        public List<Opening> RetrieveAllOpenings()
        {
            List<Opening> openings;

            try
            {
                openings = _gameAccessor.SelectAllOpenings();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return openings;
        }

        public int UpdateGame(Game oldGame, Game newGame)
        {
            int rowsAffected;

            try
            {
                _gameAccessor.InsertVariation(newGame.ECO, newGame.Opening);
            }
            catch (Exception)
            {
                // try insert variation.  if duplicate, do nothing
            }

            try
            {
                rowsAffected = _gameAccessor.UpdateGame(oldGame, newGame);
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected;
        }

        public List<Game> RetrieveGamesByECO(string eco)
        {
            List<Game> games;

            try
            {
                games = _gameAccessor.SelectGamesByECO(eco);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return games;
        }
    }
}
