using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;

namespace NameDraw
{
	class MainClass
	{
		private const string emailFormat = @"
Dear {0},

Welcome to the Annual 2010 Joldersma Family Name Draw!  
This year brought to you with truly random atmooshperic noise based selection, courtesy of RANDOM.ORG.

To see who you drew, please click on this link: http://184.106.168.91/{1}.html.

Merry Christmas!

Santa's Grown Up Helper

-------------------------------------------------------------------------------------------------------
";
		
		private const string htmlFormat = @"
<html>
<head>
<style>
	p.finePrint {{font-size:7px}}
	strong {{font-size:1.2em; text-decoration:blink}}
</style>
</body>
<body>
<h1>Welcome to the Atmospherically-Randomized Joldersma Family Name Draw!</h1>

<p>The name you drew for 2010 is: <strong>{0}</strong>! Congratulations! We know you'll have a 
wonderful time thinking of and finding the perfect gift this person has ALWAYS wanted! Don't forget to read the fine print!</p>

<p class=finePrint>Standard Joldersma Family Name Draw rules apply: maximum purchase price is $20.  Both name drawer and drawee are permitted to broadcast 
any hints about preferences or preferance for percentage of gift amount to be donated to a cause of their choosing to all email addresses of all participants.
Liability waivers may be downloaded 
<a href=""http://www.northpole.com/stories/storyframes.asp?title=Sally*s$Secret&story=SallySecret&page=1&type=tlltllt&last=7&cp=n&loc=Workshop&flash=no&craft="">here</a>
</p>
</html>";
		
		public static void Main (string[] args)
		{
			List<string> drawers = new List<string> () {
				"Tom", 
				"Samara", 
				"Paul", 
				"Diana", 
				"Jonah", 
				"Damien", 
				"Rie", 
				"Ben", 
				"Nicole",
				"Jackson",
				"Kate"
			};
			
			List<string> drawees = new List<string> (drawers);
			
			WebClient wc = new WebClient ();
			StreamReader sr = new StreamReader (wc.OpenRead ("http://www.random.org/integers/?num=100&min=1&max=100&col=1&base=10&format=plain&rnd=new"));
			var readInt = new Func<int> (delegate {
				return int.Parse (sr.ReadLine ());
			});

			foreach (string drawer in drawers)
			{
				Guid g = Guid.NewGuid ();			
				string drawee = null;
				do 
				{
					drawee = drawees[readInt()%drawees.Count];
				}
				while (drawee == drawer);
				drawees.Remove (drawee);
				
				System.Console.WriteLine (emailFormat, drawer, g);
				
				TextWriter tw = new StreamWriter (g.ToString() + ".html");
				tw.Write (htmlFormat, drawee);
				tw.Close ();
 			}
		}
	}
}
