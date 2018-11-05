using System.Net;

namespace Avista.ESB.Testing.Integration
{
    /// <summary>
    ///     Represents a sendstop whose send port URI will be flip-flopped
    /// </summary>
    public interface IFaultingSendPort : IBizTalkSendStop
    {
        /// <summary>
        ///     Set whatever values on the response are needed to simulate an communication error.
        /// </summary>
        /// <param name="response">The response to return to the caller.</param>
        void SetErrorHttpResponse(HttpListenerResponse response);
    }
}