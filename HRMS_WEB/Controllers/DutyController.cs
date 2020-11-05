using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class DutyController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IViewdataRepository viewdataRepository;
        private static IEnumerable<UsersDTO> usersList;

        public DutyController(IUserRepository userRepository, IViewdataRepository viewdataRepository)
        {
            this.userRepository = userRepository;
            this.viewdataRepository = viewdataRepository;
        }

        public IActionResult Index()
        {
            usersList = userRepository.getBasicUserListContainsId();
            var model = new DutyLogViewModel { usersList = usersList };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DutyLogViewModel model)
        {
            model.usersList = usersList;


            if (model.selectedUser != null)
            {
                model.dutyLogList = await viewdataRepository.getDutyLogsForTheUser(model.selectedUser, model.selectedDate);
                if (model.dutyLogList != null)
                {
                    var dutyonLogList = model.dutyLogList.Where(l => l.IsDutyOn == true).ToArray();
                    var dutyoffLogList = model.dutyLogList.Where(l => l.IsDutyOn == false).ToArray();

                    var dutyonhoursum = dutyonLogList.Sum(l => l.LogDateTime.TimeOfDay.TotalHours);
                    var dutyoffhoursum = dutyoffLogList.Sum(l => l.LogDateTime.TimeOfDay.TotalHours);

                    if (dutyonLogList.Length == dutyoffLogList.Length)
                    {
                        model.workedHours = RoundUp(dutyoffhoursum - dutyonhoursum, 2);
                    } 
                    else
                    {
                        model.workedHours = RoundUp(dutyoffhoursum - dutyonhoursum + DateTime.Now.TimeOfDay.TotalHours, 2); 
                    }
                }

            }

            return View(model);
        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
