using Application.Core;
using Application.Interfaces;
using Infobip.Api.SDK;
using Infobip.Api.SDK.SMS.Models;
using Microsoft.Extensions.Options;

namespace Infrastructure.SendMessage
{
    public class SendSmsService(IOptions<InfobipAPI> config) : ISendSmsService
    {
        public async Task<Result<string>> SendSms(string to, string text, CancellationToken cancellationToken)
        {
            var infobipSecret = config.Value;

            var configuration = new ApiClientConfiguration(infobipSecret.APIBaseURL, infobipSecret.APIKey);
            var client = new InfobipApiClient(configuration);
            var destination = new SmsDestination(null, to);
            var message = new SmsMessage()
            {
                Text = text,
                From = infobipSecret.PhoneNumber,
                Destinations = [destination],
            };
            var request = new SendSmsMessageRequest()
            {
                Messages = [message]
            };

            var result = await client.Sms.SendSmsMessage(request, cancellationToken);
            return Result<string>.Success(result.Messages.ToString());
        }
    }
}