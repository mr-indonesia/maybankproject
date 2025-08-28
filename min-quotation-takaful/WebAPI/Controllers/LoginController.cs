using System;
using System.Threading.Tasks;
using App.Core.Interfaces;
using App.Core.Models.Login;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public LoginController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /*[HttpGet]
        [Route("login")]
        public string Login()
        {
            //Fire - and - Forget Job - this job is executed only once
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Welcome to Shopping World!"));

            return $"Job ID: {jobId}. Welcome mail sent to the user!";
        }*/

        [Route("GenerateTokenBySessionId")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var data = await _unitOfWork.IdentityService.Login(loginModel.SessionId);
            return Ok(data);
        }
    }
}
