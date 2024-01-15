
using Antlr.Runtime.Debug;
using Forex_Update.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace myForex.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        Entities db = new Entities();

        public ActionResult Index(int? i)
        {
           
            int currentuserid = User.Identity.GetUserId<int> ();

            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));
            customer.LastLoginTime = DateTime.Now.ToString();
            db.SaveChanges();
            ViewBag.Firstname = customer.FirstName;
            ViewBag.Lastname = customer.LastName;
            ViewBag.Email = customer.Email;
            ViewBag.Id = customer.Id;
            ViewBag.Time = customer.LastLoginTime;

            List<Trading> listoftrading = db.Tradings.ToList();
            AllViewmodel objallViewmodels = new AllViewmodel();
            objallViewmodels.TradingView = listoftrading.Where(a => a.UserId.Equals(currentuserid) && a.Active.Equals(true)).Reverse().ToPagedList(i ?? 1, 5);
            return View(objallViewmodels);

        }


        public ActionResult Discoveries()
        {  
            return View();
        }

        [HttpGet]
        public ActionResult withdrawal()
        {
            int currentuserid = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));

            ViewBag.Firstname = customer.FirstName;
            ViewBag.Lastname = customer.LastName;
            ViewBag.Promo = customer.Promocode;
            ViewBag.Ballance = Math.Round((double)customer.UserBallance, 2);
            return View();
        }

        [HttpGet]
        public ActionResult Deposit()
        {
            int currentuserid = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));

            ViewBag.Email = customer.Email;
            ViewBag.Phone = customer.PhoneNumber;
            ViewBag.PayID = customer.Id;
            return View();
        }



        [HttpPost]
        public string createwithdrawal(string email,double amount,string paymentmethod,string phone, /*string postalcode,*/string city,string dateofbirth,string ssn,string promocode)
        {
            try
            {
                Withdrawal obj = new Withdrawal();
                int currentuserid = User.Identity.GetUserId<int>();
                obj.PromoCode = promocode;
                obj.UserId = currentuserid.ToString();
                obj.UseEmail = email;
                obj.Amount = amount;
                obj.PaymentMethod = paymentmethod;
                obj.Phone = phone;
                //obj.PostalCode = postalcode;
                obj.City = city;
                obj.DateOfBirth = dateofbirth;
                obj.SSN=ssn;
                obj.Status = 0;

                db.Withdrawals.Add(obj);
                db.SaveChanges();

                return "1";
            }
            catch(Exception ex) {

                Console.WriteLine(ex);
                return "0";
            }
        }

        [HttpGet]
        public async Task<decimal> getbtcprice()
        {
            string symbol = "BTCUSD";
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


        [HttpPost]
        public Boolean depositrequest(string transferfrom, double btcamount,double usdamount)
        {
            int currentuserid = User.Identity.GetUserId<int>();
            Request deposit = new Request();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));
            try
            {
                deposit.UserId = currentuserid;
                deposit.UserPhone = customer.PhoneNumber;
                deposit.UserEmail = customer.Email;
                deposit.UserBTCWallet = customer.Btcwallet;
                deposit.RequestDate = DateTime.Now.ToString();
                deposit.DepositAmount = usdamount;
                deposit.DepositAmountBTC = Math.Round(btcamount,8);
                deposit.Status = 0;

                db.Requests.Add(deposit);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex) {

                return false;
            }
            
        }

        public double getbtcvalue()
        {
            int currentuserid = User.Identity.GetUserId<int>();
            var deposit = db.Requests.Where(a => a.UserId== currentuserid).ToList().Last();

            double? amount = deposit.DepositAmountBTC;

            return (double)amount;


        }


        [HttpGet]
        public JsonResult getmystock()
        {
            int userid = User.Identity.GetUserId<int>();
            var currentuser = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);
            List<TotalShare> myList = db.TotalShares.ToList();
            var data = myList.Where(x => x.UserId.Equals(userid)).Reverse();
            Console.Write(data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public string getdataforpie()
        {
            int userid = User.Identity.GetUserId<int>();
            using (var context = new Entities()) // Replace `YourDbContext` with your actual DbContext class
            {
                var data = context.TotalShares
                    .Where(a=>a.UserId.Equals(userid))
                    .AsNoTracking()
                    .GroupBy(s => s.Symbol)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.LivePrice));

                var pie = new Pie
                {
                    Total = data.Values.Sum(),
                    Data = data
                };
                if(pie == null)
                {
                    return "Empty";
                }

                return JsonConvert.SerializeObject(pie);
            }
        }

        public string getwallet()
        {
            int userid = User.Identity.GetUserId<int>();
            var currentuser = db.AspNetUsers.FirstOrDefault(x => x.Id == userid);

            return currentuser.Btcwallet;
        }

        [HttpGet]
        public ActionResult discoveries()
        {
            return View();
        }

    }
}