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
            ConcurrentDictionary<string, object> distinct = new ConcurrentDictionary<string, object>();

            var lastFmFeed = new PeriodicallyPolledFeed<SyndicationItem>(TimeSpan.FromSeconds(30), new PolledRssAtom("http://ws.audioscrobbler.com/1.0/user/martindevans/recenttracks.rss") { ForceUpdate = true });
            //PolledRssAtom twitterFeed = new PolledRssAtom("http://gtweetapp.appspot.com/feed/oa-d9044918-1165-494e-a3fc-d7bb6e8f18f7/with-friends") { ForceUpdate = true };

            lastFmFeed
                .OrderBy(b => b.PublishDate, TimeSpan.FromSeconds(1))
                .Filter(a => a.Id + a.PublishDate, distinct)
                .RemoveRepeatedSequences((a, b) => (a == null && b != null) || (b == null && a != null) || a.Title.Text.ToLowerInvariant() == b.Title.Text.ToLowerInvariant())
                .Subscribe(
                    a => Console.WriteLine(a.Title.Text),
                    a => Console.WriteLine("Feed Error " + a),
                    () => Console.WriteLine("Feed complete")
            );
            lastFmFeed.Start();

            //twitterFeed.AsObservable()
            //    .OrderBy(b => b.PublishDate, TimeSpan.FromSeconds(5))
            //    .Filter(a => a.Id, distinct)
            //    .RemoveRepeatedSequences((a, b) => (a == null && b != null) || (b == null && a != null) || a.Title.Text.ToLowerInvariant() == b.Title.Text.ToLowerInvariant())
            //    .Subscribe(
            //        a => Console.WriteLine(a.Title.Text),
            //        a => Console.WriteLine("Feed Error" + a),
            //        () => Console.WriteLine("Feed Complete")
            //);
            //twitterFeed.Start();

            Console.WriteLine("Done, press any key to exit");
            Console.ReadKey();
        }
    }
}
