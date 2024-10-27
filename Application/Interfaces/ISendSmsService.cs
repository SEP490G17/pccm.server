using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;

namespace Application.Interfaces
{
    public interface ISendSmsService
    {
        public Task<Result<string>> SendSms(string to, string text,CancellationToken cancellationToken);
    }
}