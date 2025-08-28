using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models
{
    public class EmailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailMessage(string from, string to, string cc, string bcc, string subject, string body)
        {
            From = from;
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }
    }
}
