using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin.Collections
{
    public class ImmutableList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        private readonly IList<T> _list;

        public ImmutableList(IList<T> list)
        {
            Debug.Assert(list != null);
            _list = list;
        }

        public static ImmutableList<T> Empty => new ImmutableList<T>(new List<T>());

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        public static implicit operator ImmutableList<T>(List<T> l) => new ImmutableList<T>(l);
    }

    public static class ImmutableExtensions
    {
        public static IReadOnlyCollection<T> ToReadonlyCollection<T>(this IList<T> list) => new ImmutableList<T>(list);

        public static IReadOnlyList<T> ToReadonlyList<T>(this IList<T> list) => new ImmutableList<T>(list);
    }
}
