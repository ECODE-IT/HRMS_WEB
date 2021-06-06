using HRMS_WEB.DbContext;
using HRMS_WEB.DbOperations.WindowsService;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ViewdataService
{
    public class ViewdataRepository : IViewdataRepository
    {
        private readonly HRMSDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWindowsServiceRepository windowsServiceRepository;

        public ViewdataRepository(HRMSDbContext db, UserManager<ApplicationUser> userManager, IWindowsServiceRepository windowsServiceRepository)
        {
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
                .Select(dlg => new LeaveViewModel { 
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
                if(DateTime.Equals(selectedDate.Date, DateTime.Now.Date))
                {
                    return dutyoffsum - dutyonsum + DateTime.Now.TimeOfDay.TotalHours - ((poweroffsum) / 60.0);
                }
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
            var result =  await db.DutyLogs.Where(dl => DateTime.Equals(dl.LogDateTime.Date, date.Date)).ToListAsync();
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
    }
}
