using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using Cartel.Feeds;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReader reader = XmlReader.Create("https://github.com/martindevans/Cartel/commits/master.atom");
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items)
            {
                Console.WriteLine(item.Title.Text);
            }

            PeriodicRssAtom feed2 = new PeriodicRssAtom(TimeSpan.FromMinutes(1), "https://github.com/martindevans/Cartel/commits/master.atom");

            feed2.Start();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
