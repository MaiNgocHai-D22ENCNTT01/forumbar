using System;
using System.Linq;
using System.Web.Mvc;
using forumbar.Models;

namespace forumbar.Areas.Admin.Controllers
{
    public class ChuDeController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // Hiển thị danh sách chủ đề
        public ActionResult Index()
        {
            var chuDeList = db.CHUDEs.ToList();
            return View(chuDeList);
        }

        // Hiển thị trang tạo chủ đề mới
        public ActionResult Create()
        {
            return View();
        }

        // Xử lý tạo chủ đề mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CHUDE chuDe)
        {
            if (ModelState.IsValid)
            {
                db.CHUDEs.Add(chuDe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chuDe);
        }

        // Xóa chủ đề
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var chuDe = db.CHUDEs.Find(id);
            if (chuDe != null)
            {
                db.CHUDEs.Remove(chuDe);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
