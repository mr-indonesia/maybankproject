using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;
using App.Core.Models.Email;
using DataAccess.EFCore;
using DataAccess.EFCore.Repositories;
using Hangfire.Storage;
using Hangfire;
using SharedKernel.Interfaces;

namespace App.Core.Services
{
    public class SchedulerService : EFRepository, ISchedulerService
    {
        private readonly IRepository repository;
        private readonly List<IJobService> _jobs;
        private readonly IMailSender mailSender;
        TimeZoneInfo tZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public SchedulerService(ApplicationContext context, IRepository _repository, IMailSender _emailSender) : base(context)
        {
            this.repository = _repository;
            _jobs = new List<IJobService>();
            mailSender = _emailSender;
        }

        public void Run()
        {
            try
            {
                var recurringJobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs();

                foreach (var item in recurringJobs)
                {
                    RecurringJob.RemoveIfExists(item.Id);
                }

                // Jadwalkan eksekusi fungsi ScheduleEmailJobs setiap 5 menit
                RecurringJob.AddOrUpdate(
                    "email-job",
                    () => ScheduleEmailJobs(),
                    "*/35 * * * *",
                    tZone,
                    "jobsmailapproval"
                );

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ScheduleEmailJobs()
        {
            var emailList = GetEmailList();

            foreach (var email in emailList)
            {
                var job = new SendEmailJobService(mailSender, email.Id);
                job.Execute();
            }
        }
        private List<EmailModel> GetEmailList()
        {
            var data = mailSender.GetEmail().GetAwaiter().GetResult();
            return data;
        }
    }
}
