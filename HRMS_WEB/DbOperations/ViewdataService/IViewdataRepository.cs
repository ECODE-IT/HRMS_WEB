using HRMS_WEB.Entities;
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
    }
}
