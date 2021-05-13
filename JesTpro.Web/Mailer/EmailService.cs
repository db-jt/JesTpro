// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using jt.jestpro.Services;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jt.jestpro.Mailer
{
	public interface IEmailService
	{
		Task Send(EmailMessage emailMessage);
	}

	public class EmailService : IEmailService
	{
		private readonly ISettingService _settingService;

		public EmailService(ISettingService settingService)
		{
			_settingService = settingService;
		}

		public async Task Send(EmailMessage emailMessage)
		{
			var settings = await _settingService.GetList(new Models.SettingFilterDto());
			var nameFrom = settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.NameFrom", StringComparison.InvariantCultureIgnoreCase)).Value;
			var mailFrom = settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.MailFrom", StringComparison.InvariantCultureIgnoreCase)).Value;
			var smtpServer = settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.SmtpServer", StringComparison.InvariantCultureIgnoreCase)).Value;
			var smtpPort = int.Parse(settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.SmtpPort", StringComparison.InvariantCultureIgnoreCase)).Value);
			var smtpEnableSSL = bool.Parse(settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.EnableSsl", StringComparison.InvariantCultureIgnoreCase)).Value);
			var smtpUsername = settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.SmtpUsername", StringComparison.InvariantCultureIgnoreCase)).Value;
			var smtpPassword = settings.FirstOrDefault(x => x.Key != null && x.Key.Equals("email.SmtpPassword", StringComparison.InvariantCultureIgnoreCase)).Value;

			var message = new MimeMessage();
			message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
			if (emailMessage.FromAddresses.Count > 0)
			{
				message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
			}
			else
            {
				message.From.Add(new MailboxAddress(nameFrom, mailFrom));
			}

			message.Subject = emailMessage.Subject;

			var builder = new BodyBuilder();

			#region Attachments
			foreach (var attach in emailMessage.Attachments)
			{
				// We may also want to attach a calendar event for Monica's party...
				builder.Attachments.Add(attach);
			}
			#endregion

			#region Embedded image attachments
			// In order to reference selfie.jpg from the html text, we'll need to add it
			// to builder.LinkedResources and then use its Content-Id value in the img src.
			// to be tested (http://www.mimekit.net/docs/html/P_MimeKit_BodyBuilder_Attachments.htm)
			foreach (var imgPath in emailMessage.EmbeddedImagesAttachments)
			{
				var image = builder.LinkedResources.Add(imgPath);
				image.ContentId = MimeUtils.GenerateMessageId();
				emailMessage.Content.Replace(imgPath, $"cid:{image.ContentId}");
			}
			#endregion

			builder.HtmlBody = emailMessage.Content;

			// Now we just need to set the message body and we're done
			message.Body = builder.ToMessageBody();

			//We will say we are sending HTML. But there are options for plaintext etc. 
			//message.Body = new TextPart(TextFormat.Html)
			//{
			//	Text = emailMessage.Content
			//};

			//Be careful that the SmtpClient class is the one from Mailkit not the framework!
			using (var emailClient = new SmtpClient())
			{
                //The last parameter here is to use SSL(Which you should!)
                if (smtpPort > -1)
                {
                    emailClient.Connect(smtpServer, smtpPort, smtpEnableSSL);
                }
                else
                {
                    emailClient.Connect(smtpServer, 25, MailKit.Security.SecureSocketOptions.None);
                }
                
                //Remove any OAuth functionality as we won't be using it. 
                //emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                if (!string.IsNullOrWhiteSpace(smtpUsername) && !string.IsNullOrWhiteSpace(smtpPassword))
                {
                    emailClient.Authenticate(smtpUsername, smtpPassword);
                }
				emailClient.Send(message);

				emailClient.Disconnect(true);
			}
		}
	}
}
