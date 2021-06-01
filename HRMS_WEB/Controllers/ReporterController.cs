using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using HRMS_WEB.Reports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Controllers
{
    public class ReporterController : Controller
    {
        public IActionResult Viewer()
        {
            ViewBag.report = new Samplereport();
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
    }
}
