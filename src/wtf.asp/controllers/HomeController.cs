using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace wtf.asp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            var factory = new LoggerFactory();
            factory.AddElmahIo("168e8ebc7e4d4755bf9f1038ab8b5ecd", Guid.Parse("4edc9492-8c3c-44bf-a727-53f708f2fba5"));
            factory.CreateLogger("elmah");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
