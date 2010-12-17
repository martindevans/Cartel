using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace Cartel
{
    public abstract class Feed : IObservable<FeedEntry>
    {
        private ConcurrentDictionary<Subscription, IObserver<FeedEntry>> observers = new ConcurrentDictionary<Subscription, IObserver<FeedEntry>>();

        public int ObserversCount
        {
            get
            {
                return observers.Count;
            }
        }

        protected void PushNext(FeedEntry entry)
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnNext(entry);
        }

        protected void PushCompleted()
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnCompleted();
        }

        protected void PushError(Exception error)
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnError(error);
        }

        public IDisposable Subscribe(IObserver<FeedEntry> observer)
        {
            return new Subscription(observer, observers);
        }

        private class Subscription : IDisposable
        {
            ConcurrentDictionary<Subscription, IObserver<FeedEntry>> set;
            IObserver<FeedEntry> observer;

            public Subscription(IObserver<FeedEntry> observer, ConcurrentDictionary<Subscription, IObserver<FeedEntry>> set)
            {
                this.set = set;
                if (!set.TryAdd(this, observer))
                    throw new InvalidOperationException("This subscription cannot already be in the dictionary");
            }

            public void Dispose()
            {
                IObserver<FeedEntry> o;
                set.TryRemove(this, out o);
            }
        }
    }
}
