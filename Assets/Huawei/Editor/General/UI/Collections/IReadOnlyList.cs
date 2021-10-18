using System.Collections.Generic;

namespace HmsPlugin.Collections
{
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>,
        IEnumerable<T>
    {
        T this[int index] { get; }
    }
}