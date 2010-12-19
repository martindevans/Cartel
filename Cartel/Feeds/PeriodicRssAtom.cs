﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Cartel.Feeds
{
    public class PeriodicRssAtom
        :PeriodicallyPolledFeed<SyndicationItem>
    {
        public readonly string Uri;

        private DateTime latestItemWithDatestamp = DateTime.MinValue;
        private HashSet<Guid> receivedNonDatestampedItems = new HashSet<Guid>();

        public bool ForceUpdate { get; set; }

        public bool RespectTtl { get; set; }

        public PeriodicRssAtom(TimeSpan period, string uri)
            :base(period)
        {
            Uri = uri;
        }

        protected override void Poll(DateTime lastUpdated)
        {
            var reader = XmlReader.Create(Uri);

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            TimeSpan? setTtl = null;
            if (RespectTtl)
            {
                var ttl = feed.ElementExtensions.Where(a => a.OuterName == "ttl");
                if (ttl.Count() != 0)
                {
                    var r = ttl.First().GetReader();
                    string s = r.ReadElementString();
                    try
                    {
                        TimeSpan time = TimeSpan.FromMinutes(Int32.Parse(s));
                        setTtl = time;
                    }
                    catch (XmlException x) { /* swallow and igonore ttl */ }
                    catch (FormatException f) { /* swallow and igonore ttl */ }
                }
            }

            if (ForceUpdate || feed.LastUpdatedTime > lastUpdated || feed.LastUpdatedTime.DateTime == default(DateTime))
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
                lastUpdated = feed.LastUpdatedTime.DateTime;
            }

            if (setTtl.HasValue)
            {
                Period = setTtl.Value;
                Pause();
                Start();
            }
        }
    }
}
