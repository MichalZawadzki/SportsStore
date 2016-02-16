using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository _repository;

        public AdminController(IProductRepository repo)
        {
            _repository = repo;
        }

        public ActionResult Index()
        {
            return View(_repository.Products);
        }

        public ViewResult Edit(int id)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.Id == id);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                _repository.SaveProduct(product);
                TempData["message"] = string.Format("Zapisano {0} ", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Product deletedProduct = _repository.DeleteProduct(id);
            if (deletedProduct != null)
                TempData["message"] = string.Format("Usunięto {0}", deletedProduct.Name);
            return RedirectToAction("Index");
        }

        public FileContentResult GetImage(int id)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                return File(product.ImageData, product.ImageMimeType);
            else
                return null;
        }
	}
}