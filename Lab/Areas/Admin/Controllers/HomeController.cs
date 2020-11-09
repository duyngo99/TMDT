using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
    {
        [Authorize(Roles ="admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
