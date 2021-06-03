using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using HRMS_WEB.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Controllers
{
    public class ReporterController : Controller
    {
        private readonly IConfiguration configuration;

        public ReporterController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Viewer(String sql)
        {
            ViewBag.report = CreateReport(sql);
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

        public DataSet getDataset(String sql)
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
                    dataSet1.DataSetName = "nwindDataSet1";

                    dataSet1.Tables.Add(dataTable);

                    return dataSet1;

                }
                catch (Exception ex)
                {
                    return new DataSet();
                }
            }
        }

        XtraReport CreateReport(String sql)
        {
            // Create a dataset.           
            DataSet ds = getDataset(sql);
            // Define a report
            Samplereport report = new Samplereport()
            {
                DataSource = ds,
                DataMember = ds.Tables[0].TableName,
            };
            return report;
        }

    }
}
