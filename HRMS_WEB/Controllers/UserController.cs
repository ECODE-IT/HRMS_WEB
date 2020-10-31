using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbOperations.UserRepository;
using HRMS_WEB.DbOperations.ViewdataService;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IViewdataRepository viewdataRepository;
        private readonly IUserRepository userRepository;

        public UserController(IViewdataRepository viewdataRepository, IUserRepository userRepository)
        {
            this.viewdataRepository = viewdataRepository;
            this.userRepository = userRepository;
        }
        public async Task<IActionResult> Index()
        {
            var userlist = await viewdataRepository.getUserList();
            return View(userlist);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
                await userRepository.insertUser(user);
                return RedirectToAction("Index");
        }
    }
}
