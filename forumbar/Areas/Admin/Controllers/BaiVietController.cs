using forumbar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace forumbar.Areas.Admin.Controllers
{
    public class BaiVietController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // GET: Admin/BaiViet
        public ActionResult Index()
        {
            // Lấy tất cả các bài viết từ cơ sở dữ liệu
            var baiViets = db.BAIVIETs.ToList();

            // Trả về view với danh sách bài viết
            return View(baiViets);  // View này nhận kiểu IEnumerable<BAIVIET>
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            BAIVIET baiViet = db.BAIVIETs.Find(id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }

        // POST: Admin/BaiViet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BAIVIET baiViet = db.BAIVIETs.Find(id);
            db.BAIVIETs.Remove(baiViet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/BaiViet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            BAIVIET baiViet = db.BAIVIETs.Include("CHUDE").Include("USER_INFO").FirstOrDefault(bv => bv.IdBaiViet == id);
            if (baiViet == null)
            {
                return HttpNotFound();
            }
            return View(baiViet);
        }
    }
}
