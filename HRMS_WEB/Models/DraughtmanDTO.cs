using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class DraughtmanDTO
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String PhoneNumber { get; set; }
        public double ManhourCount { get; set; }
        public int subLevelCount { get; set; }
    }
}
