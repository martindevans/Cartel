using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Diagnostics;

namespace Cartel.Feeds
{
    public class PolledRssAtom
        :PolledFeed<SyndicationItem>
    {
        public readonly string Uri;

        private DateTime latestItemWithDatestamp = DateTime.MinValue;
        private HashSet<Guid> receivedNonDatestampedItems = new HashSet<Guid>();

        DateTime lastPolled = DateTime.MinValue;

        public bool ForceUpdate { get; set; }

        public PolledRssAtom(string uri)
            :base()
        {
            Uri = uri;
        }

        public override void Poll()
        {
            var reader = XmlReader.Create(Uri);

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            TimeSpan? setTtl = null;
            if (ParseTtl)
            {
                var ttl = feed.ElementExtensions.Where(a => a.OuterName == "ttl");
                if (ttl.Count() != 0)
                {
                    var r = ttl.First().GetReader();
                    string s = r.ReadElementString();
                    try
                    {
                        TimeSpan time = TimeSpan.FromMinutes(Int32.Parse(s));
                        NewTtl(time);
                    }
                    catch (XmlException x) { Trace.TraceWarning(x.ToString()); }
                    catch (FormatException f) { Trace.TraceWarning(f.ToString()); }
                }
            }

            if (ForceUpdate || feed.LastUpdatedTime > lastPolled || feed.LastUpdatedTime.DateTime == default(DateTime))
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
            }

            lastPolled = DateTime.Now;
        }
    }
}
