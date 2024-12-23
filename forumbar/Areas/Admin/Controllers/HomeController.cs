using System.Linq;
using System.Web.Mvc;
using forumbar.Models; // Thay đổi namespace phù hợp với cấu trúc project của bạn

namespace forumbar.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        // GET: Admin/Home/Index
        public ActionResult Index()
        {
            // Kiểm tra nếu chưa đăng nhập
            if (Session["AdminUser"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // GET: Admin/Home/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kiểm tra username và password
            var user = db.USER_INFO
                         .Where(u => u.UserName == username && u.Password == password)
                         .FirstOrDefault();

            if (user != null && user.IdType == 1) // Kiểm tra IdType == 1 (Admin)
            {
                // Lưu thông tin vào Session
                Session["AdminUser"] = user.UserName;
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "Invalid username, password, or you are not an admin.";
            return View();
        }

        public ActionResult Logout()
        {
            // Xóa thông tin trong Session
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
