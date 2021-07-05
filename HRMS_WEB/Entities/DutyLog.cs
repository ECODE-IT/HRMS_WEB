using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class DutyLog
    {
        public int ID { get; set; }
        public String UserId { get; set; }
        public bool IsDutyOn { get; set; }
        public DateTime LogDateTime { get; set; }
        public int PowerOffMinutes { get; set; }
        public int idletime { get; set; }
        public int autocadtime { get; set; }
        public int exceltime { get; set; }
        public int wordtime { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsWeb { get; set; }

        public ApplicationUser User { get; set; }
    }
}
