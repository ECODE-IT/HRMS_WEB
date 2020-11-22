using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class ProjectDataDTO
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Customer { get; set; }
        public DateTime Deadline { get; set; }
        public double Progress { get; set; }
        public int SubLevelCount { get; set; }
        public int UserCount { get; set; }
        public DateTime FinishedDate { get; set; }
    }
}
