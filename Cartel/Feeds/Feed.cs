﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Cartel.Feeds
{
    public abstract class Feed : IObservable<Feed.Entry>
    {
        #region fields
        private ConcurrentDictionary<Subscription, IObserver<Entry>> observers = new ConcurrentDictionary<Subscription, IObserver<Entry>>();

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
        protected void PushNext(Entry entry)
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
        #endregion

        #region subscribe
        public IDisposable Subscribe(IObserver<Entry> observer)
        {
            return new Subscription(observer, observers);
        }

        private class Subscription : IDisposable
        {
            ConcurrentDictionary<Subscription, IObserver<Entry>> set;
            IObserver<Entry> observer;

            public Subscription(IObserver<Entry> observer, ConcurrentDictionary<Subscription, IObserver<Entry>> set)
            {
                this.set = set;
                if (!set.TryAdd(this, observer))
                    throw new InvalidOperationException("This subscription cannot already be in the dictionary");
            }

            public void Dispose()
            {
                IObserver<Entry> o;
                set.TryRemove(this, out o);
            }
        }
        #endregion

        public class Entry
            :DynamicObject
        {

        }
    }
}
