using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class DutyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
