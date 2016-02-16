using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void CanLoginWithValidCredentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "sekret")).Returns(true);
            LoginViewModel model = new LoginViewModel(){UserName = "admin", Password = "sekret"};
            AccountController controller = new AccountController(mock.Object);

            ActionResult result = controller.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void CannotLoginWithInvalidCredentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("zlyAdmin", "zlySekret")).Returns(false);
            LoginViewModel model = new LoginViewModel() { UserName = "zlyAdmin", Password = "zlySekret" };
            AccountController controller = new AccountController(mock.Object);

            ActionResult result = controller.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
