using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.ViewdataService
{
    public interface IViewdataRepository
    {
        Task<IEnumerable<User>> getUserList();
        Task<int> GetSubLevelCount();
        Task<IEnumerable<SubLevel>> GetSubLevelsForPage(int indexfrom, int indexto);
        Task<List<DutyLog>> getDutyLogsForTheUser(String id, DateTime selectedDate);
        Task<List<IGrouping<String, DutyLog>>> GetUserRegistariesForDate(DateTime date);
        IEnumerable<Leave> getleavesforthisMonth(DateTime currentdatetime);
        Task<IEnumerable<LeaveViewModel>> getMonthDraughtmenReport(DateTime selectedDate);
        Task insertTempData(double temp, double humidity, double temp2, double humidity2);
        Task<Object> GetHumidities();
        Task<Humidity> getlastHumidityData();
        Task<UserMonthEndSummaryDTO> GetUserMonthEndSummary(String userid, int month);
        Task<MonthEndEmployeeDTO> GetMonthEndEmployeeSummary(int month);
    }
}
