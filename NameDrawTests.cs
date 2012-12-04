using System;
using NUnit.Framework;

namespace NameDraw
{
	[TestFixture()]
	public class NameDrawTests
	{
		[Test()]
		public void MailTest ()
		{
			Mailer mailer = new Mailer ("bennidhamma@gmail.com");

			mailer.Send ("ben@joldersma.org", "Ben Joldersma", "testing", "testing");
		}
	}
}

