using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab.Models
{
    public class AppUser : IdentityUser
    {
        public string Address { get; set; }
    }
}
