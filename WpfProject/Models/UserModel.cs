﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public int UserId { get; set; }
    }
}
