using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface  IEmailService
    {
      Task SendMail(MailContent mailContent);
    
      Task SendEmailAsync(string email, string subject, string htmlMessage);
    
    }
}