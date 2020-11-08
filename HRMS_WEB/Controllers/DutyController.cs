using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class DutyController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IViewdataRepository viewdataRepository;
        private readonly IWindowsServiceRepository windowsServiceRepository;
        private static IEnumerable<UsersDTO> usersList;

        public DutyController(IUserRepository userRepository, IViewdataRepository viewdataRepository, IWindowsServiceRepository windowsServiceRepository)
        {
            this.userRepository = userRepository;
            this.viewdataRepository = viewdataRepository;
            this.windowsServiceRepository = windowsServiceRepository;
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
                    model.workedHours = RoundUp(await windowsServiceRepository.getworkedHours(model.selectedUser), 3);

                    model.poweroffTime = RoundUp(model.dutyLogList.Sum(dl => dl.PowerOffMinutes), 3);

                }

            }

            return View(model);
        }

        public IActionResult ActiveMembers()
        {
            var model = new ActiveMembersViewModel { DutyOnOffList = viewdataRepository.GetUserRegistariesForDate(DateTime.Now).Result, date = DateTime.Now};

            return View(model);
        }

        public IActionResult ActivityChart()
        {
            usersList = userRepository.getBasicUserListContainsId();
            var model = new ActivityChartViewModel { usersList = usersList, selectedDate = DateTime.Now};

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ActivityChart(ActivityChartViewModel model)
        {
            var workedhours = await windowsServiceRepository.getworkedHours(model.selectedUser);
            var idlehours = await windowsServiceRepository.getidleHours(model.selectedUser, model.selectedDate);
            var autocadHours = await windowsServiceRepository.getAutocadHours(model.selectedUser, model.selectedDate);

            model.usersList = usersList;

            model.workedHours = RoundUp(workedhours, 3);
            model.idleHours = RoundUp(idlehours, 3);
            model.autocadHours = RoundUp(autocadHours, 3);

            return View(model);
        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
