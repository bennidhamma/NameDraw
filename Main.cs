using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;

namespace NameDraw
{
	class MainClass
	{		
		static float ExcludeProbability = 0.8f;
		static bool DryRun = true;
		static string From = "bennidhamma@gmail.com";
		static string Subject = "IMPORTANT!! Joldersma Famly Name Draw 2013! -- DATED MATERIAL - OPEN IMMEDIATELY!!";
		static Random random = new Random();


		public static void Main (string[] args)
		{
			//The list of participants, first as a collection of drawers.
			var drawers = new List<Entry> ();

			foreach (var line in File.ReadAllLines ("names"))
			{
				var lineParts = line.Split (',');
				var entry = new Entry () {
					Name = lineParts[0],
					Email = lineParts[1],
					Exclusions = lineParts.Length > 2 ? lineParts[2].Split(':') : new string[] {}
				};
				drawers.Add (entry);
			}

			var emailFormat = File.ReadAllText ("emailFormat");
			
			//Copy the list to create a shrinking pool of drawees.
			//The algorithm here is to shrink the pool by one each time we successfully find a match.
			var drawees = new List<Entry> (drawers);
			
			//Request a list of 100 truly random numbers from RANODM.ORG.
			WebClient wc = new WebClient ();
			StreamReader sr = new StreamReader (wc.OpenRead ("http://www.random.org/integers/?num=100&min=1&max=100&col=1&base=10&format=plain&rnd=new"));
			
			//Create a delegate (callback function) that reads one line from the web request, converts it to an integer, and returns that value.
			var readInt = new Func<int> (delegate {
				return int.Parse (sr.ReadLine ());
			});
			
			//loop through each drawer
			foreach (var drawer in drawers)
			{
				Entry drawee = null;
				//In order to avoid drawing yourself, we keep picking a drawee until we have one that is not the drawer.  
				//In other words, keep drawing a drawee while that drawee is the same as the drawer (usually this will only happen once).
				do {
					//select a name from the drawees list by reading an integer from the web stream, and modulating the number by the size of the 
					//drawees list.
					drawee = drawees [readInt () % drawees.Count];
				} while(!IsValidDraw(drawer, drawee));

				if (drawer.WhoYouDrew != null)
					throw new Exception (string.Format ("{0} already drew {1}", drawer, drawer.WhoYouDrew));
				if (drawee.WhoDrewYou != null)
					throw new Exception (string.Format("Sorry {0}, {1} already drew {2}", drawer.Name, drawee.WhoDrewYou, drawee));

				drawer.WhoYouDrew = drawee;
				drawee.WhoDrewYou = drawer;

				//now remove the drawee from the drawees list, so he/she won't be picked again.
				drawees.Remove (drawee);
				
				//format and print out the email string to send to the draweer
				//System.Console.WriteLine (emailFormat, drawer, drawee);
								
				//continue the loop until we are done!
 			}

			//validate selections. make sure that everyone both drew someone and is drawn by someone.
			HashSet<Entry> hasBeenDrawn = new HashSet<Entry> ();
			drawees = new List<Entry> (drawers);

			foreach (var entry in drawers)
			{
				if (hasBeenDrawn.Contains (entry.WhoYouDrew))
					throw new Exception (entry.WhoYouDrew + " has already been drawn");
				hasBeenDrawn.Add (entry.WhoYouDrew);
				drawees.Remove (entry.WhoYouDrew);
				if (entry.WhoYouDrew.WhoDrewYou != entry)
					throw new Exception ("Draw mismatch");
				if (ExcludeProbability == 1 && entry.Exclusions.Contains (entry.WhoYouDrew.Name))
					throw new Exception ("Exclusion drew");
			}

			if (drawees.Count > 0)
				throw new Exception ("Not everyone was drawn");

			Console.WriteLine ("Sanity verified. okay to email.");

			//email!
			Mailer m = new Mailer (From);
			foreach (var entry in drawers) {
				string body = string.Format (emailFormat, entry.Name, entry.WhoYouDrew.Name);
				if (DryRun) {
					Console.WriteLine (body);
				} else {
					m.Send (entry.Email, entry.Name, Subject, body);
				}
			}
		}

		private static bool IsValidDraw (Entry drawer, Entry drawee)
		{
			if (drawee == drawer) {
				return false;
			}

			if (random.NextDouble() < ExcludeProbability && drawer.Exclusions.Contains(drawee.Name)) {
				return false;
			}

			return true;
		}
	}

	public class Entry {
		public string Name {get; set;}
		public string Email {get; set;}
		public string[] Exclusions {get; set;}

		public Entry WhoYouDrew {get; set;}
		public Entry WhoDrewYou {get; set;}

		public override bool Equals (object obj)
		{
			return (obj as Entry).Name == Name;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}
