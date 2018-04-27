using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using comp2007amMusicStore.Models;

namespace comp2007amMusicStore.Controllers
{
    public class OrdersController : Controller
    {
        // comment out hard-coded db connection so we can also unit test
        //private MusicStoreModel db = new MusicStoreModel();
        private IMockOrdersRepository db;

        // default constructor - use the database / ef
        public OrdersController()
        {
            this.db = new EFOrdersRepository();
        }

        // unit test constructor - mock data comes in so don't use the database/ef
        public OrdersController(IMockOrdersRepository mockRepo)
        {
            this.db = mockRepo;
        }

        // GET: Orders
        public ActionResult Index()
        {
            return View(db.Orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            // modify so it can work with ef or the mock repo
            //Order order = db.Orders.Find(id);

            // new code to select single order using LINQ
            Order order = db.Orders.SingleOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                //db.Orders.Add(order);
                //db.SaveChanges();
                db.Save(order);  // use repo's now instead of ef directly
                return RedirectToAction("Index");
            }

            return View("Create", order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            // original scaffold code - doesn't work when unit testing
            //Order order = db.Orders.Find(id);

            // new code - works with both mock and ef repositories
            Order order = db.Orders.SingleOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                // original scaffold code - doesn't work when unit testing
                //db.Entry(order).State = EntityState.Modified;
                //db.SaveChanges();

                // new code - works with mock & ef repo's
                db.Save(order);

                return RedirectToAction("Index");
            }
            return View("Edit", order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View("Error");
            }
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                //return HttpNotFound();
                return View("Error");
            }
            return View("Delete", order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Order order = db.Orders.Find(id);
            //db.Orders.Remove(order);
            //db.SaveChanges();
            Order order = db.Orders.SingleOrDefault(o => o.OrderId == id);
            db.Delete(order);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
