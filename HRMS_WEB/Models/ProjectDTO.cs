using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class ProjectDTO
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public DateTime AssignedDateTime { get; set; }
        public String UserID { get; set; }
        public String Customer { get; set; }
        public DateTime Deadline { get; set; }
        public String Remarks { get; set; }
        public double Progress { get; set; }
        public bool IsFinished { get; set; }

        public List<String> SubLevelNameList { get; set; }
    }
}
