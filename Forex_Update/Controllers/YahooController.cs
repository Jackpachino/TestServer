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
    public class YahooController : Controller
    {
        Entities db = new Entities();
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult buildpie()
        {
            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.Where(x => x.UserId.Equals(userid)).ToList();

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> LiveAsync()
        {

            int userid = User.Identity.GetUserId<int>();
            var obj = db.TotalShares.Where(x => x.UserId.Equals(userid)).ToList();
            for (int i = 0; i < obj.Count(); i++)
            {
                if (obj[i].Obj.Equals("stock"))
                {
                    decimal pr = await getstockprice(obj[i].Symbol);
                    obj[i].LivePrice = pr * obj[i].TotalVolume;
                    db.SaveChanges();
                }
                if (obj[i].Obj.Equals("crypto"))
                {
                    decimal pr = await getcrypto(obj[i].Symbol);
                    obj[i].LivePrice = pr * obj[i].TotalVolume;
                    db.SaveChanges();
                }

                if (obj[i].Obj.Equals("currency"))
                {
                    decimal pr = await getcurrency(obj[i].Symbol);
                    obj[i].LivePrice = pr * obj[i].TotalVolume;
                    db.SaveChanges();
                }
            }

            return "updated";

        }


        public async Task<double> PL()
        {

            int userid = User.Identity.GetUserId<int>();
            decimal current = 0;
            decimal live;
            var obj = db.TotalShares.Where(x => x.UserId.Equals(userid)).ToList();
            for (int i = 0; i < obj.Count(); i++)
            {

                current = obj[i].TotalPayedAmount;
                if (obj[i].Obj.Equals("stock"))
                {
                    live = await getstockprice(obj[i].Symbol);
                    decimal profit = obj[i].TotalVolume * live;
                    obj[i].PL = profit - current;
                    db.SaveChanges();
                }
                if (obj[i].Obj.Equals("crypto"))
                {
                    live = await getcrypto(obj[i].Symbol);
                    decimal profit = obj[i].TotalVolume * live;
                    obj[i].PL = profit - current;
                    db.SaveChanges();
                }
                if (obj[i].Obj.Equals("currency"))
                {
                    live = await getcurrency(obj[i].Symbol);
                    decimal profit = obj[i].TotalVolume * live;
                    obj[i].PL = profit - current;
                    db.SaveChanges();
                }
            }

            return 1;

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

    }
}