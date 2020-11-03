using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class GalleryViewmodel
    {
        public GalleryViewmodel()
        {
            imagePathList = new List<string>();
            usersList = new List<UsersDTO>();
        }
        public List<String> imagePathList { get; set; }
        public IEnumerable<UsersDTO> usersList { get; set; }
        public String selectedUser { get; set; }
        public DateTime selectedDate { get; set; } = DateTime.Now;
    }
}
