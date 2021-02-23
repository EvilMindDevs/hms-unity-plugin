using System;

namespace HmsPlugin.Dropdown
{
    public interface IDropdown
    {
        event Action OnChangedSelection;
    }
}