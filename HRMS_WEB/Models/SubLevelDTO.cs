using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class SubLevelDTO
    {
        public int SubLevelID { get; set; }
        public String UserID { get; set; }
        public String Remark { get; set; }
        public String Deadline { get; set; }
        public double ManHours { get; set; }
        public int UrgentLevel { get; set; }
    }
}
