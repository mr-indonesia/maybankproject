﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Models.Users
{
    public class User
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
    }
}
