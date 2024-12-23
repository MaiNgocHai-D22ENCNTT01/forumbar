using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using forumbar.Models;

namespace forumbar.Areas.Admin.Controllers
{
    public class USER_INFOSController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // GET: Admin/USER_INFO
        public ActionResult Index()
        {
            var uSER_INFO = db.USER_INFO.Include(u => u.USERTYPE);
            return View(uSER_INFO.ToList());
        }

        // GET: Admin/USER_INFO/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER_INFO uSER_INFO = db.USER_INFO.Find(id);
            if (uSER_INFO == null)
            {
                return HttpNotFound();
            }
            return View(uSER_INFO);
        }

        // GET: Admin/USER_INFO/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.USERTYPEs, "IdType", "NameType");
            return View();
        }

        // POST: Admin/USER_INFO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUser,FullName,UserName,Password,Phone,Address,Email,IdType,Avatar")] USER_INFO uSER_INFO)
        {
            if (ModelState.IsValid)
            {
                db.USER_INFO.Add(uSER_INFO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.USERTYPEs, "IdType", "NameType", uSER_INFO.IdType);
            return View(uSER_INFO);
        }

        // GET: Admin/USER_INFO/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER_INFO uSER_INFO = db.USER_INFO.Find(id);
            if (uSER_INFO == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.USERTYPEs, "IdType", "NameType", uSER_INFO.IdType);
            return View(uSER_INFO);
        }

        // POST: Admin/USER_INFO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUser,FullName,UserName,Password,Phone,Address,Email,IdType,Avatar")] USER_INFO uSER_INFO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSER_INFO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.USERTYPEs, "IdType", "NameType", uSER_INFO.IdType);
            return View(uSER_INFO);
        }

        // GET: Admin/USER_INFO/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USER_INFO uSER_INFO = db.USER_INFO.Find(id);
            if (uSER_INFO == null)
            {
                return HttpNotFound();
            }
            return View(uSER_INFO);
        }

        // POST: Admin/USER_INFO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USER_INFO uSER_INFO = db.USER_INFO.Find(id);
            db.USER_INFO.Remove(uSER_INFO);
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
