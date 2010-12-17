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

        private XmlReader reader;

        public PeriodicRssAtom(TimeSpan period, string uri)
            :base(period)
        {
            Uri = uri;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
            };
            reader = XmlReader.Create(uri, settings);
        }

        protected override void Poll()
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            foreach (var item in feed.Items)
            {
                try
                {
                    PushNext(ToEntry(item));
                }
                catch (Exception e)
                {
                    PushError(e);
                }
            }
        }

        private Feed.Entry ToEntry(SyndicationItem item)
        {
            throw new NotImplementedException();
        }
    }
}
