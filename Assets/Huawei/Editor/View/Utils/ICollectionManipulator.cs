using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public interface ICollectionManipulator : IDisposable
    {
        event Action OnRefreshRequired;
    }
}
