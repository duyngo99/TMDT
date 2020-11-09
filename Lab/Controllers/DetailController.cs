using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab.Data;
using Lab.Models;
using Lab.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Controllers
{
    public class DetailController : Controller
    {
        private readonly DataContext dataContext;
        public DetailController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        [HttpGet("Detail/{id}/{name}")]
        public IActionResult Index(int id)
        {
            Product product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);

            var category = dataContext.Categories.FirstOrDefault(p => p.CategoryId == product.CategoryId);
            ViewBag.Category = category.CategoryName;
            return View(product);
        }
    }
}
