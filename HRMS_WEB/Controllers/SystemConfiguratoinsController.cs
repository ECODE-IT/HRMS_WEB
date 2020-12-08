using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Controllers
{
    public class SystemConfigurationsController : Controller
    {
        private readonly HRMSDbContext db;

        public SystemConfigurationsController(HRMSDbContext db)
        {
            this.db = db;
        }

        public IActionResult GetSystemConfigurations()
        {
            return View(db.SystemSettings.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSystemConfigurations(SystemSettings systemSettings)
        {
            if (systemSettings != null)
            {
                db.SystemSettings.Update(systemSettings);
                await db.SaveChangesAsync();
                return RedirectToAction("GetSystemConfigurations");
            }
            throw new Exception("Empty submission of data from the form");
        }

    }
}
