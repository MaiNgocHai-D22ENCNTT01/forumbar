using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using forumbar.Models;

namespace forumbar.Areas.Admin.Controllers
{
    public class SANPHAMsController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // GET: Admin/SANPHAMs
        public ActionResult Index()
        {
            var sANPHAMs = db.SANPHAMs.Include(s => s.DANHMUCSP);
            return View(sANPHAMs.ToList());
        }

        // GET: Admin/SANPHAMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = db.SANPHAMs.Find(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Create
        public ActionResult Create()
        {
            ViewBag.MaDM = new SelectList(db.DANHMUCSPs, "MaDM", "TenDM");
            return View();
        }

        // POST: Admin/SANPHAMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaSP,TenSP,GiaGoc,SLTon,MaDM,Anh")] SANPHAM sANPHAM)
        {
            if (ModelState.IsValid)
            {
                db.SANPHAMs.Add(sANPHAM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDM = new SelectList(db.DANHMUCSPs, "MaDM", "TenDM", sANPHAM.MaDM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = db.SANPHAMs.Find(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDM = new SelectList(db.DANHMUCSPs, "MaDM", "TenDM", sANPHAM.MaDM);
            return View(sANPHAM);
        }

        // POST: Admin/SANPHAMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SANPHAM sanPham, HttpPostedFileBase Anh)
        {
            if (ModelState.IsValid)
            {
                // Nếu có file ảnh được tải lên
                if (Anh != null && Anh.ContentLength > 0)
                {
                    // Tạo đường dẫn cho ảnh
                    string fileName = Path.GetFileName(Anh.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                    // Lưu file lên server
                    Anh.SaveAs(path);

                    // Cập nhật thuộc tính Anh với đường dẫn ảnh
                    sanPham.Anh = "~/Images/" + fileName;
                }

                // Cập nhật sản phẩm trong database
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();

                // Chuyển hướng về danh sách sản phẩm
                return RedirectToAction("Index");
            }
            return View(sanPham);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var sanPham = db.SANPHAMs.Find(id);
            if (sanPham != null)
            {
                db.SANPHAMs.Remove(sanPham);
                db.SaveChanges();
            }
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
