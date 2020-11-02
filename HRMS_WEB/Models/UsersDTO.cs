using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class UsersDTO
    {
        public String ID { get; set; }
        public String Username { get; set; }
        public String PhoneNumber { get; set; }
        public int LeftProjectCount { get; set; }
        public double ManHoursSum { get; set; }
    }
}
