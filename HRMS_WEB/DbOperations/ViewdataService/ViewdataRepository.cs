using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ViewdataService
{
    public class ViewdataRepository : IViewdataRepository
    {
        private readonly ILogger<ViewdataRepository> logger;
        private readonly HRMSDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWindowsServiceRepository windowsServiceRepository;
        double IdleAlocationdaily = 0.5;

        public ViewdataRepository(ILogger<ViewdataRepository> logger, HRMSDbContext db, UserManager<ApplicationUser> userManager, IWindowsServiceRepository windowsServiceRepository)
        {
            this.logger = logger;
            this.db = db;
            this.userManager = userManager;
            this.windowsServiceRepository = windowsServiceRepository;
        }

        public async Task<List<DutyLog>> getDutyLogsForTheUser(string id, DateTime selectedDate)
        {
            return await db.DutyLogs.AsNoTracking().Where(dl => DateTime.Equals(dl.LogDateTime.Date, selectedDate.Date) && dl.UserId.Equals(id)).ToListAsync();
        }

        public IEnumerable<Leave> getleavesforthisMonth(DateTime currentdatetime)
        {
            return db.Leaves.Where(l => l.Date.Month == currentdatetime.Month || l.Date.Month == currentdatetime.Month - 1 || l.Date.Month == currentdatetime.Month + 1).Include(l => l.User);
        }

        public async Task<IEnumerable<LeaveViewModel>> getMonthDraughtmenReport(DateTime selectedDate)
        {
            var dutylogs = await db.DutyLogs.Where(dl => Convert.ToInt32(dl.LogDateTime.Month) == Convert.ToInt32(selectedDate.Month) && Convert.ToInt32(dl.LogDateTime.Year) == Convert.ToInt32(selectedDate.Year)).Include(dl => dl.User).ToListAsync();
            var sysconfig = await db.SystemSettings.FirstOrDefaultAsync();

            return dutylogs
                .GroupBy(dl => dl.User.Name)
                .Select(dlg => new LeaveViewModel
                {
                    Username = dlg.Key,
                    TodayWorkedHoursProgress = getTodayWorkingHours(dutylogs, dlg.FirstOrDefault().User.Id, selectedDate) * 100 / sysconfig.DailyTargetHours,
                    MonthWorkedHoursProgress = getMonthWorkingHours(dutylogs, dlg.FirstOrDefault().User.Id) * 100 / sysconfig.MonthlyTargetHours,
                    DailyIdleHours = getDailyIdleHours(dutylogs, dlg.FirstOrDefault().User.Id, selectedDate),
                    MonthIdleHours = getMonthlyIdleHours(dutylogs, dlg.FirstOrDefault().User.Id)
                }).OrderByDescending(s => (s.MonthWorkedHoursProgress - s.MonthIdleHours));
        }

        // get monthly working hours
        public double getMonthWorkingHours(List<DutyLog> dutylogs, String userid)
        {

            var dutyonlogs = dutylogs.Where(dl => dl.IsDutyOn == true && dl.User.Id.Equals(userid)).ToArray();
            var dutyofflogs = dutylogs.Where(dl => dl.IsDutyOn == false && dl.User.Id.Equals(userid)).ToArray();

            var dutyonsum = dutyonlogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);
            var dutyoffsum = dutyofflogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);

            var poweroffsum = dutylogs.Where(dl => dl.User.Id.Equals(userid)).Sum(dl => dl.PowerOffMinutes);

            if (dutyonlogs.Length == dutyofflogs.Length)
            {
                return dutyoffsum - dutyonsum - ((poweroffsum) / 60.0);
            }
            else
            {
                return dutyoffsum - dutyonsum + DateTime.Now.TimeOfDay.TotalHours - ((poweroffsum) / 60.0);
            }

        }

        //get weekly working hours
        public double getTodayWorkingHours(List<DutyLog> dutylogs, String userid, DateTime selectedDate)
        {

            var dutyonlogs = dutylogs.Where(dl => dl.IsDutyOn == true && DateTime.Equals(selectedDate.Date, dl.LogDateTime.Date) && dl.User.Id.Equals(userid)).ToArray();
            var dutyofflogs = dutylogs.Where(dl => dl.IsDutyOn == false && DateTime.Equals(selectedDate.Date, dl.LogDateTime.Date) && dl.User.Id.Equals(userid)).ToArray();

            var dutyonsum = dutyonlogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);
            var dutyoffsum = dutyofflogs.Sum(dl => dl.LogDateTime.TimeOfDay.TotalHours);

            var poweroffsum = dutylogs.Where(dl => DateTime.Equals(selectedDate.Date, dl.LogDateTime.Date) && dl.User.Id.Equals(userid)).Sum(dl => dl.PowerOffMinutes);

            if (dutyonlogs.Length == dutyofflogs.Length)
            {
                return dutyoffsum - dutyonsum - ((poweroffsum) / 60.0);
            }
            else
            {
                if (DateTime.Equals(selectedDate.Date, DateTime.Now.Date))
                {
                    return dutyoffsum - dutyonsum + DateTime.Now.TimeOfDay.TotalHours - ((poweroffsum) / 60.0);
                }
                logger.LogError("Selected Date has odd count of duty log for the user : " + userid);
                throw new Exception("Selected Date has odd count of duty log for the user : " + userid);
            }

        }

        public double getMonthlyIdleHours(List<DutyLog> dutylogs, String userid)
        {
            return dutylogs.Where(dl => dl.UserId.Equals(userid)).Sum(dl => dl.idletime) / 60.0;
        }

        public double getDailyIdleHours(List<DutyLog> dutylogs, String userid, DateTime selectedDate)
        {
            return dutylogs.Where(dl => dl.UserId.Equals(userid) && DateTime.Equals(dl.LogDateTime.Date, selectedDate.Date)).Sum(dl => dl.idletime) / 60.0;
        }

        public async Task<int> GetSubLevelCount()
        {
            int count = await db.SubLevels.CountAsync();
            return count;
        }

        public async Task<IEnumerable<SubLevel>> GetSubLevelsForPage(int indexfrom, int indexto)
        {
            var sublevels = await db.SubLevels.ToListAsync();
            return sublevels.Where(sl => sl.ID >= indexfrom && sl.ID <= indexto);
        }

        public async Task<IEnumerable<User>> getUserList()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<List<IGrouping<String, DutyLog>>> GetUserRegistariesForDate(DateTime date)
        {
            var result = await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, date.Date)).ToListAsync();
            return result.OrderBy(dl => dl.LogDateTime).GroupBy(dl => dl.UserId).ToList();
        }

        public async Task insertTempData(double temp, double humidity, double temp2, double humidity2)
        {
            await db.HumidityDatas.AddAsync(new Humidity { Time = DateTime.Now, RH = humidity, Temp = temp, Temp2 = temp2, RH2 = humidity2 });
            await db.SaveChangesAsync();
        }

        public async Task<Object> GetHumidities()
        {
            var result = await db.HumidityDatas.OrderByDescending(hd => hd.ID).Take(100).ToListAsync();
            return new { lane1 = result.Select(r => r.RH), lane2 = result.Select(r => r.RH2), labels = result.Select(r => r.Time.ToString("HH:mm:ss")) };
        }

        public async Task<Humidity> getlastHumidityData()
        {
            return await db.HumidityDatas.OrderByDescending(hd => hd.ID).FirstOrDefaultAsync();
        }

        public async Task<UserMonthEndSummaryDTO> GetUserMonthEndSummary(String userid, int month)
        {
            try
            {
                var username = await userManager.FindByIdAsync(userid);
                var salary = await db.Salary.FirstOrDefaultAsync(s => s.Username.Equals(username.UserName));
                var othourrate = salary.OTHourRate;
                var sysconfig = await db.SystemSettings.FirstOrDefaultAsync();
                var nonotalocation = sysconfig.DailyTargetHours;
                string monthName = new DateTime(2010, month, 1)
                    .ToString("MMM", CultureInfo.InvariantCulture);
                var summaryDto = new UserMonthEndSummaryDTO() { Designation = "Draughtmen", Month = monthName, Name = username?.Name, Year = DateTime.Now.Year.ToString() };

                var dutyLogs = await db.DutyLogs
                    .Where(dl => dl.LogDateTime.Month == month && dl.LogDateTime.Year == DateTime.Now.Year && dl.UserId.Equals(userid))
                    .OrderBy(dl => dl.LogDateTime).ToListAsync();

                var groupedLogs = dutyLogs.GroupBy(dl => dl.LogDate);

                var holidaylist = await db.Holidays.ToListAsync();

                summaryDto.DaySummaries = new List<DaySummaryDTO>();

                var weekdayotsum = 0.0;
                var weekendotsum = 0.0;

                foreach (var group in groupedLogs)
                {
                    var firston = group.Where(dl => dl.IsDutyOn).FirstOrDefault();
                    var lastoff = group.Where(dl => !dl.IsDutyOn).LastOrDefault();

                    var fistitem = group.FirstOrDefault();
                    var dailyidlehours = getDailyIdleHours(group.ToList(), userid, fistitem.LogDateTime);
                    var dailyworkedhours = getTodayWorkingHours(group.ToList(), userid, fistitem.LogDateTime);

                    var isholiday = group.Key.DayOfWeek.Equals(DayOfWeek.Saturday) || group.Key.DayOfWeek.Equals(DayOfWeek.Sunday) || holidaylist.Any(h => h.Date.Equals(group.Key));
                    var ottime = 0d;
                    if (isholiday)
                    {
                        if (dailyidlehours > IdleAlocationdaily)
                        {
                            ottime = dailyworkedhours - (dailyidlehours - IdleAlocationdaily);
                        }
                        else
                        {
                            ottime = dailyworkedhours;
                        }
                        if (ottime < 0)
                        {
                            ottime = 0d;
                        }

                        weekendotsum += ottime;

                    }
                    else
                    {
                        if (dailyidlehours > IdleAlocationdaily)
                        {
                            ottime = dailyworkedhours - (dailyidlehours - IdleAlocationdaily) - nonotalocation;
                        }
                        else
                        {
                            ottime = dailyworkedhours - nonotalocation;
                        }

                        if (ottime < 0)
                        {
                            ottime = 0;
                        }
                        weekdayotsum += ottime;
                    }




                    var daysummerydto = new DaySummaryDTO()
                    {
                        DateNo = fistitem.LogDateTime.Day,
                        DayName = group.Key.ToString("dddd").Substring(0, 3),
                        FirstIn = firston.LogDateTime,
                        LastOut = lastoff.LogDateTime,
                        IdleHours = string.Format("{0:0.00} hrs", dailyidlehours),
                        WorkedHours = string.Format("{0:0.00} hrs", dailyworkedhours),
                        IsHoliday = isholiday,
                        OTTimeweekday = isholiday ? "0.00 hrs" : string.Format("{0:0.00} hrs", ottime),
                        OTTimeweekend = isholiday ? string.Format("{0:0.00}", ottime) + " hrs" : "0.00 hrs",
                    };

                    summaryDto.DaySummaries.Add(daysummerydto);
                }

                summaryDto.OTWeekdaySum = string.Format("{0:0.00} hrs", weekdayotsum);
                summaryDto.OTWeekendSum = string.Format("{0:0.00} hrs", weekendotsum);

                summaryDto.OTWeekdayEarning = string.Format("Rs. {0:n}", weekdayotsum * othourrate);
                summaryDto.OTWeekendEarning = string.Format("Rs. {0:n}", weekendotsum * othourrate);

                summaryDto.OTAllEarning = string.Format("Rs. {0:n}", (weekdayotsum * othourrate) + (weekendotsum * othourrate));

                return summaryDto;
            }
            catch (Exception ex)
            {
                return new UserMonthEndSummaryDTO();
            }

        }

        public async Task<MonthEndEmployeeDTO> GetMonthEndEmployeeSummary(int month)
        {
            int year = DateTime.Now.Year;
            string monthName = new DateTime(year, month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var dutylogs = await db.DutyLogs.Include(dl => dl.User).Where(dl => dl.LogDateTime.Month == month).ToListAsync();
            var dlgroup = dutylogs.GroupBy(dl => dl.UserId);
            var sysconfig = await db.SystemSettings.FirstOrDefaultAsync();
            var nonotalocation = sysconfig.DailyTargetHours;
            var holidaylist = await db.Holidays.ToListAsync();

            var employeeprofilelist = new List<MonthEndEmployeeProfile>();
            int i = 0;
            foreach (var group in dlgroup)
            {
                i++;
                var weekendotsum = 0d;
                var weekdayotsum = 0d;
                foreach (var dategroup in group.ToList().GroupBy(dl => dl.LogDate))
                {
                    var dailyidlehours = getDailyIdleHours(dategroup.ToList(), group.Key, dategroup.Key);
                    var dailyworkedhours = getTodayWorkingHours(dategroup.ToList(), group.Key, dategroup.Key);
                    var isholiday = dategroup.Key.DayOfWeek.Equals(DayOfWeek.Saturday) || dategroup.Key.DayOfWeek.Equals(DayOfWeek.Sunday) || holidaylist.Any(h => h.Date.Equals(group.Key));
                    var ottime = 0d;

                    if (isholiday)
                    {
                        if (dailyidlehours > 1)
                        {
                            ottime = dailyworkedhours - (dailyidlehours - IdleAlocationdaily);
                        }
                        else
                        {
                            ottime = dailyworkedhours;
                        }
                        if (ottime < 0)
                        {
                            ottime = 0d;
                        }

                        weekendotsum += ottime;

                    }
                    else
                    {
                        if (dailyidlehours > 1)
                        {
                            ottime = dailyworkedhours - (dailyidlehours - IdleAlocationdaily) - nonotalocation;
                        }
                        else
                        {
                            ottime = dailyworkedhours - nonotalocation;
                        }

                        if (ottime < 0)
                        {
                            ottime = 0;
                        }
                        weekdayotsum += ottime;
                    }
                }

                var monthworkedhours = getMonthWorkingHours(group.ToList(), group.Key);
                var monthidlehours = getMonthlyIdleHours(group.ToList(), group.Key);

                var empprofile = new MonthEndEmployeeProfile()
                {
                    Index = i,
                    WorkedHours = string.Format("{0:0.00} hrs", monthworkedhours),
                    IdleHours = string.Format("{0:0.00} hrs", monthidlehours),
                    Name = group.FirstOrDefault().User.Name,
                    WorkedDays = group.ToList().Select(dl => dl.LogDate).Distinct().Count(),
                    OtHours = string.Format("{0:0.00} hrs", weekdayotsum + weekendotsum),
                    WeekdayOTHours = string.Format("{0:0.00} hrs", weekdayotsum),
                    HolidayOTHours = string.Format("{0:0.00} hrs", weekendotsum)
                };

                employeeprofilelist.Add(calculatePerformanceIndex(empprofile,monthworkedhours, monthidlehours ));


            }

            return new MonthEndEmployeeDTO() { Month = monthName, monthEndEmployeeDTOs = employeeprofilelist };
            

        }

        private MonthEndEmployeeProfile calculatePerformanceIndex(MonthEndEmployeeProfile dto, double workedhours, double idlehours)
        {
            var actualworkedhours = workedhours - idlehours;
            double workeddays = actualworkedhours / dto.WorkedDays;
            dto.PerformaceIndex = string.Format("{0:0.00} Hrs/Day", workeddays);
            return dto;
        }
    }
}
