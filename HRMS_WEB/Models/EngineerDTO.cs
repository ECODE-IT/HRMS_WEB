using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class EngineerDTO
    {
        public EngineerDTO()
        {
            ProjectList = new List<ProjectDTO>();
            SpecialTasks = new List<SpecialTask>();
        }
        public String ID { get; set; }
        public String Name { get; set; }
        public int TotalProjectCount { get; set; }
        public List<ProjectDTO> ProjectList { get; set; }
        public List<SpecialTask> SpecialTasks { get; set; }

    }
}
