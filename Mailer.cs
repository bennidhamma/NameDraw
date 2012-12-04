using System;
using System.Net.Mail;
using System.Net;

namespace NameDraw
{
	public class Mailer
	{
		SmtpClient smtp;
		MailAddress from;

		public Mailer (string fromEmail)
		{
			from = new MailAddress (fromEmail);
			var credentials = new NetworkCredential (from.Address, Environment.GetEnvironmentVariable("PWD"));
			smtp = new SmtpClient {
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = credentials
			};
		}

		public void Send (string toEmail, string toName, string subject, string message)
		{
			var toAddress = new MailAddress (toEmail, toName);
			var msg = new MailMessage (from, toAddress) {
				Subject = subject,
				Body = message
			};
			smtp.Send (msg);
		}
	}
}

