using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class SubLevel
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public int ProjectID { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        public DateTime Deadline { get; set; }
        public double ManHours { get; set; }
        public double progressFraction { get; set; }
        public int PriorityLevel { get; set; }

        // navigation properties
        public ApplicationUser User { get; set; }
    }
}
