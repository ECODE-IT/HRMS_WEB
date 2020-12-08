using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class LeaveViewModel
    {
        public String Username { get; set; }
        public double TodayWorkedHoursProgress { get; set; }
        public double WeekWorkedHoursProgress { get; set; }
        public double MonthWorkedHoursProgress { get; set; }
    }
}
