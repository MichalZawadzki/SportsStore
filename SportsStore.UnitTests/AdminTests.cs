using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void IndexContainsAllProducts()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                new Product(){Id = 2, Name = "P2"},
                new Product(){Id = 3, Name = "P3"}
            });
            AdminController controller = new AdminController(mock.Object);

            Product[] result = ((IEnumerable<Product>)((ViewResult)controller.Index()).ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void CanEditProduct()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                new Product(){Id = 2, Name = "P2"},
                new Product(){Id = 3, Name = "P3"}
            });
            AdminController controller = new AdminController(mock.Object);

            Product p1 = controller.Edit(1).ViewData.Model as Product;
            Product p2 = controller.Edit(2).ViewData.Model as Product;
            Product p3 = controller.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.Id);
            Assert.AreEqual(2, p2.Id);
            Assert.AreEqual(3, p3.Id);
        }

        [TestMethod]
        public void CannotEditNonexistentProduct()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                new Product(){Id = 2, Name = "P2"},
                new Product(){Id = 3, Name = "P3"}
            });
            AdminController controller = new AdminController(mock.Object);

            Product result = (Product) controller.Edit(4).ViewData.Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanSaveValidChanges()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController controller = new AdminController(mock.Object);
            Product product = new Product(){Name = "Test"};

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(product));
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanSaveInvalidChanges()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController controller = new AdminController(mock.Object);
            Product product = new Product() { Name = "Test" };
            controller.ModelState.AddModelError("error", "error");
            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanDeleteValidProducts()
        {
            Product prod = new Product(){Id = 2, Name = "Test"};
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product(){Id = 1, Name = "P1"},
                prod,
                new Product(){Id = 3, Name = "P3"}
            });
            AdminController controller = new AdminController(mock.Object);

            controller.Delete(prod.Id);

            mock.Verify(m => m.DeleteProduct(prod.Id));
        }
    }
}
