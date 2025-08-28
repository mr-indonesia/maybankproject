using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using App.Core.Interfaces;
using App.Core.Models;
using DataAccess.EFCore.Repositories;
using DataAccess.EFCore;
using SharedKernel.Interfaces;
using App.Core.Models.Email;
using System.Threading.Tasks;
using App.Core.Models.Authentications;
using App.Core.Models.Users;
using System.Linq;
using App.Core.UnitOfWorks;
using Hangfire;

namespace App.Core.Services
{
    public class MailSenderService : EFRepository, IMailSender
    {
        private readonly SmtpConfiguration _smtpConfig;
        public MailSenderService() { }

        private readonly IRepository repository;

        public MailSenderService(ApplicationContext context, IRepository _repository) : base(context)
        {
            this.repository = _repository;
        }

        public MailSenderService(ApplicationContext context, IRepository _repository, SmtpConfiguration smtpConfig) : base(context)
        {
            this.repository = _repository;
            _smtpConfig = smtpConfig;
        }

        public void SendMail(EmailMessage message)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpConfig.Username, _smtpConfig.Password);
                    smtpClient.EnableSsl = _smtpConfig.EnableSsl;

                    var mailMessage = new MailMessage(message.From, message.To, message.Subject, message.Body);

                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        public async Task SendMail(long mailid)
        {
            try
            {
                string query = "PROCESS_APPROVAL_NOTIFICATION";
                var param = new Dictionary<string, object> {
                    {"@EmailId", mailid }
                };
                await repository.ExecuteQueryAsync(query, param, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}"); ;
            }
        }

        public async Task<List<EmailModel>> GetEmail()
        {
            try
            {
                string query = @"SELECT
		                        M.Id,
		                        M.EmailTo,
		                        M.EmailCc,
		                        M.EmailBcc,
		                        M.EmailSubject,
		                        M.EmailBody
		                        FROM RSendEmail M WITH (NOLOCK) 
		                        WHERE
		                        M.IsProcess = 0 AND M.IsDeleted = 0
		                        ORDER BY CreatedAt DESC	";

                var data = await repository.QueryAsync<EmailModel>(query, null, false);

                return data;
            }
            catch (Exception)
            {

                return new List<EmailModel>();
            }
        }

        public async Task ProcessEmailApproval()
        {
            var data = await GetEmail();
            foreach (var email in data)
            {
                //await SendMail(email.Id);
                var jobsid = BackgroundJob.Enqueue(() => SendMail(email.Id));
            }
        }
    }
}
