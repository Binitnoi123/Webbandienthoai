using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebBanDienThoaiDoAn.Models;

namespace WebBanDienThoaiDoAn.Controllers
{
    public class CategoryController : Controller
    {
        CellPhoneSContext context = new CellPhoneSContext();
        // GET: Category
        public ActionResult Index()
        {
            List<Category> lstCatalog = context.Categories.ToList();
            return View(lstCatalog);
        }
        public ActionResult Details(int id)
        {
            Category p = context.Categories.FirstOrDefault(x => x.Id == id);
            return View(p);
        }
        public ActionResult Edit(int ID)
        {

            Category dbUpdate = context.Categories.FirstOrDefault(p => p.Id == ID);
            return View(dbUpdate);
        }
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Edit")]
        public ActionResult PostEditCategory(int Id, HttpPostedFileBase fileImage)
        {
            Category bk = context.Categories.FirstOrDefault(p => p.Id == Id);
            if (bk != null)
            {
                if (Request.Form.Count == 0) return View(bk);
                bk.Name = Request.Form["Name"];
                bk.NameVN = Request.Form["NameVN"];
                HttpPostedFileBase file = Request.Files["Icon"];
                if (fileImage != null || fileImage.ContentLength > 0)
                {
                    string _fileName = Path.GetFileName(fileImage.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/img/icons/"), _fileName);
                    fileImage.SaveAs(path);
                    bk.Icon = Path.GetFileName(fileImage.FileName);
                }
                context.Categories.AddOrUpdate(bk); //Add or Update  b
                context.SaveChanges();
            }
            return RedirectToAction("Index","Product");
        }
        public ActionResult Delete(int id)
        {
            Category dbDelete = context.Categories.FirstOrDefault(p => p.Id == id);
            if (dbDelete != null)
            {
                context.Categories.Remove(dbDelete);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Create")]
        public ActionResult CreateCategory([Bind(Include = "NameVN,Name,Icon")] Category category, HttpPostedFileBase fileImage)
        {
            try
            {
                if (fileImage != null || fileImage.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileImage.FileName);
                    string path = Path.Combine(Server.MapPath("/Content/img/icons/"), _FileName);
                    if (System.IO.File.Exists(path))
                    {
                        //nếu hình ảnh đã tồn tại, thì xóa ảnh cũ, cập nhật lại ảnh mới
                        System.IO.File.Delete(path);
                        fileImage.SaveAs(path);
                        category.Icon = _FileName;
                    }
                    else
                    {
                        fileImage.SaveAs(path);
                        category.Icon = _FileName;

                    }
                }

                if (ModelState.IsValid)
                {
                    context.Categories.Add(category);
                    context.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            return RedirectToAction("Index");
        }
    }
}