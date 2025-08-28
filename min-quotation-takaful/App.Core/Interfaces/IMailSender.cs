using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using App.Core.Models;
using App.Core.Models.Email;
using SharedKernel.Interfaces;

namespace App.Core.Interfaces
{
    public interface IMailSender : IRepository
    {
        Task ProcessEmailApproval();
        Task SendMail(long mailid);
        void SendMail(EmailMessage message);
        Task<List<EmailModel>> GetEmail();
    }
}
