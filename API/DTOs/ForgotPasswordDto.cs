using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.DTOs
{
    public class ForgotPasswordDTO 
    {
         public string Email { get; set; }
    }
}