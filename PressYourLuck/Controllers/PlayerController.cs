using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using PressYourLuck.Models;
using PressYourLuck.Helpers;
using Microsoft.EntityFrameworkCore;

namespace PressYourLuck.Controllers
{
    public class PlayerController : Controller
    {
        private readonly AuditContext _auditContext;
        
        public PlayerController(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        [HttpGet("Player")]
        public IActionResult Index()
        {
           
            return View("Index");
        }

        // If information passes validation, stores name and totalCoins into cookies
        // Then passes controller back to the Home/Index page
        public IActionResult AddPlayer(Player player, Audit audit)
        {
            if (ModelState.IsValid)
            {
                player.Name = this.Request.Query["name"];
                string totalCoinsStr = Request.Query["totalCoins"];
                player.TotalCoins = double.Parse(totalCoinsStr);

                var options = new CookieOptions { Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("name", player.Name, options);
                Response.Cookies.Append("totalCoins", totalCoinsStr);
                CoinsHelper.SaveTotalCoins(HttpContext, player.TotalCoins);
                
                HttpContext.Session.SetString("name", player.Name);

                audit = new Audit { PlayerName = player.Name, CreatedDate = DateTime.Now, Amount = player.TotalCoins, AuditTypeId = 1 };
                _auditContext.Audits.Add(audit);
                _auditContext.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Index");
            }
        }
        
        // Clear the cookies and send the user back to enter a new player
        public IActionResult CashOut(Audit audit)
        {
            if (CoinsHelper.GetTotalCoins(HttpContext) == 0)
            {
                TempData["message"] = $"You've lost all your coins and must enter more to keep playing!";
                Response.Cookies.Delete("name");
                //HttpContext.Session.Remove("name");
                Response.Cookies.Delete("totalCoins");
                HttpContext.Session.Remove("totalCoins");

                audit = new Audit { PlayerName = HttpContext.Request.Cookies["name"], CreatedDate = DateTime.Now, Amount = CoinsHelper.GetOriginalBet(HttpContext), AuditTypeId = 4 };
                _auditContext.Audits.Add(audit);
                _auditContext.SaveChanges();

                return View("Index");
            }
            else
            {
                string name = HttpContext.Request.Cookies["name"];
                //string totalCoins = HttpContext.Request.Cookies["totalCoins"];
                string totalCoins = CoinsHelper.GetTotalCoins(HttpContext).ToString();
                TempData["message"] = $"You cashed out for {totalCoins} coins";
                if (name != null)
                {
                    Response.Cookies.Delete("name");
                    HttpContext.Session.Remove("name");
                }
                if (totalCoins != null)
                {
                    Response.Cookies.Delete("totalCoins");
                    HttpContext.Session.Remove("totalCoins");
                }

                audit = new Audit { PlayerName = name, CreatedDate = DateTime.Now, Amount = double.Parse(totalCoins), AuditTypeId = 2 };
                _auditContext.Audits.Add(audit);
                _auditContext.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
