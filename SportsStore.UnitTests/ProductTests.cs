using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void CanGeneratePageLinks()
        {
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo(){CurrentPage = 2, TotalItems = 28, ItemsPerPage = 10};
            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"+
                @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>"+
                @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void CanPaginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                new Product(){Id = 2, Name = "P2"},
                new Product(){Id = 3, Name = "P3"},
                new Product(){Id = 4, Name = "P4"},
                new Product(){Id = 5, Name = "P5"},
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            var result = (ViewResult)controller.List(null, 2);

            Product[] prodArray = ((ProductListViewModel)result.Model).Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void CanSendPaginationViewModel()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                new Product(){Id = 2, Name = "P2"},
                new Product(){Id = 3, Name = "P3"},
                new Product(){Id = 4, Name = "P4"},
                new Product(){Id = 5, Name = "P5"},
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            var result = (ViewResult) controller.List(null, 2);

            PagingInfo pagingInfo = ((ProductListViewModel)result.Model).PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
        }

        [TestMethod]
        public void CanFilterProducts()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {Id = 1, Name = "P1", Category = "Cat1"},
                new Product() {Id = 2, Name = "P2", Category = "Cat2"},
                new Product() {Id = 3, Name = "P3", Category = "Cat1"},
                new Product() {Id = 4, Name = "P4", Category = "Cat2"},
                new Product() {Id = 5, Name = "P5", Category = "Cat3"},
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            var viewResult = ((ViewResult)controller.List("Cat2", 1));
            var result = ((ProductListViewModel) viewResult.Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void CanCreateCategories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {Id = 1, Name = "P1", Category = "Jabłka"},
                new Product() {Id = 2, Name = "P2", Category = "Jabłka"},
                new Product() {Id = 3, Name = "P3", Category = "Śliwki"},
                new Product() {Id = 4, Name = "P4", Category = "Pomarańcze"}
            });
            NavController controller = new NavController(mock.Object);
            
            var viewResult = (PartialViewResult) controller.Menu();
            string[] result = ((IEnumerable<string>) viewResult.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "Jabłka");
            Assert.AreEqual(result[1], "Pomarańcze");
            Assert.AreEqual(result[2], "Śliwki");
        }

        [TestMethod]
        public void IndicatesSelectedCategory()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {Id = 1, Name = "P1", Category = "Jabłka"},
                new Product() {Id = 2, Name = "P2", Category = "Pomorańcze"}
            });
            NavController controller = new NavController(mock.Object);
            string categoryToSelect = "Jabłka";

            var result = controller.Menu(categoryToSelect).ViewBag.SelectedCategory;
            
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void GenerateCategorySpecificProductCount()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {Id = 1, Name = "P1", Category = "Cat1"},
                new Product() {Id = 2, Name = "P2", Category = "Cat2"},
                new Product() {Id = 3, Name = "P3", Category = "Cat1"},
                new Product() {Id = 4, Name = "P4", Category = "Cat2"},
                new Product() {Id = 5, Name = "P5", Category = "Cat3"},
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            int res1 = ((ProductListViewModel) ((ViewResult) controller.List("Cat1")).Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)((ViewResult)controller.List("Cat2")).Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)((ViewResult)controller.List("Cat3")).Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)((ViewResult)controller.List(null)).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
