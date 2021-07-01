using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Salary
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public double Basic { get; set; }
        public double OTHourRate { get; set; }
    }
}
