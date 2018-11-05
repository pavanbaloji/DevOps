using System;
using Avista.ESB.Admin;


namespace Avista.ESB.Testing.Integration
{
    public class CustomSendStop : BizTalkSendStop
    {
        public CustomSendStop(string sendPortName, string soapAction)
            : base(sendPortName, soapAction)
        {
        }

        /// <summary>
        /// Here is where you must provide a way to insinuate this <see cref="BizTalkSendStop"/> into the BizTalk system.
        /// </summary>
        public Action<BizTalkManager, Uri> SetupAction { get; set; }

        /// <summary>
        /// Here is where you must provide a way to clean up the insinuation of this <see cref="BizTalkSendStop"/>.
        /// </summary>
        public Action<BizTalkManager> CleanupAction { get; set; }


        public override void Setup(BizTalkManager manager, Uri moxyUri)
        {
            if (SetupAction == null)
                throw new InvalidOperationException("The Setup action has not been configured for this Send Stop.");
            SetupAction.Invoke(manager, moxyUri);
            base.Setup(manager, moxyUri);
        }

        public override void Cleanup(BizTalkManager manager)
        {
            if (CleanupAction == null)
                throw new InvalidOperationException("The Cleanup action has not been configured for this Send Stop.");
            CleanupAction.Invoke(manager);
        }
    }
}