using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class SpecialTask
    {
        public int ID { get; set; }
        public String UserID { get; set; }
        public int ProjectID { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsFinished { get; set; }

        public ApplicationUser User { get; set; }
        public Project Project { get; set; }

    }
}
