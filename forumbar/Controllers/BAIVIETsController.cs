using forumbar.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace forumbar.Controllers
{
    public class BAIVIETsController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // GET: BAIVIETs
        public ActionResult Index(string sortOrder)
        {
            // Lưu lại thông tin về thứ tự sắp xếp để sử dụng trong View
            ViewBag.SortOrder = sortOrder;

            // Lấy tất cả bài viết từ cơ sở dữ liệu và liên kết với các bảng CHUDE và USER_INFO
            var bAIVIETs = db.BAIVIETs.Include(b => b.CHUDE).Include(b => b.USER_INFO).AsQueryable();

            // Kiểm tra giá trị của sortOrder và sắp xếp danh sách bài viết theo chữ cái đầu tiên của tiêu đề
            switch (sortOrder)
            {
                case "asc":
                    // Sắp xếp theo chữ cái đầu tiên của tiêu đề từ A-Z
                    bAIVIETs = bAIVIETs.OrderBy(b => b.TieuDe.Substring(0, 1));
                    break;
                case "desc":
                    // Sắp xếp theo chữ cái đầu tiên của tiêu đề từ Z-A
                    bAIVIETs = bAIVIETs.OrderByDescending(b => b.TieuDe.Substring(0, 1));
                    break;
                case "date_desc":
                    // Sắp xếp theo ngày đăng giảm dần
                    bAIVIETs = bAIVIETs.OrderByDescending(b => b.ThoiGianDang);
                    break;
                case "date_asc":
                    // Sắp xếp theo ngày đăng tăng dần
                    bAIVIETs = bAIVIETs.OrderBy(b => b.ThoiGianDang);
                    break;
                default:
                    // Mặc định sắp xếp theo chữ cái đầu tiên của tiêu đề từ A-Z
                    bAIVIETs = bAIVIETs.OrderBy(b => b.TieuDe.Substring(0, 1));
                    break;
            }

            // Trả về View với danh sách bài viết đã sắp xếp
            return View(bAIVIETs.ToList());
        }
        public ActionResult Search(string searchTerm)
        {
            // Kiểm tra nếu không có từ khóa tìm kiếm
            if (string.IsNullOrEmpty(searchTerm))
            { 
                var allPosts = db.BAIVIETs.Include(b => b.CHUDE).Include(b => b.USER_INFO).ToList();
                return View("Index", allPosts); // Chuyển sang View Index với toàn bộ bài viết
            }
            else
            {
                var filteredPosts = db.BAIVIETs
                                      .Where(b => b.TieuDe.Contains(searchTerm))
                                      .Include(b => b.CHUDE)
                                      .Include(b => b.USER_INFO)
                                      .ToList();

                return View("Index", filteredPosts);
            }
        }
        public ActionResult NavPartial()
        {
            return PartialView();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BAIVIET bAIVIET = db.BAIVIETs.Include(b => b.USER_INFO).SingleOrDefault(b => b.IdBaiViet == id);

            if (bAIVIET == null)
            {
                return HttpNotFound();
            }

            var comments = db.BINHLUANs.Include(b => b.USER_INFO)
                                       .Where(b => b.IdBaiViet == id)
                                       .ToList();

            ViewBag.Comments = comments;
            ViewBag.BaiVietId = id;

            return View(bAIVIET);
        }

        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            ViewBag.IdChuDe = new SelectList(db.CHUDEs, "IdChuDe", "TenChuDe");
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "IdBaiViet,TieuDe,NoiDung,IdChuDe")] BAIVIET bAIVIET, HttpPostedFileBase AnhFile)
        {
            // Check if the user is logged in
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            var user = (USER_INFO)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (AnhFile != null && AnhFile.ContentLength > 0)
                {
                    // Generate a unique file name
                    var fileName = Path.GetFileNameWithoutExtension(AnhFile.FileName);
                    var extension = Path.GetExtension(AnhFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                    var path = Path.Combine(Server.MapPath("~/images/"), uniqueFileName);
                    AnhFile.SaveAs(path);

                    bAIVIET.Anh = uniqueFileName;
                }

                bAIVIET.IdUser = user.IdUser;
                bAIVIET.ThoiGianDang = DateTime.Now;

                db.BAIVIETs.Add(bAIVIET);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdChuDe = new SelectList(db.CHUDEs, "IdChuDe", "TenChuDe", bAIVIET.IdChuDe);
            return View(bAIVIET);
        }

        public ActionResult Edit(int id)
        {
            // Retrieve the post by its ID
            var baiViet = db.BAIVIETs.Find(id);

            if (baiViet == null)
            {
                return HttpNotFound();
            }

            // Make sure only the author of the post can edit it
            var currentUser = (USER_INFO)Session["User"];
            if (baiViet.IdUser != currentUser.IdUser)
            {
                return RedirectToAction("Index", "BAIVIET");
            }

            return View(baiViet);
        }

        // POST: BAIVIET/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(BAIVIET baiViet, HttpPostedFileBase Anh)
        {
            if (ModelState.IsValid)
            {
                // Find the existing post to update
                var existingPost = db.BAIVIETs.Find(baiViet.IdBaiViet);

                if (existingPost != null)
                {
                    // Update the fields that are editable
                    existingPost.TieuDe = baiViet.TieuDe;
                    existingPost.NoiDung = baiViet.NoiDung;

                    // Handle the image upload if a new one is provided
                    if (Anh != null && Anh.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(Anh.FileName);
                        string extension = Path.GetExtension(Anh.FileName);
                        string uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        string path = Path.Combine(Server.MapPath("~/images/posts/"), uniqueFileName);

                        // Ensure the folder exists before saving
                        string folderPath = Server.MapPath("~/images/posts/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        Anh.SaveAs(path);
                        existingPost.Anh = uniqueFileName;
                    }

                    db.SaveChanges();  // Save changes to the database
                    return RedirectToAction("UserProfile", "USER_INFO");
                }
            }

            return View(baiViet);
        }

        // GET: BAIVIETs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAIVIET bAIVIET = db.BAIVIETs.Find(id);
            if (bAIVIET == null)
            {
                return HttpNotFound();
            }
            return View(bAIVIET);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BAIVIET bAIVIET = db.BAIVIETs.Find(id);
            db.BAIVIETs.Remove(bAIVIET);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createcommenter(int IdBaiViet, String NoiDung)
        {
            if (string.IsNullOrWhiteSpace(NoiDung))
            {
                ModelState.AddModelError("NoiDung", "Comment content is required");
                return RedirectToAction("Details", new { id = IdBaiViet });
            }

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            var user = (USER_INFO)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            var comment = new BINHLUAN
            {
                NoiDung = NoiDung,
                IdUser = user.IdUser,
                ThoiGianBinhLuan = DateTime.Now,
                IdBaiViet = IdBaiViet
            };

            db.BINHLUANs.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = IdBaiViet });
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
