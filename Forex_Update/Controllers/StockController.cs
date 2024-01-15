using Forex_Update.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Forex_Update.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        Entities db = new Entities();
       
        // GET: Stock
        public ActionResult Index()
        {
            int currentuserid = User.Identity.GetUserId<int>();

            StockViewModel stonkss = new StockViewModel();

            List<stock> listofstocks = db.stocks.ToList();
            List<StockSecond> listofsecondstocks = db.StockSeconds.ToList();

            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);
            string symbol = customer.CurrentStockSymbol;

            string name = listofsecondstocks
                 .Where(x => x.Symbol == symbol)
                 .Select(x => x.Name)
                 .FirstOrDefault(); // This will get you the first name that matches the symbol, or null if none do





            ViewBag.stock = symbol;
            ViewBag.stockname = name;
            //ViewBag.ccr = cryoto;
            ViewBag.firstname = customer.FirstName;
            ViewBag.chart = customer.Chart;


      
            stonkss.StocksList = listofstocks;
            stonkss.SecondStocksList = listofsecondstocks;
       
            db.SaveChanges();


            return View(stonkss);
        }

        [HttpGet]
        public JsonResult getStocklist()
        {


            List<stock> myList = new List<stock>();
            myList = db.stocks.ToList();
            return Json(myList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult gettradlists()
        {
            int userid = User.Identity.GetUserId<int>();
            var currentuser = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<Trading> myList = db.Tradings.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid) && x.Symbol.Equals(currentuser.CurrentStockSymbol) && x.Obj.Equals("stock")).Reverse();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getAllStocktradlists()
        {
            int userid = User.Identity.GetUserId<int>();
            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<Trading> myList = db.Tradings.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid)).Reverse();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getEntireStock(string stock)
        {
            int userid = User.Identity.GetUserId<int>();

            try
            {
                // Check if there are any shares for the user and stock
                if (!db.TotalShares.Any(x => x.UserId == userid && x.Symbol == stock))
                {
                    return Json(0, JsonRequestBehavior.AllowGet); // Return 0 if no shares
                }

                // Calculate the sum of the TotalVolume field
                decimal totalVolume = db.TotalShares
                                        .Where(x => x.UserId == userid && x.Symbol == stock)
                                        .Sum(x => x.TotalVolume);

                return Json(totalVolume, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpGet]
        public async Task<double> getballance()
        {


           int userid = User.Identity.GetUserId<int>();

            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            double ballance = Convert.ToDouble(userdetail.UserBallance);

            return Math.Round(ballance, 2);
        }
        [HttpPost]
        public bool selectstock(string cryptoSymbol)
        {
            int userid = User.Identity.GetUserId<int>();

            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);

            AspNetUser user = db.AspNetUsers.Find(userid);
            stock getstock = db.stocks.FirstOrDefault(x => x.Symbol.Contains(cryptoSymbol));
            user.CurrentStockSymbol = getstock.Symbol;

            try
            {
               
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
          
        }

        public async Task<string> maketransaction(string symbol, decimal quantity)
        {
            int userid = User.Identity.GetUserId<int>();
            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);

            decimal callfunc = await getstockprice(symbol);
            //decimal liveprice = callfunc;
            //decimal price = liveprice;
            if (quantity * callfunc <= userdetail.UserBallance)
            {
                try
                {


                    stock getstock = db.stocks.FirstOrDefault(x => x.Symbol == symbol);

                    Trading tradings = new Trading();
                    tradings.SharePart = quantity;
                    tradings.Openprice = callfunc;
                    tradings.UserId = userid;
                    tradings.Type = "Buy";
                    tradings.Obj = "stock";
                    tradings.TotalPayedAmount = Math.Round(quantity * callfunc, 4);
                    tradings.Active = true;
                    //tradings.currentprice = Convert.ToDouble(callfunc);
                    tradings.Name = getstock.Name;
                    tradings.Symbol = symbol;
                    tradings.BuyTime = DateTime.Now;

                    userdetail.UserBallance = userdetail.UserBallance - (tradings.Openprice * quantity);

                    var checker = db.TotalShares.Where(x => x.UserId == userid && x.Symbol.Equals(symbol));
                    if (checker.Count() >= 1)
                    {
                        await updatetotalstock(symbol, quantity, callfunc);

                    }
                    else
                    {
                        await addtotalstock(symbol, quantity, callfunc);
                    }

                    db.Tradings.Add(tradings);
                    db.SaveChanges();


                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);

                }

                return "yes";
            }

            return "not";
        }

        public async Task<string> addtotalstock(string symbol, decimal quantity, decimal price)
        {
            TotalShare ts = new TotalShare();
            int userid = User.Identity.GetUserId<int>();
            stock getstock = db.stocks.FirstOrDefault(x => x.Symbol == symbol);

            ts.Symbol = symbol;
            ts.TotalVolume = Math.Round(quantity, 2);
            ts.name = getstock.Name;
            ts.UserId = userid;
            ts.TotalPayedAmount = price * quantity;
            ts.Obj = "stock";



            db.TotalShares.Add(ts);
            db.SaveChanges();

            return "Saved";
        }


        public async Task<string> updatetotalstock(string symbol, decimal quantity, decimal price)
        {
            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.FirstOrDefault(x => x.UserId == userid && x.Symbol.Equals(symbol));

            if (obj.Symbol.Equals(symbol) && obj.UserId.Equals(userid))
            {
                obj.TotalVolume = Math.Round(obj.TotalVolume + quantity, 2);
                obj.TotalPayedAmount += price * quantity;
                db.SaveChanges();
            }
            return "Updated";
        }


        public async Task<string> decreasestock(string symbol, decimal volume, decimal price)
        {
            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.FirstOrDefault(x => x.UserId == userid && x.Symbol.Equals(symbol));
            obj.TotalVolume = Math.Round(obj.TotalVolume - volume, 2);
            obj.TotalPayedAmount -= price;

            if (obj.TotalVolume == 0)
            {

                db.TotalShares.Remove(obj);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "updated";
        }

        public async Task<decimal> getstockprice(string symbol)
        {

            string polygonApiKey = "VUomlSZX9l0AVmn08TsdSYDI3upYjXNT";

            DateTime currentDateTime = DateTime.Now;
            DateTime fromDatelive = DateTime.Now.AddDays(-3);

            // Format the current date and time using ToString with the "yyyy-MM-dd" format string.
            string formattedDate = currentDateTime.ToString("yyyy-MM-dd");
            string fromdate = fromDatelive.ToString("yyyy-MM-dd");

            // Print the formatted date.
            Console.WriteLine("Formatted Date: " + formattedDate);


            // Create an instance of HttpClient.
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.polygon.io/v2/aggs/ticker/" + symbol.ToUpper() + "/range/1/minute/" + fromdate + "/" + formattedDate + "?adjusted=true&sort=desc&limit=1&apiKey=" + polygonApiKey);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(responseBody);

            // Extract the "c" value
            decimal stockValue = (decimal)data["results"][0]["c"];



            return stockValue;
        }
        [HttpPost]
        public async Task<int> sellstock(string symbol,decimal quantity)
        {
            int userid = User.Identity.GetUserId<int>();
            TotalShare total = db.TotalShares.FirstOrDefault(x => x.Symbol == symbol && x.UserId == userid);
            if (quantity <= total.TotalVolume)
            {

                try
                {

                    Trading newobj = new Trading();

                    stock stock = db.stocks.FirstOrDefault(x => x.Symbol == symbol);

                    decimal value = await getstockprice(symbol);
                    var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
                    userdetail.UserBallance = userdetail.UserBallance + (quantity * value);



                    newobj.UserId = userid;
                    newobj.Type = "Sell";
                    newobj.Openprice = value;

                    newobj.CurrentPrice = value;
                    newobj.Name = stock.Name;
                    newobj.Symbol = symbol;
                    newobj.BuyTime = DateTime.Now;
                    newobj.SharePart = quantity;
                    newobj.TotalPayedAmount = userdetail.UserBallance + (quantity * value);
                    newobj.Active = false;
                    newobj.Obj = "stock";

                    decimal pass = quantity * value;

                    await decreasestock(symbol, quantity, pass);

                    db.Tradings.Add(newobj);
                    db.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return 0;
                }
            }
            return 0;
        }

        [HttpPost]
        public async Task<JsonResult> checkProfitBeforesell(int itemId)
        {
            Trading obj = db.Tradings.FirstOrDefault(x => x.Id.Equals(itemId));
            var quantity = obj.SharePart;
            var spendvalue = obj.TotalPayedAmount;
            decimal livevalue = await getstockprice(obj.Symbol);

            //live value of current stock
            var total = (livevalue) * quantity;
            //profit 
            decimal profit = (total - obj.TotalPayedAmount);


            decimal realprofit = Math.Round(profit, 4); // Round to 2 decimal places


            if (obj.Openprice <= livevalue)
            {
                obj.Profit = realprofit;
                db.SaveChanges();
            }
            else
            {
                obj.Profit = realprofit;
                db.SaveChanges();
            }
            var data = db.Tradings.Where(x => x.Id.Equals(itemId)).ToList();
            Console.WriteLine(data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult fillTableWithData(int itemId)
        {
            var data = db.Tradings.Where(x => x.Id.Equals(itemId)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<string> getcrypto(string symbol)
        {

            string polygonApiKey = "VUomlSZX9l0AVmn08TsdSYDI3upYjXNT";

            DateTime currentDateTime = DateTime.Now;
            DateTime fromDatelive = DateTime.Now;

            // Format the current date and time using ToString with the "yyyy-MM-dd" format string.
            string formattedDate = currentDateTime.ToString("yyyy-MM-dd");
            string fromdate = fromDatelive.ToString("yyyy-MM-dd");

            // Print the formatted date.
            Console.WriteLine("Formatted Date: " + formattedDate);


            // Create an instance of HttpClient.
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.polygon.io/v2/aggs/ticker/X:" + symbol.ToUpper() + "/range/1/minute/" + fromdate + "/" + formattedDate + "?adjusted=true&sort=desc&limit=1&apiKey=" + polygonApiKey);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(responseBody);

            // Extract the "c" value
            double stockValue = (double)data["results"][0]["c"];



            return stockValue.ToString();
        }
        public async Task<decimal> Moneyinstock()
        {
            int userid = User.Identity.GetUserId<int>();

            // Check if the user has any shares
            bool hasShares = db.TotalShares.Any(a => a.UserId.Equals(userid));

            decimal totalLivePrice = 0;

            if (hasShares)
            {
                // Calculate the total live price only if shares exist
                totalLivePrice = db.TotalShares
                                   .Where(a => a.UserId.Equals(userid))
                                   .Sum(a => a.LivePrice);
            }

            // Retrieve the user from the database
            var user = db.AspNetUsers.Find(userid);

            if (user != null)
            {
                try
                {
                    user.MoneyInStock = totalLivePrice; // This will be 0 if no shares

                    // Save changes to the database
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // Log the exception
                    // Consider rethrowing the exception or handling it appropriately
                    return 0; // Return 0 in case of an exception
                }
            }
            else
            {
                // Handle the case where user does not exist
                return 0;
            }

            return totalLivePrice; // Return the calculated total live price or 0
        }




        public async Task<double> EquityCalculator()
        {

            int userid = User.Identity.GetUserId<int>();
            AspNetUser user = db.AspNetUsers.Find(userid);
            user.Equity = user.UserBallance + user.MoneyInStock;

            db.SaveChanges();
            return Math.Round(Convert.ToDouble(user.Equity), 2);
        }

        [HttpGet]
        public bool checkforrefresh()
        {

            int userid = User.Identity.GetUserId<int>();
            AspNetUser user = db.AspNetUsers.Find(userid);
            if (user.Refresh.Equals(true))
            {
                return true;
            }
            else
                return false;

        }
        public async Task<double> totalprofit()
        {
            int userid = User.Identity.GetUserId<int>();
            decimal sum, overal = 0;

            var stocks = db.TotalShares.Where(a => a.UserId.Equals(userid)).ToList();
            for (int i = 0; i < stocks.Count(); i++)
            {
                sum = stocks[i].PL;
                overal += sum;
            }
            AspNetUser user = db.AspNetUsers.Find(userid);
            user.TotalProfit = overal;

            db.SaveChanges();

            return Math.Round((double)user.TotalProfit, 2);
        }


    }
}


