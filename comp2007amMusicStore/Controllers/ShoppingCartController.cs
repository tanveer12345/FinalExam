using comp2007amMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe.net;
using Stripe;

namespace comp2007amMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        // db conn
        private MusicStoreModel db = new MusicStoreModel();

        // identify the user's cart id with their username or a session-generated value
        //string currentCartId { get; set; }

        // GET: ShoppingCart
        public ActionResult Index()
        {
            // get cart items from db
            if (Session["CartId"] == null)
            {
                // migrate / load the user's cart in case something is in it
                GetCartId();
                //MigrateCart();
            }
            string currentCartId = Session["CartId"].ToString();

            var CartItems = db.Carts.Where(c => c.CartId == currentCartId);

            // calculate cart Total
            if (CartItems.Count() == 0)
            {
                ViewBag.CartTotal = 0;
            }
            else
            {
                decimal? total = (from c in CartItems
                                  select (int?)c.Count * c.Album.Price).Sum();
                ViewBag.CartTotal = total;
            }

            return View(CartItems);
        }

        // GET: /addtocart/5
        public ActionResult AddToCart(int AlbumId)
        {
            // generate CartId if empty
            GetCartId();

            string currentCartId = Session["CartId"].ToString();

            // is album already in cart?
            var cartItem = db.Carts.SingleOrDefault(
                c => c.CartId == currentCartId
                && c.AlbumId == AlbumId);

            if (cartItem == null) { 
            // add the item to the current cart
                cartItem = new Cart
                {
                    AlbumId = AlbumId,
                    Count = 1,
                    CartId = Session["CartId"].ToString(),
                    DateCreated = DateTime.Now
                };

                db.Carts.Add(cartItem);
            }
            else
            {
                // album already in this user's cart, so add 1 to the current count
                cartItem.Count++;
            }

            // save the insert or update
            db.SaveChanges();

            // show the cart view
            return RedirectToAction("Index");
        }

        public void GetCartId()
        {
            //HttpContextBase context = this.HttpContext;

            // create currentCartId if it's empty
            if (Session["CartId"] == null)
            {
                // is user logged in?
                if (User.Identity.Name == "")
                {
                    // if not logged in, generate a new unique id
                    Session["CartId"] = Guid.NewGuid().ToString();
                }
                else
                {
                    Session["CartId"] = User.Identity.Name;
                }

                // store current cart Id in the session object
                //Session["CartId"] = currentCartId;
            }
        }

        // GET: /removefromcart/5
        public ActionResult RemoveFromCart(int AlbumId)
        {
            string currentCartId = Session["CartId"].ToString();

            // get current album from the cart
            var item = db.Carts.SingleOrDefault(c => c.CartId == currentCartId
                && c.AlbumId == AlbumId);

            db.Carts.Remove(item);
            db.SaveChanges();

            // refresh cart page
            return RedirectToAction("Index");
        }

        [Authorize]
        // GET: /checkout
        public ActionResult Checkout()
        {
            MigrateCart();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: /checkout
        public ActionResult Checkout(FormCollection values)
        {
            // create new order from the form inputs
            Order order = new Order();
            TryUpdateModel(order);

            // auto-fill username, email, orderdate, and total
            order.Username = User.Identity.Name;
            order.Email = User.Identity.Name;
            order.OrderDate = DateTime.Now;

            string currentCartId = Session["CartId"].ToString();
            var CartItems = db.Carts.Where(c => c.CartId == currentCartId);

            decimal total = (from c in CartItems
                              select (int)c.Count * c.Album.Price).Sum();

            order.Total = total;

            // store entire order object in a session variable
            Session["Order"] = order;

            return RedirectToAction("Payment");
        }

        [Authorize]
        // GET: /payment
        public ActionResult Payment()
        {
            // get the order total
            Order order = Session["Order"] as Order;
            ViewBag.Total = order.Total;
            ViewBag.CentsTotal = order.Total * 100;
            ViewBag.StripePublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: /payment
        public ActionResult Payment(string stripeEmail, string stripeToken)
        {
            // set secret key
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["StripeSecretKey"]);

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();
            Order order = Session["Order"] as Order;


            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = Convert.ToInt16(order.Total * 100),
                Description = "MVC Music Store Purchase",
                Currency = "cad",
                CustomerId = customer.Id
            });

            // save the order
            db.Orders.Add(order);
            db.SaveChanges();

            // save the items
            var CartItems = db.Carts.Where(c => c.CartId == order.Username);

            foreach (Cart item in CartItems)
            {
                var OrderDetail = new OrderDetail();
                OrderDetail.OrderId = order.OrderId;
                OrderDetail.AlbumId = item.AlbumId;
                OrderDetail.Quantity = item.Count;
                OrderDetail.UnitPrice = item.Album.Price;
                db.OrderDetails.Add(OrderDetail);
            }
            db.SaveChanges();

            EmptyCart();

            return RedirectToAction("Details", "Orders", new { id = order.OrderId });
        }

        public void EmptyCart()
        {
            string CurrentCartId = User.Identity.Name;

            var CartItems = db.Carts.Where(c => c.CartId == CurrentCartId);

            foreach (Cart item in CartItems)
            {
                db.Carts.Remove(item);
            }

            db.SaveChanges();
        }

        public void MigrateCart()
        {
            /* if user has items in their cart, 
            update the CartId to their username when they log in */

            string currentCartId = Session["CartId"].ToString();

            if (currentCartId != User.Identity.Name)
            {
                // get current cart contents

                var cartItems = db.Carts.Where(c => c.CartId == currentCartId);

                // change the session & the db to use the username for this cart
                Session["CartId"] = User.Identity.Name;
                foreach(Cart item in cartItems)
                {
                    item.CartId = User.Identity.Name;
                }
                db.SaveChanges();
            }
        }
    }
}