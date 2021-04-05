using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public IActionResult BackupDatabase()
        {
            string binary = @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe";
            string arguments = @"mysqldump -u root -p root123 -h localhost hrms_db";
            ProcessStartInfo PSI = new System.Diagnostics.ProcessStartInfo(binary, arguments);
            PSI.RedirectStandardInput = true;
            PSI.RedirectStandardOutput = true;
            PSI.RedirectStandardError = true;
            PSI.UseShellExecute = true;
            Process p = System.Diagnostics.Process.Start(PSI);
            Encoding encoding = p.StandardOutput.CurrentEncoding;
            System.IO.StreamWriter SW = new StreamWriter(@"d:\ssss5.sql", false, encoding);
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            SW.Write(output);
            SW.Close();
            return RedirectToAction("GetSystemConfigurations");
        }
    }
}
