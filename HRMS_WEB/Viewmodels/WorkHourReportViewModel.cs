using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class WorkHourReportViewModel
    {
        public IEnumerable<LeaveViewModel> LeaveViewModels { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
}
