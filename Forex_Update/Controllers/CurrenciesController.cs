using Forex_Update.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Forex_Update.Controllers
{
    [Authorize]
    public class CurrenciesController : Controller
    {
        Entities db = new Entities();   
        // GET: Currencies
        public ActionResult Index()
        {
            int currentuserid = User.Identity.GetUserId<int>();

            StockViewModel stonkss = new StockViewModel();

            List<Currency> currencies = db.Currencies.ToList();
            

            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);
            string symbol = customer.Currency;

            string name = currencies
                 .Where(x => x.CurrencySymbol == symbol)
                 .Select(x => x.Name)
                 .FirstOrDefault(); // This will get you the first name that matches the symbol, or null if none do





            ViewBag.currency = symbol;
            ViewBag.currencyname = name;
          
            ViewBag.firstname = customer.FirstName;
     



            stonkss.CurrenciesList = currencies;
          

            db.SaveChanges();


            return View(stonkss);
        }

        [HttpPost]
        public async Task<int> sellcurrency(string symbol, decimal quantity)
        {
            int userid = User.Identity.GetUserId<int>();
            TotalShare total = db.TotalShares.FirstOrDefault(x => x.Symbol == symbol && x.UserId == userid);
            if (quantity <= total.TotalVolume)
            {

                try
                {

                    Trading newobj = new Trading();

                    Currency currency = db.Currencies.FirstOrDefault(x => x.CurrencySymbol == symbol);

                    decimal value = await getcurrency(symbol);
                    var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
                    userdetail.UserBallance = userdetail.UserBallance + (quantity * value);



                    newobj.UserId = userid;
                    newobj.Type = "Sell";
                    newobj.Openprice = value;

                    newobj.CurrentPrice = value;
                    newobj.Name = currency.Name;
                    newobj.Symbol = symbol;
                    newobj.BuyTime = DateTime.Now;
                    newobj.SharePart = quantity;
                    newobj.TotalPayedAmount = userdetail.UserBallance + (quantity * value);
                    newobj.Active = false;
                    newobj.Obj = "currency";

                    decimal pass = quantity * value;

                    await decreasecurrency(symbol, quantity, pass);

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
        public bool selectcurrency(string cryptoSymbol)
        {

            int currentuserid = User.Identity.GetUserId<int>();

            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);

            AspNetUser user = db.AspNetUsers.Find(User.Identity.GetUserId<int>());

            try
            {
                Currency getcurrency = db.Currencies.FirstOrDefault(x => x.CurrencySymbol.Contains(cryptoSymbol));
                user.Currency = getcurrency.CurrencySymbol;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<decimal> getcurrency(string symbol)
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
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.polygon.io/v2/aggs/ticker/C:" + symbol.ToUpper() + "/range/1/minute/" + fromdate + "/" + formattedDate + "?adjusted=true&sort=desc&limit=1&apiKey=" + polygonApiKey);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(responseBody);

            // Extract the "c" value
            decimal stockValue = (decimal)data["results"][0]["c"];



            return stockValue;
        }


        public async Task<string> maketransaction(string symbol, decimal quantity)
        {
            int currentuserid = User.Identity.GetUserId<int>();
            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);

            decimal callfunc = await getcurrency(symbol);


            if (quantity * callfunc <= userdetail.UserBallance)
            {

                try
                {


                    Currency currency = db.Currencies.FirstOrDefault(x => x.CurrencySymbol == symbol);

                    Trading tradings = new Trading();
                    tradings.SharePart = quantity;
                    tradings.Openprice = callfunc;
                    tradings.UserId = User.Identity.GetUserId<int>();
                    tradings.Type = "Buy";
                    tradings.Obj = "currency";
                    tradings.TotalPayedAmount = Math.Round(quantity * callfunc, 4);
                    tradings.Active = true;
                    //tradings.currentprice = Convert.ToDouble(callfunc);
                    tradings.Name = currency.CurrencySymbol;
                    tradings.Symbol = symbol;
                    tradings.BuyTime = DateTime.Now;

                    userdetail.UserBallance = userdetail.UserBallance - (tradings.Openprice * quantity);

                    var checker = db.TotalShares.Where(x => x.UserId == currentuserid && x.Symbol.Equals(symbol));
                    if (checker.Count() >= 1)
                    {
                        await updatetotalcurrency(symbol, quantity, callfunc);

                    }
                    else
                    {
                        await addtotalcurrency(symbol, quantity, callfunc);
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


        public JsonResult getEntirecurrency(string currency)
        {
            int userid = User.Identity.GetUserId<int>();

           

            try
            {
                // Check if there are any shares for the user and stock
                if (!db.TotalShares.Any(x => x.UserId == userid && x.Symbol == currency))
                {
                    return Json(0, JsonRequestBehavior.AllowGet); // Return 0 if no shares
                }

                // Calculate the sum of the TotalVolume field
                decimal totalVolume = db.TotalShares
                                        .Where(x => x.UserId == userid && x.Symbol == currency)
                                        .Sum(x => x.TotalVolume);

                return Json(totalVolume, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<string> addtotalcurrency(string symbol, decimal quantity, decimal price)
        {
            TotalShare ts = new TotalShare();
            int userid = User.Identity.GetUserId<int>();

            Currency currency = db.Currencies.FirstOrDefault(x => x.CurrencySymbol == symbol);

            ts.Symbol = symbol;
            ts.TotalVolume = quantity;
            ts.name = currency.Name;
            ts.UserId = userid;
            ts.TotalPayedAmount = price * quantity;
            ts.Obj = "currency";



            db.TotalShares.Add(ts);
            db.SaveChanges();

            return "Saved";
        }


        public async Task<string> updatetotalcurrency(string symbol, decimal quantity, decimal price)
        {
            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.FirstOrDefault(x => x.UserId == userid && x.Symbol.Equals(symbol));

            if (obj.Symbol.Equals(symbol) && obj.UserId.Equals(userid))
            {
                obj.TotalVolume = obj.TotalVolume + quantity;
                obj.TotalPayedAmount += price * quantity;
                db.SaveChanges();
            }
            return "Updated";
        }


        public async Task<decimal> decreasecurrency(string symbol, decimal volume, decimal price)
        {
            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.FirstOrDefault(x => x.UserId == userid && x.Symbol.Equals(symbol));
            obj.TotalVolume = obj.TotalVolume - volume;
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

            return 1;
        }

        [HttpPost]
        public JsonResult fillTableWithData(int itemId)
        {
            var data = db.Tradings.Where(x => x.Id.Equals(itemId)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getcurrencylist()
        {


            List<Currency> myList = new List<Currency>();
            myList = db.Currencies.ToList();
            return Json(myList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult gettradlists()
        {
            int userid = User.Identity.GetUserId<int>();
            var currentuser = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<Trading> myList = db.Tradings.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid) && x.Symbol.Equals(currentuser.Currency) && x.Obj.Equals("currency")).Reverse();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getAllcurrencytradlists()
        {
            int userid = User.Identity.GetUserId<int>();
            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<Trading> myList = db.Tradings.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid)).Reverse();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<double> getballance()
        {


            int currentuserid = User.Identity.GetUserId<int>();

            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);
            double ballance = Convert.ToDouble(userdetail.UserBallance);

            return Math.Round(ballance, 2);
        }

        [HttpPost]
        public async Task<JsonResult> checkProfitBeforesell(int itemId)
        {
            var obj = db.Tradings.FirstOrDefault(x => x.Id.Equals(itemId));
            var quantity = obj.SharePart;
            var spendvalue = obj.TotalPayedAmount;
            decimal livevalue = await getcurrency(obj.Symbol);

            //live value of current currency
            decimal total = livevalue * quantity;
            //profit 
            decimal profit = total - obj.TotalPayedAmount;


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
    }
}
