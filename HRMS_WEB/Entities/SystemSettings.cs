using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class SystemSettings
    {
        public int ID { get; set; }
        public double MonthlyTargetHours { get; set; }
        public double DailyTargetHours { get; set; }
    }
}
