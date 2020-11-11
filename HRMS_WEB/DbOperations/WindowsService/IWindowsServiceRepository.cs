using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.WindowsService
{
    public interface IWindowsServiceRepository
    {
        Task<int> validateUserByUsernamePassword(ApplicationUser user, String username, String password);
        Task<double> createDutyOnOff(String username, bool isDutyOn, String datetime, int powereOffTime, int idletime, int autocadtime);
        Task<double> getworkedHours(String userId, DateTime date);
        Task<double> getidleHours(String userId, DateTime date);
        Task<double> getAutocadHours(String userId, DateTime date);
    }
}
