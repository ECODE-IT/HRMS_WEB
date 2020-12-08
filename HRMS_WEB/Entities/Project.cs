using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
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
        public String Code { get; set; }
        public DateTime AssignedDateTime { get; set; }
        public String UserId { get; set; }
        public String Customer { get; set; }
        public DateTime Deadline { get; set; }
        public String Remarks { get; set; }
        public double Progress { get; set; }
        public bool IsFinished { get; set; }
        public DateTime FinishedDate { get; set; }

        // naviagation properties
        public ICollection<SubLevel> SubLevels { get; set; }
        public ICollection<SpecialTask> SpecialTasks { get; set; }
        public ApplicationUser User { get; set; }
    }
}
