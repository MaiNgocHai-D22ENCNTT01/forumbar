using forumbar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.IO;
using System.Security.Policy;
using System.Windows.Controls.Primitives;

namespace Bartender212.Controllers
{

    public class CartController : Controller
    {
        private QuanBartender214Entities db = new QuanBartender214Entities();
        public class GioHangItem
        {
            public int MaSP { get; set; }
            public string TenSP { get; set; }
            public decimal Gia { get; set; }
            public int SoLuong { get; set; }
            public string HinhAnh { get; set; }
            public decimal ThanhTien => SoLuong * Gia;
        }


        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<GioHangItem>;
            return View(cart ?? new List<GioHangItem>());
        }

        [HttpPost]
        public ActionResult AddToCart(int productId)
        {
            if (Session["User"] == null)
            {
                TempData["ReturnUrl"] = Url.Action("AddToCart", "Cart", new { productId });
                return RedirectToAction("Login", "USER_INFO");
            }

            var product = db.SANPHAMs.SingleOrDefault(p => p.MaSP == productId);
            if (product == null || product.SLTon <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc hết hàng!" });
            }

            product.SLTon -= 1;
            db.SaveChanges();

            var cart = Session["Cart"] as List<GioHangItem> ?? new List<GioHangItem>();
            var cartItem = cart.FirstOrDefault(c => c.MaSP == productId);

            if (cartItem != null)
            {
                cartItem.SoLuong++;
            }
            else
            {
                cart.Add(new GioHangItem
                {
                    MaSP = product.MaSP,
                    TenSP = product.TenSP,
                    Gia = (decimal)product.GiaGoc,
                    SoLuong = 1,
                    HinhAnh = product.Anh
                });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index", "Bartender");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = Session["Cart"] as List<GioHangItem>;
            if (cart != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.MaSP == productId);
                if (cartItem != null)
                {
                    var product = db.SANPHAMs.SingleOrDefault(p => p.MaSP == productId);
                    if (product != null)
                    {
                        product.SLTon += cartItem.SoLuong;
                        db.SaveChanges();
                    }

                    cart.Remove(cartItem);
                }
                Session["Cart"] = cart;
            }

            return RedirectToAction("Index", "Cart");
        }

        public ActionResult Checkout()
        {
            if (Session["User"] == null)
            {
                TempData["ReturnUrl"] = Url.Action("Confirm", "Cart");
                return RedirectToAction("Login", "USER_INFO");
            }

            var cart = Session["Cart"] as List<GioHangItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Address = ((USER_INFO)Session["User"]).Address;
            return View(cart);
        }

        [HttpPost]
        public ActionResult ConfirmCheckout(string address)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Bartender");
            }

            Session["Address"] = address;
            return RedirectToAction("CompletePayment");
        }
        public ActionResult CompletePayment(string paymentMethod , FormCollection f)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "USER_INFO");
            }

            var cart = Session["Cart"] as List<GioHangItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            if (paymentMethod == "qr")
            {
                var totalAmount = cart.Sum(item => item.ThanhTien);
                string qrCodeContent = $"https://paymentgateway.com/pay?amount={totalAmount}";

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrImage = qrCode.GetGraphic(20);

                var directoryPath = Server.MapPath("~/images/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string qrImageFileName = Guid.NewGuid().ToString() + ".png";
                string qrImagePath = Path.Combine(directoryPath, qrImageFileName);
                qrImage.Save(qrImagePath, System.Drawing.Imaging.ImageFormat.Png);

                string qrImageUrl = Url.Content("~/images/" + qrImageFileName);

                return Json(new { success = true, qrCodeUrl = qrImageUrl });
            }

            if (paymentMethod == "cash")
            {
                var user = (USER_INFO)Session["User"];
                var newOrder = new DONHANG
                {
                    NgayDat = DateTime.Now,
                    DiaChiGiaoHang = f["address"].ToString(),
                    SDT = user.Phone,
                    TrangThai = "Đang xử lý",
                    DaThanhToan = true,
                    IdUser = user.IdUser
                };
                db.DONHANGs.Add(newOrder);
                db.SaveChanges();

                foreach (var item in cart)
                {
                    db.CTDonHangs.Add(new CTDonHang
                    {
                        MaDH = newOrder.MaDH,
                        MaSP = item.MaSP,
                        SoLuong = item.SoLuong
                    });
                }
                db.SaveChanges();

                TempData["SuccessMessage"] = "Thanh toán thành công! Cảm ơn bạn đã mua hàng.";
                Session["Cart"] = null;
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Checkout");
        }
    }
}
