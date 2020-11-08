using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class ActivityChartViewModel
    {
        public ActivityChartViewModel()
        {
            usersList = new List<UsersDTO>();
        }
        public IEnumerable<UsersDTO> usersList { get; set; }
        public String selectedUser { get; set; }
        public DateTime selectedDate { get; set; } = DateTime.Now;
        public double workedHours { get; set; } = 0;
        public double idleHours { get; set; } = 0;
        public double autocadHours { get; set; } = 0;
    }
}
