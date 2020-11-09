using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Aspose.Pdf;
using Lab.Data;
using Lab.Models;
using Lab.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Document = Aspose.Pdf.Document;

namespace Lab.Controllers
{
    //[Authorize(Roles ="manager")]
    public class ManagerController : Controller
    {        
        private readonly DataContext dataContext;

        private readonly UserManager<AppUser> userManager;
        private readonly IPasswordHasher<AppUser> iPasswordHasher;

        public ManagerController(DataContext dataContext, UserManager<AppUser> userManager, IPasswordHasher<AppUser> iPasswordHasher)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.iPasswordHasher = iPasswordHasher;
        }
        public async Task<IActionResult> UpdateInformation(string name)
        {
            AppUser user = await userManager.FindByNameAsync(name);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInformation(string userName, string email, string address,string phoneNumber)
        {
            AppUser user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(address))
                    user.Address = address;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(phoneNumber))
                    user.PhoneNumber = phoneNumber;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(phoneNumber))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }
        public async Task<IActionResult> Update(string name)
        {
            AppUser user = await userManager.FindByNameAsync(name);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string userName, string email, string password)
        {
            AppUser user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = iPasswordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index","Home");
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        public IActionResult Ordered(string name)
        {
            var item = from p in dataContext.OrderDetails
                       .Include(u=>u.Order)
                       .Include(t=>t.Product)
                       .Where(s=>s.Order.Name == name)
                       select p;
            return View(item.ToList());
        }

        [Authorize(Roles ="manager")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FileResult Index(string editor1)
        {
            // create a unique file name
            string fileName = Guid.NewGuid() + ".pdf";

            // convert HTML text to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(editor1);

            // generate PDF from the HTML
            MemoryStream stream = new MemoryStream(byteArray);
            HtmlLoadOptions options = new HtmlLoadOptions();
            Document pdfDocument = new Document(stream, options);

            // create memory stream for the PDF file
            Stream outputStream = new MemoryStream();
            pdfDocument.Save(outputStream);

            // return generated PDF file
            return File(outputStream, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        [Route("generate")]
        public IActionResult generate(string productId)
        {
            ViewBag.productId = productId;
            return View("index");
        }
        [Authorize(Roles = "manager,admin")]
        public IActionResult Order() 
        {
            var item = from p in dataContext.Orders
                       select p;
            return View(item.ToList());
        }
        public IActionResult Delete(int id)
        {
            //ProductModel oldProduct = productData.ProductList.FirstOrDefault(p => p.ProductId == id);
            Order order = dataContext.Orders.FirstOrDefault(p => p.OrderId == id);
            Order oldOrder = new Order()
            {
                OrderId = order.OrderId,
            };
            return View(oldOrder);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //productData.ProductList.RemoveAll(p => p.ProductId == id);
            Order order = dataContext.Orders.FirstOrDefault(p => p.OrderId == id);
            dataContext.Orders.Remove(order);
            dataContext.SaveChanges();
            return RedirectToAction("Order");
        }
        [Authorize(Roles = "manager,admin")]
        public IActionResult OrderDetail(int id)
        {
            var item = from p in dataContext.OrderDetails.Include(u=>u.Product)
                       .Where(s => s.OrderId == id)
                       select p;
            return View(item.ToList());
        }
    }
}
