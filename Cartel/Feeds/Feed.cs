using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Cartel.Feeds
{
    /// <summary>
    /// An observable feed of data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Feed<T> : IObservable<T>
    {
        #region fields
        private ConcurrentDictionary<Subscription, IObserver<T>> observers = new ConcurrentDictionary<Subscription, IObserver<T>>();

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
        public void PushNext(T item)
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnNext(item);
        }

        public void PushCompleted()
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnCompleted();
        }

        public void PushError(Exception error)
        {
            foreach (var o in observers.Values.AsParallel())
                o.OnError(error);
        }
        #endregion

        #region subscribe
        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            return new Subscription(observer, observers);
        }

        private class Subscription : IDisposable
        {
            ConcurrentDictionary<Subscription, IObserver<T>> set;
            IObserver<T> observer;

            public Subscription(IObserver<T> observer, ConcurrentDictionary<Subscription, IObserver<T>> set)
            {
                this.set = set;
                if (!set.TryAdd(this, observer))
                    throw new InvalidOperationException("This subscription cannot already be in the dictionary");
            }

            public void Dispose()
            {
                IObserver<T> o;
                set.TryRemove(this, out o);
            }
        }
        #endregion
    }
}
