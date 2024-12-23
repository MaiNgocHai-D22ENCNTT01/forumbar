using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using forumbar.Models;

namespace forumbar.Controllers
{
    public class USER_INFOController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();

        public ActionResult Index()
        {
            var uSER_INFO = db.USER_INFO.Include(u => u.USERTYPE);
            return View(uSER_INFO.ToList());
        }

        public ActionResult Details()
        {
            var user = (USER_INFO)Session["User"];

            USER_INFO uSER_INFO = db.USER_INFO.Find(user.IdUser);

            if (uSER_INFO == null)
            {
                return HttpNotFound();
            }

            return View(uSER_INFO);
        }

        public ActionResult UserProfile()
        {
            // Lấy thông tin người dùng hiện tại từ session
            var user = (USER_INFO)Session["User"];

            if (user == null)
            {
                return HttpNotFound("User not found");
            }

            var userWithPosts = db.USER_INFO
                                  .Include(u => u.BAIVIETs)
                                  .Include(u => u.BAIVIETs.Select(b => b.BINHLUANs))  
                                  .FirstOrDefault(u => u.IdUser == user.IdUser); 

            if (userWithPosts == null)
            {
                return HttpNotFound("User not found");
            }

            return View(userWithPosts);  
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: USER_INFO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection, USER_INFO US)
        {
            var FullName = collection["FullName"];
            var UserName = collection["UserName"];
            var Password = collection["Password"];
            var Phone = collection["Phone"];
            var Address = collection["Address"];
            var Email = collection["Email"];
            var IdType = collection["IdType"];

            // Validate input fields
            if (String.IsNullOrEmpty(FullName))
            {
                ViewData["err1"] = "Full Name cannot be empty";
            }
            else if (String.IsNullOrEmpty(UserName))
            {
                ViewData["err1"] = "Username cannot be empty";
            }
            else if (String.IsNullOrEmpty(Password))
            {
                ViewData["err1"] = "Password cannot be empty";
            }
            else if (String.IsNullOrEmpty(Phone))
            {
                ViewData["err1"] = "Phone number cannot be empty";
            }
            else if (String.IsNullOrEmpty(Address))
            {
                ViewData["err1"] = "Address cannot be empty";
            }
            else if (String.IsNullOrEmpty(Email))
            {
                ViewData["err1"] = "Email cannot be empty";
            }
            else if (db.USER_INFO.SingleOrDefault(n => n.UserName == UserName) != null)
            {
                ViewBag.ThongBao = "Username already exists";
            }
            else
            {
                // Assign input values to the USER_INFO object
                US.FullName = FullName;
                US.UserName = UserName;
                US.Password = Password;
                US.Phone = Phone;
                US.Address = Address;
                US.Email = Email;

                // Assign IdType
                US.IdType = 2;

                // Save to database
                db.USER_INFO.Add(US);
                db.SaveChanges();

                // Redirect to Login page
                return RedirectToAction("Login");
            }

            // Return Register view if there are errors
            return View();
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = db.USER_INFO.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(USER_INFO US, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                var user = db.USER_INFO.Find(US.IdUser); // Assuming IdUser is the primary key

                if (user != null)
                {
                    // Update allowed fields
                    user.FullName = US.FullName;
                    user.Phone = US.Phone;
                    user.Address = US.Address;
                    user.Email = US.Email;

                    // Handle Avatar update with a unique filename
                    if (Avatar != null && Avatar.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(Avatar.FileName);
                        string extension = Path.GetExtension(Avatar.FileName);
                        string uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        string path = Path.Combine(Server.MapPath("~/images/resources/"), uniqueFileName);

                        // Ensure the folder exists before saving
                        string folderPath = Server.MapPath("~/images/resources/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        Avatar.SaveAs(path);
                        user.Avatar = uniqueFileName;
                    }

                    db.SaveChanges(); // Save changes to the database
                }

                return RedirectToAction("UserProfile"); // Or whichever page to redirect after edit
            }

            return View(US); // Return to the view with validation errors if any
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(USER_INFO model)
        {
            if (ModelState.IsValid)
            {
                var user = db.USER_INFO.FirstOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);

                if (user != null)
                {
                    Session["User"] = user;
                    Session["UserName"] = user.FullName;

                    return RedirectToAction("Index", "BAIVIETs");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["UserID"] = null;

            return RedirectToAction("Index", "BAIVIETs");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("x2"));
                }
                return builder.ToString();
            }
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
