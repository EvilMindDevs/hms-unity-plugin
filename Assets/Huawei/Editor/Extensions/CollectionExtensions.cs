using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin.Collections
{
    // A way to have readonly list interfaces
    // while at the same time supporting older .net until it is no longer available
    public class ImmutableList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private readonly IList<T> _list;

        public ImmutableList(IList<T> list)
        {
            Debug.Assert(list != null);
            _list = list;
        }

        public static ImmutableList<T> Empty()
        {
            return new ImmutableList<T>(new List<T>());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public static implicit operator ImmutableList<T>(List<T> l)
        {
            return new ImmutableList<T>(l);
        }
    }

    public static class ImmutableExtensions
    {
        public static IReadOnlyCollection<T> ToReadonlyCollection<T>(this IList<T> list)
        {
            return new ImmutableList<T>(list);
        }

        public static IReadOnlyList<T> ToReadonlyList<T>(this IList<T> list)
        {
            return new ImmutableList<T>(list);
        }
    }
}