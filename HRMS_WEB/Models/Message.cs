using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class Message
    {
        public Message(IEnumerable<String> to, String subject, String content, IEnumerable<String> cc)
        {
            To = new List<MailboxAddress>();
            Cc = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x)));
            if (cc != null)
            {
                Cc.AddRange(cc.Select(x => new MailboxAddress(x)));
            }
            Subject = subject;
            Content = content;
        }

        public List<MailboxAddress> To { get; set; }
        public List<MailboxAddress> Cc { get; set; }
        public String Subject { get; set; }
        public String Content { get; set; }

    }
}
