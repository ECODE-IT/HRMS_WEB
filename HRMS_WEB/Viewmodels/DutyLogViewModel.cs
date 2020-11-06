using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class DutyLogViewModel
    {
        public DutyLogViewModel()
        {
            dutyLogList = new List<DutyLog>();
            usersList = new List<UsersDTO>();
        }
        public List<DutyLog> dutyLogList { get; set; }
        public IEnumerable<UsersDTO> usersList { get; set; }
        public String selectedUser { get; set; }
        public DateTime selectedDate { get; set; } = DateTime.Now;
        public double workedHours { get; set; }
        public double poweroffTime { get; set; }
    }
}
