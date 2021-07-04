using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class SecondaryProjectLog
    {
        public int ID { get; set; }
        public DateTime LogDateTime { get; set; }
        public DateTime LogDate { get; set; }
        public String UserId { get; set; }
        public int SecondaryProjectId { get; set; }
        public String Remarks { get; set; }

        public ApplicationUser User { get; set; }
        public SecondaryProject SecondaryProject { get; set; }
    }
}
