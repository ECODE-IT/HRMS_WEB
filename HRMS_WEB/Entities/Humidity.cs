using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Humidity
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public double RH { get; set; }
        public double Temp { get; set; }
        public double RH2 { get; set; }
        public double Temp2 { get; set; }
    }
}
