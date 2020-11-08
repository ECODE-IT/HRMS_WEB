using HRMS_WEB.Entities;
using HRMS_WEB.Models;
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
    }
}
