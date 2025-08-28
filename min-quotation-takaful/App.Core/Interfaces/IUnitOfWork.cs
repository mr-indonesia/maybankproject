using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public int Complete();
        public IMailSender MailSender { get; }
        public IIdentityService IdentityService { get; }
        IEncryptionService EncryptionService { get; }
    }
}
