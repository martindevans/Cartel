using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cartel.Feeds
{
    public abstract class PeriodicallyPolledFeed
        :Feed
    {
        private long period = TimeSpan.FromMinutes(1).Ticks;
        public TimeSpan Period
        {
            get
            {
                return new TimeSpan(Interlocked.Read(ref period));
            }
            set
            {
                Interlocked.Exchange(ref period, value.Ticks);
            }
        }

        protected const long TRUE = 1;
        protected const long FALSE = 0;
        protected long started = FALSE;

        private Timer timer;

        public PeriodicallyPolledFeed(TimeSpan period)
        {
            Period = period;

            timer = new Timer((a) => 
            {
                if (Interlocked.Read(ref started) == TRUE)
                    Poll();
            });
        }

        public void Start()
        {
            Interlocked.Exchange(ref started, TRUE);

            long p = Interlocked.Read(ref period);
            timer.Change(0, p);
        }

        public void Pause()
        {
            Interlocked.Exchange(ref started, FALSE);

            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        protected abstract void Poll();
    }
}
