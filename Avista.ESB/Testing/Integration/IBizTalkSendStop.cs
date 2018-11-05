using System;
using Avista.ESB.Admin;


namespace Avista.ESB.Testing.Integration
{
    public interface IBizTalkSendStop
    {
        /// <summary>
        /// Arguments to be passed to the <seealso cref="SimulatedOutput"/> method.
        /// </summary>
        object[] ExtraIncomingArgs { get; set; }

        /// <summary>
        /// If set, the proxy will check this value
        /// </summary>
        string ExpectedIncomingPayload { get; set; }

        /// <summary>
        /// The name of the BizTalk Send Port this stop will attach to.
        /// </summary>
        string SendPortName { get; }

        /// <summary>
        /// The SoapAction of the send port in question.
        /// </summary>
        string SoapAction { get; }

        /// <summary>
        /// 
        /// </summary>
        Uri SendPortUri { get; }

        /// <summary>
        /// This gets set to True when the Send Stop has been hit.
        /// </summary>
        bool RequestCaught { get; set; }

        /// <summary>
        /// If this is set to something, the proxy will return this instead of going on to whatever was requested
        /// This can theoretically be used to phase out all the mock services
        /// </summary>
        Func<string, object[], string> SimulatedOutput { get; set; }

        /// <summary>
        /// Called to set up the Send Stop to insert itself into the BizTalk pipeline.
        /// </summary>
        /// <param name="manager">A <see cref="BizTalkManager"/> to act on the current BizTalk instance.</param>
        /// <param name="moxyUri">The <see cref="Uri"/> of the Moxy server.</param>
        void Setup(BizTalkManager manager, Uri moxyUri);

        /// <summary>
        /// Called after a test run to revert any changes made by the Send Stop.
        /// </summary>
        /// <param name="manager">A <see cref="BizTalkManager"/> to act on the current BizTalk instance.</param>
        void Cleanup(BizTalkManager manager);
    }
}