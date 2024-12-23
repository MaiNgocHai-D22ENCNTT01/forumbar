using forumbar.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bartender212.Controllers
{
    public class BartenderController : Controller
    {
        // GET: Bartender
        private QuanBartender214Entities db = new QuanBartender214Entities();

        public ActionResult Index(int? page)
        {
            int pageSize = 8; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            var products = db.SANPHAMs.Include("DANHMUCSP").OrderBy(p => p.MaSP).ToPagedList(pageNumber, pageSize);

            ViewBag.BestSellers = products;
            return View(products);
        }

        public ActionResult Category(int categoryId, int? page)
        {
            // Lấy tất cả sản phẩm
            var allProducts = db.SANPHAMs.ToList();

            // Lọc sản phẩm theo danh mục
            var productsByCategory = allProducts.Where(p => p.MaDM == categoryId).ToList();

            // Lấy danh sách sản phẩm bán chạy
            var bestSellers = allProducts.OrderByDescending(p => p.SLTon) // Giả sử SLTon là số lượng bán hoặc tồn
                                          .Take(5) // Lấy 5 sản phẩm bán chạy
                                          .ToList();
            ViewBag.BestSellers = bestSellers;

            // Lấy danh sách sản phẩm đề xuất (ví dụ: sản phẩm mới hoặc ngẫu nhiên)
            var recommendedProducts = allProducts.OrderBy(p => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
                                                   .Take(5) // Lấy 5 sản phẩm
                                                   .ToList();
            ViewBag.RecommendedProducts = recommendedProducts;

            // Phân trang: Mỗi trang sẽ hiển thị 12 sản phẩm
            int pageSize = 12;
            int pageNumber = (page ?? 1); // Mặc định trang 1 nếu không có trang nào được chỉ định

            // Phân trang sản phẩm theo danh mục
            var pagedProducts = productsByCategory.ToPagedList(pageNumber, pageSize);

            // Truyền danh sách sản phẩm theo danh mục và phân trang vào View
            return View(pagedProducts);
        }



        public ActionResult Details(int id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu dựa vào id
            var product = db.SANPHAMs.SingleOrDefault(p => p.MaSP == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
    }
}
