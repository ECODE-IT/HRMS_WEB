using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class SubLevelViewmodel
    {
        public SubLevelViewmodel()
        {
            SubLevelList = new List<SubLevel>();
            ProjectList = new List<ProjectDTO>();
        }

        public IEnumerable<SubLevel> SubLevelList { get; set; }
        public IEnumerable<ProjectDTO> ProjectList { get; set; }
        public int selectedProjectId { get; set; }
    }
}
