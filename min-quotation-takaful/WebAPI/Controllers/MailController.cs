using System;
using System.Threading.Tasks;
using App.Core.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("SendMail")]
        [Authorize]
        public async Task<string>  SendMail()
        {
            try
            {
                //var data = await _unitOfWork.MailSender.GetEmail();
                //foreach (var email in data)
                //{
                //    var parentjobId = BackgroundJob.Enqueue(() => _unitOfWork.MailSender.SendMail(email.Id).GetAwaiter().GetResult());
                //}
                await _unitOfWork.MailSender.ProcessEmailApproval();
            }
            catch (Exception ex)
            {

                return $"jobs mail failed : {ex.Message}";
            }           
            
            return $"jobs mail success";
        }
    }
}
