using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;
using Hangfire;

namespace App.Core.Services
{
    public class SendEmailJobService : IJobService
    {
        private readonly IMailSender emailSender;
        private readonly long emailId;

        public SendEmailJobService(IMailSender _emailSender, long _emailId)
        {
            this.emailSender = _emailSender;
            this.emailId = _emailId;
        }

        public void Execute()
        {
            BackgroundJob.Enqueue(() => emailSender.SendMail(emailId));
        }
    }
}
