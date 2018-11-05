using System;

namespace Avista.ESB.Utilities.Components
{
    public interface IComponent : IDisposable
    {
        string Name { get; }
        void RefreshConfiguration();
    }
}
