using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using comp2007amMusicStore.Controllers;
using System.Web.Mvc;
using Moq;
using comp2007amMusicStore.Models;
using System.Linq;

namespace comp2007amMusicStore.Tests.Controllers
{
    /// <summary>
    /// Summary description for OrdersControllerTest
    /// </summary>
    [TestClass]
    public class OrdersControllerTest
    {
        OrdersController controller;
        List<Order> orders;
        Mock<IMockOrdersRepository> mock;

        // set up mock data for all unit tests - runs automatically
        [TestInitialize]
        public void TestInitialize()
        {
            // instantiate new mock object
            mock = new Mock<IMockOrdersRepository>();

            // mock order data
            orders = new List<Order>
            {
                new Order { OrderId = 1, FirstName = "First", LastName = "Customer" },
                new Order { OrderId = 2, FirstName = "Second", LastName = "Customer" },
                new Order { OrderId = 3, FirstName = "Third", LastName = "Customer" }
            };

            // populate the mock repo with the mock data
            mock.Setup(m => m.Orders).Returns(orders.AsQueryable());

            // inject the mock dependency when calling the controller's constructor
            controller = new OrdersController(mock.Object);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void IndexViewLoads()
        {
            // see if Orders/index loads successfully
            // arrange - now happens in TestInitialize()
           // OrdersController controller = new OrdersController();

            // act
            ViewResult result = controller.Index() as ViewResult;

            // assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexLoadsOrders()
        {
            // act - cast ActionResult returntype to ViewResult to access the model
            var actual = (List<Order>)((ViewResult)controller.Index()).Model;

            // assert
            CollectionAssert.AreEqual(orders, actual);
        }

        [TestMethod]
        public void DetailsValidId()
        {
            // act
            var actual = ((ViewResult)controller.Details(1)).Model;

            // assert
            Assert.AreEqual(orders[0], actual);
        }

        [TestMethod]
        public void DetailsInvalidId()
        {
            // act
            //var actual = ((ViewResult)controller.Details(4)).Model;
            var actual = (ViewResult)controller.Details(4);

            // assert
            //Assert.IsNull(actual);
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void DetailsNoId()
        {
            // act
            var actual = (ViewResult)controller.Details(null);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        // GET: Edit
        [TestMethod]
        public void EditGetValidId()
        {
            // act
            var actual = ((ViewResult)controller.Edit(1)).Model;

            // assert
            Assert.AreEqual(orders[0], actual);
        }

        [TestMethod]
        public void EditGetInvalidId()
        {
            // act
            var actual = (ViewResult)controller.Edit(4);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void EditGetNoId()
        {
            // assert - must pass an int so the overload calls G not P
            int? id = null;

            // act
            var actual = (ViewResult)controller.Edit(id);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        // POST: Edit
        [TestMethod]
        public void EditPostValid()
        {
            // act - pass in the first moq ordr object
            var actual = (RedirectToRouteResult)controller.Edit(orders[0]);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["actionName"]);
        }

        [TestMethod]
        public void EditPostInvalid()
        {
            // arrange - manually set the model state to invalid
            controller.ModelState.AddModelError("key", "unit test error");

            // act - pass in the first mock order object
            var actual = (ViewResult)controller.Edit(orders[0]);

            // assert
            Assert.AreEqual("Edit", actual.ViewName);
        }

        // GET: Create
        [TestMethod]
        public void CreateViewLoads()
        {
            // act
            var actual = (ViewResult)controller.Create();

            // assert
            Assert.AreEqual("Create", actual.ViewName);
        }

        // POST: Create
        [TestMethod]
        public void CreatePostValid()
        {
            // arrange
            Order o = new Order
            {
                FirstName = "Some",
                LastName = "Customer"
            };

            // act
            var actual = (RedirectToRouteResult)controller.Create(o);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["actionName"]);
        }

        [TestMethod]
        public void CreatePostInvalid()
        {
            // arrange
            Order o = new Order
            {
                FirstName = "Some",
                LastName = "Customer"
            };

            controller.ModelState.AddModelError("key", "cannot add order");

            // act
            var actual = (ViewResult)controller.Create(o);

            // assert
            Assert.AreEqual("Create", actual.ViewName);
        }

        // GET: Delete
        [TestMethod]
        public void DeleteValidId()
        {
            // act
            var actual = ((ViewResult)controller.Delete(1)).Model;

            // assert
            Assert.AreEqual(orders[0], actual);
        }

        [TestMethod]
        public void DeleteInvalidId()
        {
            // act
            var actual = (ViewResult)controller.Delete(4);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        [TestMethod]
        public void DeleteNoId()
        {
            // act
            var actual = (ViewResult)controller.Delete(null);

            // assert
            Assert.AreEqual("Error", actual.ViewName);
        }

        // POST: Delete
        [TestMethod]
        public void DeletePostValid()
        {
            // act
            var actual = (RedirectToRouteResult)controller.DeleteConfirmed(34);

            // assert
            Assert.AreEqual("Index", actual.RouteValues["actionName"]);
        }
    }
}
