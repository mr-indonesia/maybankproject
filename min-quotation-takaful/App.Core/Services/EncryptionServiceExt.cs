using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;

namespace App.Core.Services
{
    public static class EncryptionServiceExt
    {
        public static IEncryptionService CreateEncryptionService(string key) => new EncryptionService(key);
        //public static IEncryptionService CreateEncryptionService(string key)
        //{
        //    return new EncryptionService(key);
        //}
    }
}
