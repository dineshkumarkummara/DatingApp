using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.DTOs
{
    public class RegisterDto
    {
        public required string UserName {get;set;}
        public required string password{get;set;}
    }
}