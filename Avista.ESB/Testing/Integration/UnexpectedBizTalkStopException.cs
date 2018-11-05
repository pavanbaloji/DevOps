using System;

namespace Avista.ESB.Testing.Integration
{
    public class UnexpectedBizTalkStopException : Exception
    {
        public UnexpectedBizTalkStopException()
        {
        }

        public UnexpectedBizTalkStopException(string message)
            : base(message)
        {
        }

        public UnexpectedBizTalkStopException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}