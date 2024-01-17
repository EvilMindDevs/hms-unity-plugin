using System;

namespace HmsPlugin
{
    public interface ICollectionManipulator : IDisposable
    {
        event Action OnRefreshRequired;
    }
}
