using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PressYourLuck.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PressYourLuck.Models;
using Newtonsoft.Json;

namespace PressYourLuck.Controllers
{
    public class GameController : Controller
    {
        private readonly AuditContext _auditContext;

        public GameController(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        // If the list of tiles isn't in session, generate a new game and store in seesion,
        // otherwise, deserialize the information that's there into a List of Tile
        public IActionResult Index()
        {
            int? gameStarted = HttpContext.Session.GetInt32("gameStarted");
            if (gameStarted == null)
            {
                var tileList = GameHelper.GenerateNewGame();
                TileListViewModel currentResult = new TileListViewModel { Tiles = tileList, CurrentBet = 0};
                string resultJson = JsonConvert.SerializeObject(currentResult);
                HttpContext.Session.SetString("currentResult", resultJson);
                HttpContext.Session.SetInt32("gameStarted", 1);
                return View(tileList);
            }
            else
            {
                string totalCoins = HttpContext.Session.GetString("totalCoins");
                CoinsHelper.SaveTotalCoins(HttpContext, double.Parse(totalCoins));
                string resultJson = HttpContext.Session.GetString("currentResult");
                TileListViewModel currentResult = JsonConvert.DeserializeObject<TileListViewModel>(resultJson);
                return View(currentResult.Tiles);
            } 
        }

        // GET method for GetBet for current-bet
        // Doing validation to check the amount of bet must be greater than 0
        [HttpGet("/GetBet")]
        public IActionResult GetBet()
        {
            double coins = CoinsHelper.GetTotalCoins(HttpContext);
            string bet = this.Request.Query["current-bet"];
           
            if (double.Parse(bet) <= 0)
            {
                TempData["message"] = $"The amount of the bet must be greater than 0!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                CoinsHelper.SaveCurrentBet(HttpContext, double.Parse(bet));
                double leftCoins = coins - double.Parse(bet);
                CoinsHelper.SaveOriginalBet(HttpContext, double.Parse(bet));

                CoinsHelper.SaveTotalCoins(HttpContext, leftCoins);
                HttpContext.Session.SetString("totalCoins", leftCoins.ToString());

                return RedirectToAction("Index", "Game");
            }
        }

        // Reveal action for keeping track of each tile.
        // If the value is 0, the player has lost. Setting the current bet to 0, then reveal all tiles.
        // If the value is not 0, multiple the current bet by that amount.
        public IActionResult Reveal(int id, Audit audit)
        {
            if (CoinsHelper.GetTotalCoins(HttpContext) == 0)
            {
                return RedirectToAction("CashOut", "Player");
            }
            else 
            {
                string resultJson = HttpContext.Session.GetString("currentResult");
                TileListViewModel currentResult = JsonConvert.DeserializeObject<TileListViewModel>(resultJson);
                currentResult.Tiles[id].Visible = true;
                if (double.Parse(currentResult.Tiles[id].Value) == 0.00)
                {
                    TempData["message"] = $"Oh no! You busted out. Better luck next time!";
                    foreach (var tile in currentResult.Tiles)
                    {
                        HttpContext.Session.SetString("current-bet", "0.00");
                        tile.Visible = true;
                    }

                    audit = new Audit { PlayerName = HttpContext.Request.Cookies["name"], CreatedDate = DateTime.Now, Amount = CoinsHelper.GetOriginalBet(HttpContext), AuditTypeId = 4 };
                    _auditContext.Audits.Add(audit);
                    _auditContext.SaveChanges();
                }
                else
                {
                    double bet = CoinsHelper.GetCurrentBet(HttpContext);
                    double currentBet = double.Parse(currentResult.Tiles[id].Value) * bet;
                    TempData["message"] = $"Congrats you've found a {currentResult.Tiles[id].Value} multipler! " +
                        $"All remaining values have doubled. will you press your luck?";
                    CoinsHelper.SaveCurrentBet(HttpContext, currentBet);

                    audit = new Audit { PlayerName = HttpContext.Request.Cookies["name"], CreatedDate = DateTime.Now, Amount = currentBet, AuditTypeId = 3 };
                    _auditContext.Audits.Add(audit);
                    _auditContext.SaveChanges();

                    // double the remaining amounts that have not yet been chosen
                    foreach (var tile in currentResult.Tiles)
                    {
                        if (currentResult.Tiles[id] == tile || tile.Value == "0.00" || tile.Visible == true)
                        {
                            tile.Value = tile.Value;
                        }
                        else
                        {
                            tile.Value = (double.Parse(tile.Value) * 2).ToString();
                        }
                    }
                }
                resultJson = JsonConvert.SerializeObject(currentResult);
                HttpContext.Session.SetString("currentResult", resultJson);
                return RedirectToAction("Index", "Game"); 
            }
        }

        // Add the amount of the current bet to their total coins, then take the user back to make another wager
        public IActionResult TakeCoins()
        {
            double totalCoins = CoinsHelper.GetTotalCoins(HttpContext) + CoinsHelper.GetCurrentBet(HttpContext);
            double currentCoins = CoinsHelper.GetCurrentBet(HttpContext);
            TempData["message"] = $"BIG WINNER! You chased out for {currentCoins} coins! Care to press your luck again?";
            CoinsHelper.SaveTotalCoins(HttpContext, totalCoins);
           
            return RedirectToAction("Index", "Home");
        }
    }
}
