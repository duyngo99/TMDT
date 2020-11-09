using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lab.Data;
using Lab.Helpers;
using Lab.Models;
using Lab.Models.Domain;
using Lab.PayPalHelper;
using Lab.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lab.Controllers
{
    public class CartController : Controller
    {
        //public IConfiguration configuration { get; }
        //public CartController(IConfiguration _configuration)
        //{
        //    configuration = _configuration;
        //}


        private readonly DataContext dataContext;
        private readonly UserManager<AppUser> userManager;
        public CartController(DataContext dataContext, UserManager<AppUser> userManager)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
        }

        


        //[HttpPost]
        //public async Task<IActionResult> paypal(double total)
        //{
        //    var payPalAPI = new PayPalAPI(configuration);
        //    string url = await payPalAPI.getRedirectURLToPayPal(total, "USD");
        //    return Redirect(url);
        //}
        //public async Task<IActionResult> Success([FromQuery(Name = "paymentId")] string paymentId, [FromQuery(Name = "PayerID")] string payerID)
        //{
        //    var payPalAPI = new PayPalAPI(configuration);
        //    PaypalPaymentExecutedResponse result = await payPalAPI.executedPayment(paymentId, payerID);
        //    Debug.WriteLine("Transaction Details");
        //    Debug.WriteLine("cart " + result.cart);
        //    Debug.WriteLine("create_time: " + result.create_time.ToLongDateString());
        //    Debug.WriteLine("id: " + result.id);
        //    Debug.WriteLine("intent: " + result.intent);
        //    Debug.WriteLine("links 0 - href: " + result.links[0].href);
        //    Debug.WriteLine("links 0 - method: " + result.links[0].method);
        //    Debug.WriteLine("links 0 - rel: " + result.links[0].rel);
        //    Debug.WriteLine("payer_info - first_name: " + result.payer.payer_info.first_name);
        //    Debug.WriteLine("payer_info - last_name: " + result.payer.payer_info.last_name);
        //    Debug.WriteLine("payer_info - billing_address: " + result.payer.payer_info.billing_address);
        //    Debug.WriteLine("payer_info - shipping_address: " + result.payer.payer_info.shipping_address);
        //    Debug.WriteLine("payer_info - payer_id: " + result.payer.payer_info.payer_id);
        //    Debug.WriteLine("state " + result.state);
        //    ViewBag.result = result;
        //    return View("Success");
        //}



        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                return View();
            }
            else
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Product.ProductPrice * item.Quantity);
                return View();
            }
        }
        [Route("buy/{id}")]
        public IActionResult Buy(int id)
        {
            if (SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart") == null)
            {
                List<OrderDetail> cart = new List<OrderDetail>();
                cart.Add(new OrderDetail
                {
                    Product = dataContext.Products.FirstOrDefault(p => p.ProductId == id),
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<OrderDetail> cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new OrderDetail
                    {
                        Product = dataContext.Products.FirstOrDefault(p => p.ProductId == id),
                        Quantity = 1
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<OrderDetail> cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        private int isExist(int id)
        {
            List<OrderDetail> cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
            if (cart == null)
                return View();
            else
            {
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Product.ProductPrice * item.Quantity);
            }
            Order order = new Order();
            return View(order);
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (ModelState.IsValid)
            {
                List<OrderDetail> cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
                Order tempOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    PhoneNumber = order.PhoneNumber,
                    Address = order.Address,
                    Email = order.Email,
                    Name = order.Name
                };

                dataContext.Orders.Add(tempOrder);
                dataContext.SaveChanges();

                var query = dataContext.Orders.FirstOrDefault(p => p.OrderId == tempOrder.OrderId);

                foreach (var item in cart)
                {
                    dataContext.OrderDetails.Add(new OrderDetail()
                    {
                        OrderId = query.OrderId,
                        ProductId = item.Product.ProductId,
                        Quantity = item.Quantity,
                    });
                }
                dataContext.SaveChanges();
                cart.Clear();

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

                return RedirectToAction("Confirm", "Cart");
            }
            return View(order);
        }

        public IActionResult Confirm()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CheckoutWithLogin(string name)
        {
            AppUser user = await userManager.FindByNameAsync(name);
            if (user != null)
            {
                var cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
                if (cart == null)
                    return View();
                else
                {
                    ViewBag.cart = cart;
                    ViewBag.total = cart.Sum(item => item.Product.ProductPrice * item.Quantity);
                }
                return View(user);
            }                
            else
                return RedirectToAction("Index");            
        }

        [HttpPost, ActionName("CheckoutWithLogin")]
        public async Task<IActionResult> CheckoutWithLoginPost(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    List<OrderDetail> cart = SessionHelper.GetObjectFromJson<List<OrderDetail>>(HttpContext.Session, "cart");
                    Order tempOrder = new Order
                    {
                        OrderDate = DateTime.Now,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        Email = user.Email,
                        Name = user.UserName
                    };

                    dataContext.Orders.Add(tempOrder);
                    dataContext.SaveChanges();

                    var query = dataContext.Orders.FirstOrDefault(p => p.OrderId == tempOrder.OrderId);

                    foreach (var item in cart)
                    {
                        dataContext.OrderDetails.Add(new OrderDetail()
                        {
                            OrderId = query.OrderId,
                            ProductId = item.Product.ProductId,
                            Quantity = item.Quantity,
                        });
                    }
                    dataContext.SaveChanges();
                    cart.Clear();

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

                    return RedirectToAction("Confirm", "Cart");
                }
            }
            return View();
            

        }

    }
}
