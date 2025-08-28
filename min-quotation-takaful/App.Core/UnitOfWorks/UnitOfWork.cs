using App.Core.Interfaces;
using App.Core.Models.Authentications;
using App.Core.Services;
using DataAccess.EFCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        public UnitOfWork(ApplicationContext context, IRepository _repository, IOptions<ServiceConfiguration> _serviceConfiguration, TokenValidationParameters _tokenValidationParameters, string sharedScreet = "")
        {
            _context = context;
            MailSender = new MailSenderService(context, _repository);
            IdentityService = new IdentityService(context, _repository, _serviceConfiguration, _tokenValidationParameters);
            EncryptionService = new EncryptionService(sharedScreet);
        }
        public IMailSender MailSender { get; private set; }

        public IIdentityService IdentityService { get; private set; }

        public IEncryptionService EncryptionService { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
