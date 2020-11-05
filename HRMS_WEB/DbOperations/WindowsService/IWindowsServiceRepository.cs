using HRMS_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.WindowsService
{
    public interface IWindowsServiceRepository
    {
        Task<int> validateUserByUsernamePassword(String username, String password);
        Task<double> createDutyOnOff(String username, bool isDutyOn, String datetime);
    }
}
