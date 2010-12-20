using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cartel.Feeds
{
    /// <summary>
    /// A data feed which is periodically polled to check for new data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PeriodicallyPolledFeed<T>
        :IObservable<T>
    {
        DateTime lastUpdated = DateTime.MinValue;

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

        private PolledFeed<T> feed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicallyPolledFeed&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="period">The period.</param>
        public PeriodicallyPolledFeed(TimeSpan period, PolledFeed<T> feed)
        {
            Period = period;

            this.feed = feed;

            timer = new Timer((a) => 
            {
                lock (timer)
                {
                    try
                    {
                        if (Interlocked.Read(ref started) == TRUE)
                        {
                            feed.Poll();
                            lastUpdated = DateTime.Now;
                        }
                    }
                    catch (Exception e)
                    {
                        feed.PushError(e);
                    }
                }
            }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Start polling
        /// </summary>
        /// <returns>this feed</returns>
        public PeriodicallyPolledFeed<T> Start()
        {
            Interlocked.Exchange(ref started, TRUE);

            long p = Interlocked.Read(ref period);
            timer.Change(TimeSpan.FromTicks(0), TimeSpan.FromTicks(p));

            return this;
        }

        /// <summary>
        /// Pause polling
        /// </summary>
        /// <returns>this feed</returns>
        public PeriodicallyPolledFeed<T> Pause()
        {
            Interlocked.Exchange(ref started, FALSE);

            timer.Change(Timeout.Infinite, Timeout.Infinite);

            return this;
        }

        /// <summary>
        /// Send a completed messages to observers
        /// </summary>
        /// <returns>this feed</returns>
        public PeriodicallyPolledFeed<T> Stop()
        {
            Pause();
            feed.PushCompleted();
            return this;
        }

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            return feed.Subscribe(observer);
        }
    }
}
