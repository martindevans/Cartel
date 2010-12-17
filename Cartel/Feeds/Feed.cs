using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Cartel.Feeds
{
    public abstract class Feed : IObservable<SyndicationItem>
    {
        #region fields
        private ConcurrentDictionary<Subscription, IObserver<SyndicationItem>> observers = new ConcurrentDictionary<Subscription, IObserver<SyndicationItem>>();

        public int Observers
        {
            get
            {
                return observers.Count;
            }
        }
        #endregion

        public Feed()
        {

        }

        #region push
        protected void PushNext(SyndicationItem SyndicationItem)
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnNext(SyndicationItem);
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
        #endregion

        #region subscribe
        public IDisposable Subscribe(IObserver<SyndicationItem> observer)
        {
            return new Subscription(observer, observers);
        }

        private class Subscription : IDisposable
        {
            ConcurrentDictionary<Subscription, IObserver<SyndicationItem>> set;
            IObserver<SyndicationItem> observer;

            public Subscription(IObserver<SyndicationItem> observer, ConcurrentDictionary<Subscription, IObserver<SyndicationItem>> set)
            {
                this.set = set;
                if (!set.TryAdd(this, observer))
                    throw new InvalidOperationException("This subscription cannot already be in the dictionary");
            }

            public void Dispose()
            {
                IObserver<SyndicationItem> o;
                set.TryRemove(this, out o);
            }
        }
        #endregion
    }
}
