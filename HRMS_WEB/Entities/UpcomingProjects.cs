using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class UpcomingProjects
    {
        public int ID { get; set; }
        public String AssigedUserId { get; set; }
        public DateTime Deadline { get; set; }
        public String Code { get; set; }
        public String Remark { get; set; }
        public String Name { get; set; }
        public String Customer { get; set; }
        public bool IsNotified { get; set; }

        public ApplicationUser AssigedUser { get; set; }
    }
}
