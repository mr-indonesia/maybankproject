using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.Authentications
{
    public class ServiceConfiguration
    {
        public JwtSettings JwtSettings { get; set; }
    }

    public class JwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
        public string Issuer { get; set; }
    }
}
