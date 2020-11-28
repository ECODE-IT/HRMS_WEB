using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class LeaveDTO
    {
        public String Date { get; set; }
        public String Reason { get; set; }
        public int IsApproved { get; set; }
        public String UserId { get; set; }
        public String ApprovedId { get; set; }
    }
}
