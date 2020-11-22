using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
namespace HRMS_WEB.Viewmodels
{
    public class DraughtmenViewModel
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public int TotalSublevelCount { get; set; }
        public List<SubLevel> SublevelList { get; set; }

    }
}
