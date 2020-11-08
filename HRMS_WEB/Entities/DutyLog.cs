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
    }
}
