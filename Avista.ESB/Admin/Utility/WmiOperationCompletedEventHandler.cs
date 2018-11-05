using System;
using System.Management;
using System.Threading;

namespace Avista.ESB.Admin.Utility
{
    /// <summary>
    /// Implements a CompletedEventHandler delegate.
    /// </summary>
    public class WmiOperationCompletedEventHandler
    {
        /// <summary>
        /// The completed state flag.
        /// </summary>
        private bool complete = false;

        /// <summary>
        /// The completed event arguments.
        /// </summary>
        private CompletedEventArgs eventArgs = null;

        /// <summary>
        /// The operation observer.
        /// </summary>
        ManagementOperationObserver observer = null;

        /// <summary>
        /// Creates an operation completed handler registered with an observer to receive a Completed event callback.
        /// </summary>
        public WmiOperationCompletedEventHandler()
        {
            observer = new ManagementOperationObserver();
            observer.Completed += new CompletedEventHandler(this.Completed);
        }

        /// <summary>
        /// The operation observer associated with this handler.
        /// </summary>
        public ManagementOperationObserver Observer
        {
            get
            {
                return observer;
            }
        }

        /// <summary>
        /// The completed event method. This method will be called to signal when the event is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Completed(object sender, CompletedEventArgs args)
        {
            complete = true;
            eventArgs = args;
        }

        /// <summary>
        /// Returns a flag indicating whether or not the operation has completed.
        /// </summary>
        public bool Complete
        {
            get
            {
                return complete;
            }
        }

        /// <summary>
        /// If the operation has complete, this method gets the status that the operation completed with. Otherwise it returns an empty string.
        /// </summary>
        public string Status
        {
            get
            {
                string status = String.Empty;
                if (eventArgs != null)
                {
                    status = eventArgs.Status.ToString();
                }
                return status;
            }
        }

        /// <summary>
        /// Wait up to a given number of seconds for the completion event to occur.
        /// </summary>
        /// <param name="waitTimeSec">The maximum number of seconds to wait.</param>
        public void WaitForCompletion(int waitTimeSec)
        {
            DateTime timeout = DateTime.Now + new TimeSpan(0, 0, waitTimeSec);
            WaitForCompletion(timeout);
        }

        /// <summary>
        /// Wait for the completion event or until the timeout datetime has passed.
        /// </summary>
        /// <param name="timeout">The datetime at which a timeout exception should be raised.</param>
        public void WaitForCompletion(DateTime timeout)
        {
            // Wait until the operation has completed or we time out.
            while (!Complete && DateTime.Now < timeout)
            {
                Thread.Sleep(25);
            }
            // If the operation did not complete then we must have timed out.
            if (!Complete)
            {
                observer.Cancel();
                throw new Exception("The WMI operation did not complete within the time limit.");
            }
            // The operation completed, but we must check its status.
            if (Status != "NoError")
            {
                throw new Exception("The WMI operation encountered an error. " + Status.ToString());
            }
        }
    }
}
