using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataObjects;
using LogicLayer;

namespace MVCPresentation.Controllers
{
    public class GameController : Controller
    {
        IGameManager gameManager = new GameManager();
        IBoardManager boardManager = new BoardManager();

        // GET: Game
        public ActionResult Details(int gameID, int turn = 0)
        {
            ViewBag.GameID = gameID;
            ViewBag.Turn = turn;

            try
            {
                Game game = gameManager.RetrieveGame(gameID);
                game.Moves = gameManager.GetMovesForGame(game);
                boardManager.LoadGame(game);
                ViewBag.Turn = boardManager.SetTurn(turn);
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "There was a problem finding the game.";
            }

            return View(boardManager);
        }
    }
}