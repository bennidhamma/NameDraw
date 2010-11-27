using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;

namespace NameDraw
{
	class MainClass
	{
		//The email string to send to drawers
		private const string emailFormat = @"
Dear {0},

Welcome to the Annual 2010 Joldersma Family Name Draw!  
This year brought to you with truly random atmooshperic noise based selection, courtesy of RANDOM.ORG.

To see who you drew, please click on this link: http://184.106.168.91/{1}.html.

Merry Christmas!

Santa's Grown Up Helper

-------------------------------------------------------------------------------------------------------
";
		
		//The html template to populate with the drawee's name
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
			//The list of participants, first as a collection of drawers.
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
			
			//Copy the list to create a shrinking pool of drawees.
			//The algorithm here is to shrink the pool by one each time we successfully find a match.
			List<string> drawees = new List<string> (drawers);
			
			//Request a list of 100 truly random numbers from RANODM.ORG.
			WebClient wc = new WebClient ();
			StreamReader sr = new StreamReader (wc.OpenRead ("http://www.random.org/integers/?num=100&min=1&max=100&col=1&base=10&format=plain&rnd=new"));
			
			//Create a delegate (callback function) that reads one line from the web request, converts it to an integer, and returns that value.
			var readInt = new Func<int> (delegate {
				return int.Parse (sr.ReadLine ());
			});
			
			//loop through each drawer
			foreach (string drawer in drawers)
			{
				//Generate a unique guid (key) for this drawer, this will serve as the file name, to help obscure the URL and avoid any 
				//unwanted snooping!
				Guid g = Guid.NewGuid ();			
				
				string drawee = null;
				//In order to avoid drawing yourself, we keep picking a drawee until we have one that is not the drawer.  
				//In other words, keep drawing a drawee while that drawee is the same as the drawer (usually this will only happen once).
				do 
				{
					//select a name from the drawees list by reading an integer from the web stream, and modulating the number by the size of the 
					//drawees list.
					drawee = drawees [readInt () % drawees.Count];
				}
				while (drawee == drawer);
				//now remove the drawee from the drawees list, so he/she won't be picked again.
				drawees.Remove (drawee);
				
				//format and print out the email string to send to the draweer
				System.Console.WriteLine (emailFormat, drawer, g);
				
				//Open a file to write out the html containing the drawee's name
				TextWriter tw = new StreamWriter (g.ToString() + ".html");
				
				//Format and write the html to the file.
				tw.Write (htmlFormat, drawee);
				
				//save and close.
				tw.Close ();
				
				//continue the loop until we are done!
 			}
		}
	}
}
