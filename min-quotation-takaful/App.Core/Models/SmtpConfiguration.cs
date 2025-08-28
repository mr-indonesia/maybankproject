using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models
{
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }

        public SmtpConfiguration(string host, int port, string username, string password, bool enableSsl)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            EnableSsl = enableSsl;
        }
    }
}
