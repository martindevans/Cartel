using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using Cartel.Feeds;
using System.Diagnostics;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PeriodicRssAtom lastFmFeed = new PeriodicRssAtom(TimeSpan.FromSeconds(1), "http://ws.audioscrobbler.com/1.0/user/martindevans/recenttracks.rss");
            lastFmFeed.Subscribe((a) => Console.WriteLine(a.Title.Text));

            lastFmFeed.Start();

            Console.WriteLine("Done, press any key to exit");
            Console.ReadKey();
        }
    }
}
