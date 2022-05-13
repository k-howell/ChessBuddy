using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataObjects;
using LogicLayer;
using MVCPresentation.Models;

namespace MVCPresentation.Controllers
{
    public class GameController : Controller
    {
        IGameManager _gameManager = new GameManager();
        IBoardManager _boardManager = new BoardManager();
        IUserManager _userManager = new UserManager();

        // GET: Game
        public ActionResult Details(int gameID, int turn = 0, string side = "White")
        {
            GameBoardViewModel model = new GameBoardViewModel();
            model.Turn = turn;
            model.Side = side;

            try
            {
                Game game = _gameManager.RetrieveGame(gameID);
                game.Moves = _gameManager.GetMovesForGame(game);
                _boardManager.LoadGame(game);
                if (side != _boardManager.GetBoard().Color.ToString())
                {
                    _boardManager.FlipBoard();
                }
                ViewBag.Turn = _boardManager.SetTurn(turn);

                model.Board = _boardManager.GetBoard();
                model.Game = game;
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "There was a problem finding the game.";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SetTurn(int gameID, int turn, string side)
        {
            GameBoardViewModel model = new GameBoardViewModel();

            try
            {
                Game game = _gameManager.RetrieveGame(gameID);
                game.Moves = _gameManager.GetMovesForGame(game);
                _boardManager.LoadGame(game);
                if (side != _boardManager.GetBoard().Color.ToString())
                {
                    _boardManager.FlipBoard();
                }

                model.Turn = _boardManager.SetTurn(turn);
                
                model.Side = side;
                model.Board = _boardManager.GetBoard();
                model.Game = game;
            }
            catch(Exception ex)
            {
                TempData["errorMsg"] = ex.Message;
            }

            // return Json(new { success = true, model = model }, JsonRequestBehavior.DenyGet);
            return PartialView("_BoardPartial", model);
        }

        [HttpPost]
        [Authorize]
        public void AddFavorite(int gameID)
        {
            try
            {
                _userManager.AddUserFavorite(User.Identity.Name, gameID);
            }
            catch(Exception ex)
            {
                TempData["errorMsg"] = ex.Message;
            }
        }

        // GET
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int gameID)
        {
            Game model = null;
            try
            {
                model = _gameManager.RetrieveGame(gameID);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Game newGame)
        {
            try
            {
                Game oldGame = _gameManager.RetrieveGame(newGame.GameID);
                _gameManager.UpdateGame(oldGame, newGame);
            }
            catch(Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            // return RedirectToAction("Details", new { gameID = newGame.GameID });
            return View(newGame);
        }
    }
}