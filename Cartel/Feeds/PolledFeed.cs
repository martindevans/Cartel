using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cartel.Feeds
{
    public abstract class PolledFeed<T>
        :Feed<T>
    {
        public abstract void Poll();

        public event Action<TimeSpan> FoundNewTtl;

        protected void NewTtl(TimeSpan time)
        {
            if (FoundNewTtl != null)
                FoundNewTtl(time);
        }

        protected bool ParseTtl
        {
            get
            {
                return FoundNewTtl != null;
            }
        }
    }
}
