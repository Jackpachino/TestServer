using Forex_Update.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Forex_Update.Controllers
{
    [Authorize]
    public class CryptoController : Controller
    {
        private Entities db = new Entities();
        // GET: Crypto
        public ActionResult Index()
        {
            int currentuserid = User.Identity.GetUserId<int>();

            StockViewModel stonkss = new StockViewModel();


            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id== currentuserid);
            string symbol = customer.CurrentStockSymbol;
            string cryoto = customer.CurrentCryptoSymbol;

            ViewBag.crypto = cryoto + "T";
            ViewBag.ccr = cryoto;
            ViewBag.firstname = customer.FirstName;
            ViewBag.chart = customer.Chart;

            List<Crypto> listofCrypto = db.Cryptoes.ToList();
            stonkss.CryptoList = listofCrypto;


            return View(stonkss);
        }
        [HttpGet]
        public JsonResult fetchcrypto()
        {
            int userid = User.Identity.GetUserId<int>();

            try
            {
                // Calculate the sum of the TotalVolume field
                var totalVolume = db.Cryptoes.ToList();
                                        

                return Json(totalVolume, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<int> sellcrypto(string symbol,decimal quantity)
        {
            int userid = User.Identity.GetUserId<int>();
            TotalShare total = db.TotalShares.FirstOrDefault(x => x.Symbol == symbol && x.UserId == userid);
            if (quantity <= total.TotalVolume)
            {

                try
                {

                    Trading newobj = new Trading();

                    Crypto crypto = db.Cryptoes.FirstOrDefault(x => x.CryptoSymbol == symbol);

                    decimal value = await getcrypto(symbol);
                    var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
                    userdetail.UserBallance = userdetail.UserBallance + (quantity * value);



                    newobj.UserId = userid;
                    newobj.Type = "Sell";
                    newobj.Openprice = value;

                    newobj.CurrentPrice = value;
                    newobj.Name = crypto.Name;
                    newobj.Symbol = symbol;
                    newobj.BuyTime = DateTime.Now;
                    newobj.SharePart = quantity;
                    newobj.TotalPayedAmount = userdetail.UserBallance + (quantity * value);
                    newobj.Active = false;
                    newobj.Obj = "crypto";

                    decimal pass = quantity * value;

                    await decreasecrypto(symbol, quantity, pass);

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
        public int selectcrypto(string cryptoSymbol)
        {

            int currentuserid = User.Identity.GetUserId<int>();

            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);

            AspNetUser user = db.AspNetUsers.Find(User.Identity.GetUserId<int>());


            Crypto getcrypto = db.Cryptoes.FirstOrDefault(x => x.CryptoSymbol.Equals(cryptoSymbol));
            user.CurrentCryptoSymbol = getcrypto.CryptoSymbol;

            try { 

                db.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<decimal> getcrypto(string symbol)
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
            decimal stockValue = (decimal)data["results"][0]["c"];



            return stockValue;
        }


        public async Task<string> buycrypto(string symbol, decimal quantity)
        {
            int currentuserid = User.Identity.GetUserId<int>();
            var userdetail = db.AspNetUsers.FirstOrDefault(x => x.Id == currentuserid);

            decimal callfunc = await getcrypto(symbol);
          
         
            if (quantity * callfunc <= userdetail.UserBallance)
            {

                try
                {


                    Crypto crypto = db.Cryptoes.FirstOrDefault(x => x.CryptoSymbol == symbol);

                    Trading tradings = new Trading();
                    tradings.SharePart = quantity;
                    tradings.Openprice = callfunc;
                    tradings.UserId = User.Identity.GetUserId<int>();
                    tradings.Type = "Buy";
                    tradings.Obj = "crypto";
                    tradings.TotalPayedAmount = Math.Round(quantity * callfunc, 4);
                    tradings.Active = true;
                    //tradings.currentprice = Convert.ToDouble(callfunc);
                    tradings.Name = crypto.CryptoSymbol;
                    tradings.Symbol = symbol;
                    tradings.BuyTime = DateTime.Now;

                    userdetail.UserBallance = userdetail.UserBallance - (tradings.Openprice * quantity);

                    var checker = db.TotalShares.Where(x => x.UserId == currentuserid && x.Symbol.Equals(symbol));
                    if (checker.Count() >= 1)
                    {
                        await updatetotalcrypto(symbol, quantity, callfunc);

                    }
                    else
                    {
                        await addtotalcrypto(symbol, quantity, callfunc);
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

        
        public JsonResult getEntireCrypto(string crypto)
        {
            int userid = User.Identity.GetUserId<int>();

            try
            {
                // Calculate the sum of the TotalVolume field
                decimal totalVolume = db.TotalShares
                                        .Where(x => x.UserId == userid && x.Symbol == crypto)
                                        .Sum(x => x.TotalVolume);

                return Json(totalVolume, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<string> addtotalcrypto(string symbol, decimal quantity, decimal price)
        {
            TotalShare ts = new TotalShare();
            int userid = User.Identity.GetUserId<int>();

            Crypto crypto = db.Cryptoes.FirstOrDefault(x => x.CryptoSymbol == symbol);

            ts.Symbol = symbol;
            ts.TotalVolume = quantity;
            ts.name = crypto.Name;
            ts.UserId = userid;
            ts.TotalPayedAmount = price * quantity;
            ts.Obj = "crypto";



            db.TotalShares.Add(ts);
            db.SaveChanges();

            return "Saved";
        }


        public async Task<string> updatetotalcrypto(string symbol, decimal quantity, decimal price)
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


        public async Task<decimal> decreasecrypto(string symbol, decimal volume, decimal price)
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
        public JsonResult getCryptolist()
        {


            List<Crypto> myList = new List<Crypto>();
            myList = db.Cryptoes.ToList();
            return Json(myList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult gettradlists()
        {
            int userid = User.Identity.GetUserId<int>();
            var currentuser = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<Trading> myList = db.Tradings.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid) && x.Symbol.Equals(currentuser.CurrentCryptoSymbol) && x.Obj.Equals("crypto")).Reverse();
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
            decimal livevalue = await getcrypto(obj.Symbol);

            //live value of current stock
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