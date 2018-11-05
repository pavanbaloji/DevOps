using System;

namespace Avista.ESB.Testing.Integration
{
    public class BizTalkStopSoapActionException : Exception
    {
        public BizTalkStopSoapActionException()
        {
        }

        public BizTalkStopSoapActionException(string message)
            : base(message)
        {
        }

        public BizTalkStopSoapActionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}