using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Eventify.Service.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly IConfiguration _config;

        public SmsService(ILogger<SmsService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task SendSmsAsync(string toNumber, string message)
        {
            TwilioClient.Init(_config["Notification:TwilioAccountSid"], _config["Notification:TwilioAuthToken"]);
            var msg = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_config["Notification:TwilioFromNumber"]),
                to: new Twilio.Types.PhoneNumber(toNumber)
            );

            _logger.LogInformation($"SMS sent to {toNumber} with SID {msg.Sid}");
        }
    }
}
