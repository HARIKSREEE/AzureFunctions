using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace PluralsightFunc
{
    public static class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        //public static void Run([BlobTrigger("licenses/{name}", Connection = "AzureWebJobsStorage")]string licenseFileContents,[SendGrid(ApiKey ="SendGridApiKey")] out SendGridMessage message ,string name, ILogger log)
        public static void Run([BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContents, [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender, string orderId,
            [Table("orders","orders","{orderId}")] Order order, ILogger log)
        {

            //var email = Regex.Match(licenseFileContents, @"^Email\:\ (.+)$", RegexOptions.Multiline).Groups[1].Value;
            //var email = Regex.Match(licenseFileContents, @"Email\:(.+)m", RegexOptions.Multiline).Groups[0].Value
                //.Split(':')[1];
            //var email = "sivan.hari.sr@gmail.com";
            var email = order.Email;
            log.LogInformation($"Got order from {email}\n Order Id: {orderId}");
            var message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(email);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment(orderId, base64, "text/plain");
            message.Subject = "Your License File";
            message.HtmlContent = "Thank you for your order";
            if (!email.EndsWith("@test.com"))
            {
                sender.Add(message);
            }
        }
    }
}
