using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.DTOs
{
    public class ConfirmForgotPasswordDto 
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

    }
}