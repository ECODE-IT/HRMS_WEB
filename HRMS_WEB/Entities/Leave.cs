using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Leave
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public String Reason { get; set; }
        public int IsApproved { get; set; }
        public String UserId { get; set; }
        public String ApprovedId { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationUser Approved { get; set; }
    }
}
