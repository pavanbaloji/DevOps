
using System;
using Avista.ESB.Utilities.Components;

namespace Avista.ESB.Utilities.Security
{
    public interface ICredentials : IComponent
    {
        string UserId
        {
            get;
        }

        string Password
        {
            get;
        }
    }
}
