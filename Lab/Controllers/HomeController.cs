using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lab.Models;
using Microsoft.AspNetCore.Authorization;
using Lab.Data;

namespace Lab.Controllers
{    
    public class HomeController : Controller
    {
        private readonly DataContext dataContext;
        public HomeController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IActionResult Index()
        {
            var item = from p in dataContext.Products
                       select p;
            return View(item.ToList());
        }

        public ActionResult Find(string pid)
        {
            int x;
            bool resl = int.TryParse(pid, out x);
            if (resl == true)
            {
                var det = (from d in dataContext.Products
                           where d.ProductId == x
                           select d).ToList();
                return View("index", det);
            }
            else
            {
                var det = (from d in dataContext.Products
                           where d.ProductName == pid
                           select d).ToList();
                return View("index", det);
            }
        }

        public ActionResult DescendingPrices()
        {
            var det = dataContext.Products
                .OrderByDescending(p => p.ProductPrice)
                .ToList();
            return View("index", det);
        }

        public ActionResult AscendingPrices()
        {
            var det = dataContext.Products
                .OrderBy(p => p.ProductPrice)
                .ToList();
            return View("index", det);
        }

        public ActionResult Iphone(string type)
        {
            var det = dataContext.Products
                .Where(p => p.Category.CategoryName == type)
                .ToList();
            return View("index", det);
        }
        public ActionResult Samsung(string type)
        {
            var det = dataContext.Products
                .Where(p => p.Category.CategoryName == type)
                .ToList();
            return View("index", det);
        }
    }
}
