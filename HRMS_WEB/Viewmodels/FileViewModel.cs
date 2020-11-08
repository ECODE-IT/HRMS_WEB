using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class FileViewModel
    {
        public FileViewModel()
        {
            usersList = new List<UsersDTO>();
            AppLogList = new List<string>();
        }

        public List<String> AppLogList { get; set; }
        public IEnumerable<UsersDTO> usersList { get; set; }
        public String selectedUser { get; set; }
        public DateTime selectedDate { get; set; } = DateTime.Now;
    }
}