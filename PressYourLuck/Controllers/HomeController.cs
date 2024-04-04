using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PressYourLuck.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PressYourLuck.Helpers;
using Microsoft.EntityFrameworkCore;

namespace PressYourLuck.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Default landing page, if the player name cookie is empty, 
        // force the use to go the player controller to enter in new information
        public IActionResult Index()
        {
            // clear the seesion before a new game start
            HttpContext.Session.Remove("currentResult");
            HttpContext.Session.Remove("gameStarted");

            string name = Request.Cookies["name"];
            string totalCoins = Request.Cookies["totalCoins"];
            if (name == null || totalCoins == null)
            {
                return RedirectToAction("Index", "Player");
            }
            else
            {
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("totalCoins", totalCoins);
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
