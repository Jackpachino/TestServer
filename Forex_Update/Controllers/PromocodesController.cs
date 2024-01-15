using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Forex_Update.Models;
using PagedList;

namespace Forex_Update.Controllers
{
    [Authorize(Roles = "ROLE_ADMIN")]
    public class PromocodesController : Controller
    {
        private Entities db = new Entities();

        // GET: Promocodes


        public ActionResult Index(string search, int? page, bool showInactiveNoUser = false)
        {
            var promocodesWithUsers = from promocode in db.Promocodes
                                      join user in db.AspNetUsers on promocode.UserId equals user.Id into userGroup
                                      from user in userGroup.DefaultIfEmpty()
                                      select new PromocodeUserViewModel
                                      {
                                          Id = promocode.Id,
                                          PromocodeStr = promocode.PromocodeStr,
                                          BtcWallet = promocode.BtcWallet,
                                          UserName = user != null ? user.UserName : String.Empty,
                                          UserId = user != null ? (int?)user.Id : null,
                                          Active = promocode.Active
                                      };

            if (!String.IsNullOrEmpty(search))
            {
                promocodesWithUsers = promocodesWithUsers.Where(p => p.PromocodeStr.Contains(search) ||
                                                                      p.BtcWallet.Contains(search) ||
                                                                      (p.UserName != null && p.UserName.Contains(search)));
            }

            if (showInactiveNoUser)
            {
                promocodesWithUsers = promocodesWithUsers.Where(p => p.UserId == null && p.Active == false);
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);

            var pagedResult = promocodesWithUsers.OrderBy(p => p.PromocodeStr)
                                                 .ToPagedList(pageNumber, pageSize);

            return View(pagedResult);
        }


        // GET: Promocodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promocode promocode = db.Promocodes.Find(id);
            if (promocode == null)
            {
                return HttpNotFound();
            }
            return View(promocode);
        }

        // GET: Promocodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Promocodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public ActionResult Create([Bind(Include = "Id,PromocodeStr,BtcWallet")] Promocode promocode)
        {
            // Check if the promocode already exists
            bool promocodeExists = db.Promocodes.Any(p => p.PromocodeStr == promocode.PromocodeStr);
            if (promocodeExists)
            {
                ModelState.AddModelError("PromocodeStr", "This promocode already exists.");
                return View(promocode);
            }

            if (ModelState.IsValid)
            {
                promocode.Active = false;
                db.Promocodes.Add(promocode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(promocode);
        }


        // GET: Promocodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promocode promocode = db.Promocodes.Find(id);
            if (promocode == null)
            {
                return HttpNotFound();
            }
            return View(promocode);
        }

        // POST: Promocodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PromocodeStr,BtcWallet,UserId,Active")] Promocode promocode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promocode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(promocode);
        }

        // GET: Promocodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promocode promocode = db.Promocodes.Find(id);
            if (promocode == null)
            {
                return HttpNotFound();
            }
            return View(promocode);
        }

        // POST: Promocodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Promocode promocode = db.Promocodes.Find(id);
            db.Promocodes.Remove(promocode);
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
