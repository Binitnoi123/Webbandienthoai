using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDienThoaiDoAn.Models;
namespace WebBanDienThoaiDoAn.Controllers
{
    public class ProductController : Controller
    {
        CellPhoneSContext db = new CellPhoneSContext();
        // GET: Product
        public ActionResult Index()
        {
            var listPro = db.Products.ToList();
            return View(listPro);
        }

        public ActionResult Category(int CategoryID = 0)
        {
            if (CategoryID != 0)
            {
                ViewBag.TieuDe = db.Categories.FirstOrDefault(p => p.Name != null).Name;
                var model = db.Products.Where(p => p.CategoryId == CategoryID);
                return View(model);
            }

            return View();
        }
        public ActionResult Detail(int id, string SupplierID)
        {
            var model = db.Products.Find(id);

            // Tăng số lần xem
            model.Views++;
            db.SaveChanges();

            // Lấy cookie cũ tên views
            var views = Request.Cookies["views"];
            // Nếu chưa có cookie cũ -> tạo mới
            if (views == null)
            {
                views = new HttpCookie("views");
            }
            // Bổ sung mặt hàng đã xem vào cookie
            views.Values[id.ToString()] = id.ToString();
            // Đặt thời hạn tồn tại của cookie
            views.Expires = DateTime.Now.AddHours(24);
            // Gửi cookie về client để lưu lại
            Response.Cookies.Add(views);

            // Lấy List<int> chứa mã hàng đã xem từ cookie
            var keys = views.Values
                .AllKeys.Select(k => int.Parse(k)).ToList();
            // Truy vấn háng đãn xem
            ViewBag.Views = db.Products
                .Where(p => keys.Contains(p.Id));
            ViewBag.Top = db.Products.Where(p => p.Id > 0).OrderByDescending(p => p.Views).Take(10).ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Create")]
        public ActionResult Create([Bind(Include = "Id,Name,UnitBrief,UnitPrice,Image,ProductDate,Available,Description,CategoryId,SupplierId,Quantity,Discount,Special,Latest,Views")] Product pro, HttpPostedFileBase fileImage)
        {
            try
            {
                if (fileImage != null || fileImage.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileImage.FileName);
                    string path = Path.Combine(Server.MapPath("/Content/img/products/"), _FileName);
                    if (System.IO.File.Exists(path))
                    {
                        //nếu hình ảnh đã tồn tại, thì xóa ảnh cũ, cập nhật lại ảnh mới
                        System.IO.File.Delete(path);
                        fileImage.SaveAs(path);
                        pro.Image = _FileName;
                    }
                    else
                    {
                        fileImage.SaveAs(path);
                        pro.Image = _FileName;

                    }
                }

                if (ModelState.IsValid)
                {
                    db.Products.Add(pro);
                    db.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            Product pro = db.Products.FirstOrDefault(p => p.Id == id);
            return View(pro);
        }
        public ActionResult Delete(int id)
        {
            Product dbDelete = db.Products.FirstOrDefault(p => p.Id == id);
            if (dbDelete != null)
            {
                db.Products.Remove(dbDelete);
                db.SaveChanges();
            }
            return RedirectToAction("ListBook");
        }
        public ActionResult Edit(int ID)
        {

            Product dbUpdate = db.Products.FirstOrDefault(p => p.Id == ID);
            return View(dbUpdate);
        }
        public ActionResult Search(String SupplierId = "", int CategoryId = 0, String Keywords = "")
        {
            if (SupplierId != "")
            {
                var model = db.Products
                    .Where(p => p.SupplierId == SupplierId);
                return View(model);
            }
            else if (CategoryId != 0)
            {
                var model = db.Products
                    .Where(p => p.CategoryId == CategoryId);
                return View(model);
            }
            else if (Keywords != "")
            {
                var model = db.Products
                    .Where(p => p.Name.Contains(Keywords));
                return View(model);
            }
            return View(db.Products);
        }
    }
}