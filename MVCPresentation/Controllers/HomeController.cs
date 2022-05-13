﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LogicLayer;
using DataObjects;
using MVCPresentation.Models;

namespace MVCPresentation.Controllers
{
    public class HomeController : Controller
    {
        IGameManager _gameManager = new GameManager();
        IUserManager _userManager = new LogicLayer.UserManager();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Games()
        {
            ViewBag.Message = "Your page to browse chess games.";
            IEnumerable<Game> model = _gameManager.RetrieveAllGames();


            return View(model);
        }

        [Authorize]
        public ActionResult Favorites()
        {
            ViewBag.Message = "Your page to browse your favorited chess games.";
            IEnumerable<Game> model = _userManager.GetUserFavorites(User.Identity.GetUserName());

            return View(model);
        }

        [Authorize]
        public ActionResult RemoveFavorite(int gameID)
        {
            try
            {
                _userManager.RemoveUserFavorite(User.Identity.Name, gameID);
            }
            catch (Exception ex)
            {
                TempData["errorMsg"] = ex.Message;
            }

            return RedirectToAction("Favorites", "Home");
        }
    }
}