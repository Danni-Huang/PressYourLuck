using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PressYourLuck.Helpers
{
    public static class CoinsHelper
    {
        /*
         * Consider using this helper to Get and Set the Current Bet and the original bet
         * (both in session variables), as well as adding a Get and Set for the player's
         * total number of coins (which we'll store in Cookies)
         * 
         * HINT: Remember that HttpContext as well as Response and Request objects are not
         * available from here, so you may need to pass those in from your controller.
         * 
         * I've coded the first one for you and have created placeholders for the rest.
         * 
         */
        public static void SaveCurrentBet(HttpContext httpContext, double bet)
        {
            httpContext.Session.SetString("current-bet", bet.ToString("N2"));
        }


        //Return the current bet stored in session
        public static double GetCurrentBet(/*parameters?*/HttpContext httpContext)
        {

            //Code goes here
            double currentBet = double.Parse(httpContext.Session.GetString("current-bet"));
            return currentBet;
        }

        //Save the original bet into session
        public static void SaveOriginalBet(/*parameters?*/HttpContext httpContext, double bet)
        {
            //Code goes here
            httpContext.Session.SetString("original-bet", bet.ToString("N2"));
        }

        //Get the original bet from session
        public static double GetOriginalBet(/*parameters?*/HttpContext httpContext)
        {
            //Code goes here
            double originalBet;
            double.TryParse(httpContext.Session.GetString("original-bet"), out originalBet);
            return originalBet;
            
        }

        //Save the players total number of coins into a cookie.  Don't forget to
        // persist the cookie (Chapter 9!)
        public static void SaveTotalCoins(/*parameters?*/HttpContext httpContext, double coins)
        {
            //Code goes here
            var options = new CookieOptions { Expires = DateTime.Now.AddDays(30) };
            httpContext.Response.Cookies.Append("totalCoins", coins.ToString("N2"), options);
        }

        //Get the players total number of coins from a cookie.
        public static double GetTotalCoins(/*parameters?*/HttpContext httpContext)
        {
            //Code goes here
            double totalCoins; 
            double.TryParse(httpContext.Request.Cookies["totalCoins"], out totalCoins);

            return totalCoins;
        }

        // Save the selected audit type into seesion
        public static void SaveSelectAuditType(HttpContext httpContext, string type)
        {
            httpContext.Session.SetString("audit-type", type);
        }

        // Get the selected audit type from seesion
        public static string GetSelectAuditType(HttpContext httpContext)
        {
            string auditType = httpContext.Session.GetString("audit-type");
            return auditType;
        }
    }
}
