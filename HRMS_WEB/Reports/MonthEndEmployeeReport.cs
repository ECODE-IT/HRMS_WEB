using System;
using DevExpress.XtraReports.UI;
using HRMS_WEB.Models;

namespace HRMS_WEB.Reports
{
    public partial class MonthEndEmployeeReport
    {
        public MonthEndEmployeeReport(MonthEndEmployeeDTO obj)
        {
            InitializeComponent();
            objectDataSource1.DataSource = obj;
        }
    }
}
