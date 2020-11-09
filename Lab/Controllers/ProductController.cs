using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lab.Data;
using Lab.Models;
using Lab.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab.Controllers
{
    
    public class ProductController : Controller
    {
        private readonly ProductData productData;
        private readonly DataContext dataContext;
        public ProductController(ProductData productData, DataContext dataContext)
        {
            this.productData = productData;
            this.dataContext = dataContext;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Index()
        {
            List<ProductModel> products = new List<ProductModel>();
            List<Product> productss = dataContext.Products.ToList();
            foreach (var item in productss)
            {
                products.Add(new ProductModel()
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    ProductQuantity = item.ProductQuantity,
                    ProductPrice = item.ProductPrice,
                    CreateDate = DateTime.Now,
                    CategoryId = 1
                });
            }
            return View(products);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Add()
        {
            ProductModel productModel = new ProductModel();
            ViewData["CategoryId"] = new SelectList(dataContext.Categories, "CategoryId", "CategoryName");
            return View(productModel);
        }
        [HttpPost]
        public IActionResult Add(ProductModel productModel, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                ProductModel newProduct = new ProductModel();
                if (photo == null || photo.Length == 0)
                {
                    newProduct.ProductImage = "abc.png";
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photo.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    photo.CopyToAsync(stream);
                    newProduct.ProductImage = photo.FileName;
                }
                newProduct.ProductName = productModel.ProductName;
                newProduct.ProductQuantity = productModel.ProductQuantity;
                newProduct.ProductPrice = productModel.ProductPrice;
                newProduct.CreateDate = DateTime.Now;
                //productData.ProductList.Add(newProduct);
                Product p = new Product()
                {
                    ProductName = newProduct.ProductName,
                    ProductImage = newProduct.ProductImage,
                    ProductQuantity = newProduct.ProductQuantity,
                    ProductPrice = newProduct.ProductPrice,
                    CategoryId = 1
                };
                dataContext.Products.Add(p);
                dataContext.SaveChanges();
                return RedirectToAction("Index", "Product");
            }
            else
            {
                return View(productModel);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            //ProductModel oldProduct = productData.ProductList.FirstOrDefault(p => p.ProductId == id);
            Product product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);
            ProductModel oldProduct = new ProductModel()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName

            };

            return View(oldProduct);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                //ProductModel oldProduct = productData.ProductList.FirstOrDefault(p => p.ProductId == id);
                //oldProduct.ProductName = productModel.ProductName;
                //oldProduct.ProductQuantity = productModel.ProductQuantity;
                //oldProduct.ProductPrice = productModel.ProductPrice;
                //oldProduct.CreateDate = DateTime.Now;
                Product p = dataContext.Products.FirstOrDefault(p => p.ProductId == id);
                p.ProductName = productModel.ProductName;
                p.ProductQuantity = productModel.ProductQuantity;
                p.ProductPrice = productModel.ProductPrice;
                dataContext.SaveChanges();
                ViewBag.Status = 1;
            }
            return View(productModel);
        }
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            //ProductModel oldProduct = productData.ProductList.FirstOrDefault(p => p.ProductId == id);
            Product product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);
            ProductModel oldProduct = new ProductModel()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName
            };
            return View(oldProduct);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //productData.ProductList.RemoveAll(p => p.ProductId == id);
            Product product = dataContext.Products.FirstOrDefault(p => p.ProductId == id);
            dataContext.Products.Remove(product);
            dataContext.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
    }
}
