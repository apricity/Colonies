namespace Wacton.Tovarisch.Types
{
    using System;
    using System.Collections.Generic;

    public class TypeDictionary<TValue> : Dictionary<Type, TValue>
    {
        public TValue Get<T>()
        {
            return this[typeof(T)];
        }

        public void Add<T>(TValue value)
        {
            this.Add(typeof(T), value);
        }

        public bool Remove<T>()
        {
            return this.Remove(typeof(T));
        }

        public bool TryGetValue<T>(out TValue value)
        {
            return this.TryGetValue(typeof(T), out value);
        }

        public bool ContainsKey<T>()
        {
            return this.ContainsKey(typeof(T));
        }
    }
}