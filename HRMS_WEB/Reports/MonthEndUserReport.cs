using System;
using DevExpress.XtraReports.UI;
using HRMS_WEB.Models;

namespace HRMS_WEB.Reports
{
    public partial class MonthEndUserReport
    {
        public MonthEndUserReport(UserMonthEndSummaryDTO dto)
        {
            InitializeComponent();
            objectDataSource1.DataSource = dto;
        }
    }
}
