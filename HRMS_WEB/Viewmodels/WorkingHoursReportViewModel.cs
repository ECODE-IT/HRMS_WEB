using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class WorkingHoursReportViewModel
    {
        public String Username { get; set; }
        public double WorkedHours { get; set; }
        public bool AcheivedTheTarget { get; set; }
    }
}
