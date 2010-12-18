using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using Cartel.Feeds;
using System.Diagnostics;
using System.Collections.Concurrent;
using Cartel.Extensions;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PeriodicRssAtom lastFmFeed = new PeriodicRssAtom(TimeSpan.FromSeconds(5), "http://ws.audioscrobbler.com/1.0/user/martindevans/recenttracks.rss");

            ConcurrentDictionary<string, object> distinct = new ConcurrentDictionary<string, object>();
            SyndicationItem item = null;
            lastFmFeed.AsObservable()
                .Filter<SyndicationItem>(a => a.Id + a.PublishDate, distinct)
                .RemoveRepeatedSequences((a, b) => (a == null && b != null) || (b == null && a != null) || a.Title.Text.ToLowerInvariant() == b.Title.Text.ToLowerInvariant())
                .Subscribe(
                    a => Console.WriteLine(a.Title.Text),
                    a => Console.WriteLine("Feed Error " + a),
                    () => Console.WriteLine("Feed complete")
                );

            lastFmFeed.Start();

            Console.WriteLine("Done, press any key to exit");
            Console.ReadKey();
        }
    }
}
