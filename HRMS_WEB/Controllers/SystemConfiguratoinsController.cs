using ExcelDataReader;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<SystemConfigurationsController> logger;

        public SystemConfigurationsController(HRMSDbContext db, IHostingEnvironment hostingEnvironment, ILogger<SystemConfigurationsController> logger)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
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


        [HttpPost]
        public async Task<IActionResult> UpdateHolidays(SystemSettings systemSettings)
        {
            string holidayFileName = null;
            if(systemSettings.HolidaysFile != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "holidays");
                //holidayFileName = "holidaysformat" + Path.GetExtension(systemSettings.HolidaysFile.FileName);
                //string filePath = Path.Combine(uploadsFolder, holidayFileName);
                //await systemSettings.HolidaysFile.CopyToAsync(new FileStream(filePath, FileMode.Create));

               
                    IExcelDataReader reader;

                    reader = ExcelDataReader.ExcelReaderFactory.CreateReader(systemSettings.HolidaysFile.OpenReadStream());

                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var dataSet = reader.AsDataSet(conf);
                var tableData = dataSet.Tables[0];

                List<Holiday> holidayList = new List<Holiday>();


                for (int i = 0; i< tableData.Rows.Count; i++)
                {
                    var holiday = new Holiday() { 
                        Date = tableData.Rows[i].Field<DateTime>(0),
                        Name = tableData.Rows[i].Field<string>(1), 
                        Remark = tableData.Rows[i].Field<string>(2)
                    };
                    holidayList.Add(holiday);
                }
                var holidays = db.Holidays.ToList();
                db.Holidays.RemoveRange(holidays);

                await db.Holidays.AddRangeAsync(holidayList);

                await db.SaveChangesAsync();
                    
                }
                
            
            return Redirect("/SystemConfigurations/GetSystemConfigurations");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateSalary(SystemSettings systemSettings)
        {
            
            if (systemSettings.SalaryFile != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "holidays");
                //holidayFileName = "holidaysformat" + Path.GetExtension(systemSettings.HolidaysFile.FileName);
                //string filePath = Path.Combine(uploadsFolder, holidayFileName);
                //await systemSettings.HolidaysFile.CopyToAsync(new FileStream(filePath, FileMode.Create));


                IExcelDataReader reader;

                reader = ExcelDataReader.ExcelReaderFactory.CreateReader(systemSettings.SalaryFile.OpenReadStream());

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(conf);
                var tableData = dataSet.Tables[0];

                List<Salary> SalaryList = new List<Salary>();


                for (int i = 0; i < tableData.Rows.Count; i++)
                {
                    var salary = new Salary()
                    {
                        Username = tableData.Rows[i].Field<string>(0),
                        Basic = tableData.Rows[i].Field<double>(1),
                        OTHourRate = tableData.Rows[i].Field<double>(2)
                    };
                    SalaryList.Add(salary);
                }
                var salaries = db.Salary.ToList();
                db.Salary.RemoveRange(salaries);

                await db.Salary.AddRangeAsync(SalaryList);

                await db.SaveChangesAsync();

            }


            return Redirect("/SystemConfigurations/GetSystemConfigurations");

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
