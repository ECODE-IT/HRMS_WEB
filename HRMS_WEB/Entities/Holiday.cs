﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Holiday
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
