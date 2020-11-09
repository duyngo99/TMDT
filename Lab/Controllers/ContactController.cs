using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lab.Helpers;
using Lab.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lab.Controllers
{
    public class ContactController : Controller
    {
        private IConfiguration configuration;
        private IWebHostEnvironment webHostEnvironment;
        public ContactController(IConfiguration _configuration,
            IWebHostEnvironment _webHostEnvironment)
        {
            configuration = _configuration;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View("Index",new Contact());
        }
        [HttpPost]
        public IActionResult Send(Contact contact,IFormFile[] attachments)
        {
            var body = "Name: " + contact.Name + "<br>Address: " + contact.Address + "<br>Phone: " + contact.Phone + "<br>Content: " + contact.Content + "<br>";
            var mailHelper = new MailHelper(configuration);
            List<string> fileNames = null;
            if (attachments != null && attachments.Length > 0)
            {
                fileNames = new List<string>();
                foreach (IFormFile attachment in attachments)
                {
                    var path = Path.Combine(webHostEnvironment.WebRootPath,"uploads",attachment.FileName);
                    using (var stream = new FileStream(path,FileMode.Create))
                    {
                        attachment.CopyToAsync(stream);
                    }
                    fileNames.Add(path);
                }
            }
            if (mailHelper.Send(contact.Email, configuration["Gmail:Username"], contact.Subject, body, fileNames))
            {
                ViewBag.msg = "Sent Mail Successfully";
            }
            else
            {
                ViewBag.msg = "Failed";
            }
            return View("Index", new Contact());
        }
        
    }
}
