﻿using System.Collections.Concurrent;

namespace SF.Pool
{
    internal class ProducerConsumerPool<TGetter> : IPool<TGetter>
    {
        private readonly PooledFactory<TGetter> _factory;
        private readonly IProducerConsumerCollection<TGetter> _storage;

        public ProducerConsumerPool(PooledFactory<TGetter> factory,
            IProducerConsumerCollection<TGetter> storage = null)
        {
            _factory = factory;
            _storage = storage ?? new ConcurrentBag<TGetter>();
        }


        public TGetter Get => _storage.TryTake(out TGetter getFromPool) ? getFromPool : _factory(Return);

        private void Return(TGetter getter)
        {
            _storage.TryAdd(getter);
        }
    }
}