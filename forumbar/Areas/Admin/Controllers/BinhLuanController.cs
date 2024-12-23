using System;
using System.Linq;
using System.Web.Mvc;
using forumbar.Models;

namespace forumbar.Areas.Admin.Controllers
{
    public class BinhLuanController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // Hiển thị danh sách bình luận
        public ActionResult Index()
        {
            var binhLuans = db.BINHLUANs.ToList();
            return View(binhLuans);
        }

        // Xóa bình luận
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var binhLuan = db.BINHLUANs.Find(id);
            if (binhLuan != null)
            {
                db.BINHLUANs.Remove(binhLuan);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
