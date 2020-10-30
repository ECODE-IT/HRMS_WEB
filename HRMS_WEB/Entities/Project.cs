using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Project
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public DateTime AssignedDateTime { get; set; }
        public String Customer { get; set; }
        public DateTime Deadline { get; set; }
        public String Remarks { get; set; }

        // naviagation properties
        public ICollection<SubLevel> SubLevels { get; set; }
    }
}
