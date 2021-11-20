using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Models.Dtos
{
    public class NewPassword
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}
