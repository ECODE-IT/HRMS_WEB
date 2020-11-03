using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.UserRepository
{
    public interface IUserRepository
    {
        Task<int> insertUser(User user);
        Task<IEnumerable<SubLevel>> getSubLevelListForTheUser(String username);
        IEnumerable<UsersDTO> getUsersWithData();
        IEnumerable<UsersDTO> getBasicUserList();
    }
}
