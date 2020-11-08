using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class ActiveMembersViewModel
    {
        public ActiveMembersViewModel()
        {
            DutyOnOffList = new List<IGrouping<string, DutyLog>>();
        }

        public DateTime date { get; set; }
        public List<IGrouping<String, DutyLog>> DutyOnOffList { get; set; }
    }
}
