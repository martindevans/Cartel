using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Cartel.Feeds
{
    public class PeriodicRssAtom
        :PeriodicallyPolledFeed
    {
        public readonly string Uri;

        public PeriodicRssAtom(TimeSpan period, string uri)
            :base(period)
        {
            Uri = uri;
        }

        protected override void Poll(DateTime lastUpdated)
        {
            var reader = XmlReader.Create(Uri);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed.LastUpdatedTime > lastUpdated)
            {
                foreach (var item in feed.Items)
                {
                    try
                    {
                        PushNext(item);
                    }
                    catch (Exception e)
                    {
                        PushError(e);
                    }
                }
                lastUpdated = DateTime.Now;
            }
        }
    }
}
