using HRMS_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbOperations.EmailRepository
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
