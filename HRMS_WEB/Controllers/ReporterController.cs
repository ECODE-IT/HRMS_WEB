using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using HRMS_WEB.Models;
using HRMS_WEB.Reports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HRMS_WEB.Controllers
{
    public class ReporterController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;
        private static String query;

        public ReporterController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult AvailableReports()
        {
            var pathfactor = "\\";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                pathfactor = "/";
            }
                var folderpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
            var reportnamelist = new List<string>();
            foreach (String file in Directory.EnumerateFiles(folderpath, "*", SearchOption.TopDirectoryOnly))
            {

                var filename = file.Substring(file.LastIndexOf(pathfactor) + 1);

                if (filename.Contains("ecodex"))
                {
                    reportnamelist.Add(filename.Replace(".ecodex", ""));
                }
            }
            ViewBag.reportnamelist = reportnamelist;
            return View();
        }

        public IActionResult Viewer(String sql)
        {
            return View();
        }

        public ActionResult Export(string format = "pdf")
        {
            format = format.ToLower();
            Samplereport report = ViewBag.report;
            string contentType = string.Format("application/{0}", format);
            using (MemoryStream ms = new MemoryStream())
            {
                switch (format)
                {
                    case "pdf":
                        contentType = "application/pdf";
                        report.ExportToPdf(ms);
                        break;
                    case "docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        report.ExportToDocx(ms);
                        break;
                    case "xls":
                        contentType = "application/vnd.ms-excel";
                        report.ExportToXls(ms);
                        break;
                    case "xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        report.ExportToXlsx(ms);
                        break;
                    case "rtf":
                        report.ExportToRtf(ms);
                        break;
                    case "mht":
                        contentType = "message/rfc822";
                        report.ExportToMht(ms);
                        break;
                    case "html":
                        contentType = "text/html";
                        report.ExportToHtml(ms);
                        break;
                    case "txt":
                        contentType = "text/plain";
                        report.ExportToText(ms);
                        break;
                    case "csv":
                        contentType = "text/plain";
                        report.ExportToCsv(ms);
                        break;
                    case "png":
                        contentType = "image/png";
                        report.ExportToImage(ms, new ImageExportOptions() { Format = System.Drawing.Imaging.ImageFormat.Png });
                        break;
                }
                return File(ms.ToArray(), contentType);
            }
        }

        public DataSet getDataset(String sql, string sreportname = "Dynamic report")
        {
            using (var conn = new MySqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    // open the database connection
                    conn.Open();

                    // mysql command
                    var command = new MySqlCommand(sql, conn);

                    // database reader
                    var reader = command.ExecuteReader();

                    // load data and store
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    DataSet dataSet1 = new DataSet();
                    dataSet1.DataSetName = sreportname;

                    dataSet1.Tables.Add(dataTable);

                    return dataSet1;

                }
                catch (Exception ex)
                {
                    return new DataSet();
                }
            }
        }

        public IActionResult GenerateReport(String xmlname)
        {
            var reportpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
            var folderpath = Path.Combine(reportpath, xmlname);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReportRoot));
            using (StreamReader stream = new StreamReader(folderpath))
            {
                ReportRoot input = (ReportRoot)xmlSerializer.Deserialize(stream);

                IEnumerable<string> itemlist = input.EParameters.Select(p => p.bindingName + ":" + p.type + ":" + p.name);
                bool hasparams = bool.Parse(input.Query.hasParams);
                query = input.Query.value;
                String reportname = input.ReportName;
                String filename = input.FileName;

                ViewBag.formItems = itemlist;
                ViewBag.hasparams = hasparams;
                ViewBag.reportname = reportname;
                ViewBag.filename = filename;
                stream.Close();

            }

            return View();

        }

        public IActionResult ReturnReport(String args, String sql, bool hasparams, String filename, String reportname)
        {
            sql = query;
            try

            {
                if (hasparams && args != null)
                {
                    List<String> parameters = new List<string>();
                    if (args.Contains(","))
                    {
                        parameters = args.Split(",").ToList();
                    }
                    else
                    {
                        parameters.Add(args);
                    }

                    Dictionary<string, string> paramDic = new Dictionary<string, string>();

                    foreach (string complexpara in parameters)
                    {
                        if (!paramDic.ContainsKey(complexpara.Split("*")[2]))
                        {
                            paramDic.Add(complexpara.Split("*")[2], complexpara.Split("*")[0]);
                        }
                    }

                    foreach (string key in paramDic.Keys)
                    {
                        sql = sql.Replace("@" + key, paramDic[key]);
                    }

                }

                Type t = Type.GetType($"HRMS_WEB.Reports.{filename}");
                ConstructorInfo constructorInfo = t.GetConstructors()[0];
                constructorInfo.Invoke(new object[] {  });
                XtraReport report = (XtraReport)Activator.CreateInstance(t);
                DataSet ds = getDataset(sql, sreportname: reportname);
                report.DataSource = ds;
                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    report.DataMember = ds.Tables[0].TableName;
                }
                ViewBag.report = report;

            }
            catch (Exception ex)
            {

            }
            return View("/Views/Reporter/Viewer.cshtml");
        }

    }
}
