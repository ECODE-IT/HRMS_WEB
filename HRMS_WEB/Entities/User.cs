using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class User
    {
        public int ID { get; set; }
        public int UserType { get; set; }
        public String UserName { get; set; }
        public String UserPassword { get; set; }
        public String ContactNumber { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
    }
}
