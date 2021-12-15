using System;

namespace HmsPlugin.Dropdown
{
    public interface IDropdown<T>
    {
        event Action<T> OnChangedSelection;
    }
}