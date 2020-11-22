using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class SubLevelSubmissionDTO
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public String Deadline { get; set; }
        public String Remark { get; set; }
        public double ManHours { get; set; }
        public int PriorityLevel { get; set; }
    }
}
