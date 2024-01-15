using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Forex_Update.Models;
using Microsoft.AspNet.Identity;

namespace Forex_Update.Controllers
{
    [Authorize]
    public class TradingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Tradings
        public ActionResult Index()
        {

            // added by Loki
            int currentuserid = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));
            ViewBag.Firstname = customer.FirstName;
            IQueryable obj = db.Tradings.Where(a => a.UserId == currentuserid).ToList().AsQueryable().Reverse();
            return View(obj);

            // end of update 
            //return View(db.Tradings.ToList());
        }

        public ActionResult Deposit()
        {

            int currentuserid = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));

            ViewBag.Firstname = customer.FirstName;
            return View(db.Requests.AsEnumerable().Where(a => a.UserId.Equals(currentuserid)).Reverse().ToList());
        }

        public ActionResult Withdrawal()
        {
            int currentuserid = User.Identity.GetUserId<int>();
            AspNetUser u = db.AspNetUsers.FirstOrDefault(x => x.Id.Equals(currentuserid));

            ViewBag.firstname = u.FirstName;
            return View(db.Withdrawals.AsEnumerable().Where(a => a.UserId.Equals(currentuserid.ToString())).Reverse().ToList());
        }
    

        public JsonResult getjsonlist(int page = 1, int pageSize = 5) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId== currentUserId && a.Symbol == customer.CurrentCryptoSymbol && a.Obj== "crypto")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {                
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getalljson(int page = 1, int pageSize = 10) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId == currentUserId && a.Obj == "crypto")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// stockkkk one
        /// <returns></returns>
        public JsonResult getjsonlistStock(int page = 1, int pageSize = 5) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId == currentUserId && a.Symbol == customer.CurrentStockSymbol && a.Obj == "stock")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getalljsonStock(int page = 1, int pageSize = 10) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId == currentUserId && a.Obj == "stock")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getjsonlistCurrency(int page = 1, int pageSize = 5) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId == currentUserId && a.Symbol == customer.Currency && a.Obj == "currency")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getalljsonCurrency(int page = 1, int pageSize = 10) // Default values for page and pageSize
        {
            int currentUserId = User.Identity.GetUserId<int>();
            AspNetUser customer = db.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);

            // Calculate the number of records to skip
            int skip = (page - 1) * pageSize;

            // Get the paginated data
            var tradings = db.Tradings
                             .Where(a => a.UserId == currentUserId && a.Obj == "currency")
                             .OrderByDescending(a => a.BuyTime) // Assuming Trading has a TradingDate for ordering
                             .Skip(skip)
                             .Take(pageSize)
                             .ToList();

            // Construct the JSON result
            var result = new
            {
                Data = tradings
            };

            // Return the data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: Tradings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trading trading = db.Tradings.Find(id);
            if (trading == null)
            {
                return HttpNotFound();
            }
            return View(trading);
        }

        // GET: Tradings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Type,Openprice,CurrentPrice,Name,Symbol,SharePart,TotalPayedAmount,Profit,Active,BuyTime,Obj")] Trading trading)
        {
            if (ModelState.IsValid)
            {
                db.Tradings.Add(trading);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trading);
        }

        // GET: Tradings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trading trading = db.Tradings.Find(id);
            if (trading == null)
            {
                return HttpNotFound();
            }
            return View(trading);
        }

        // POST: Tradings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Type,Openprice,CurrentPrice,Name,Symbol,SharePart,TotalPayedAmount,Profit,Active,BuyTime,Obj")] Trading trading)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trading).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trading);
        }

        // GET: Tradings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trading trading = db.Tradings.Find(id);
            if (trading == null)
            {
                return HttpNotFound();
            }
            return View(trading);
        }

        // POST: Tradings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trading trading = db.Tradings.Find(id);
            db.Tradings.Remove(trading);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
