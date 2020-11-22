using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class Message
    {
        public Message(IEnumerable<String> to, String subject, String content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
            Content = content;
        }

        public List<MailboxAddress> To { get; set; }
        public String Subject { get; set; }
        public String Content { get; set; }

    }
}
