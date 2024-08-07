﻿using Microsoft.Extensions.Configuration;
using OrderSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string message)
        { 
            var fromEmail = _configuration["Mail:FromEmail"];
            var fromPassword = _configuration["Mail:FromPassword"];
            var smtpServer = _configuration["Mail:SmtpServer"];
            var port = Convert.ToInt32(_configuration["Mail:Port"]);

            SmtpClient client = new SmtpClient(smtpServer, port);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(fromEmail, fromPassword);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendFormat($"<h1>{message}</h1>");
            mailBody.AppendFormat("<br />");
            mailBody.AppendFormat("<p>Thank you</p>");
            mailMessage.Body = mailBody.ToString();

            client.Send(mailMessage);
        }
    }
}
