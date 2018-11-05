using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.BizTalk.Management;
using Microsoft.BizTalk.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avista.ESB.Admin.Utility;
using Avista.ESB.Admin;
using Avista.ESB.Utilities.Logging;
#pragma warning disable 1584,1711,1572,1581,1580

#region Overview Docs

/*
 
    System Test Design (aka "Moxy") 
    -------------------------------- 
        The Moxy test framework, which is comprised of a web server that acts as a proxy (MoxyServer) 
        and a framework in HP.Practices.BizTalk.Testing\Integration facilitates
        authoring "rigorous" tests for various "typical" system tests: a) happy path, b) routing fault, c) soap exception, d)
        successful response but it is an error response.

        By 'rigorous', we mean we want to ensure we have a mechanism to ensure we are consistently validating a set of
        characteristics as they relate to the scenarios above.

        There are three aspects to the Moxy framework of benefit to the developer: 
            1) Your system test code will be more DRY as most tests have a certain amount of boilerplate setup/teardown 
            2) You can verify the orchestration is going to a specific sendport/action in an expected order
            3) Simplifying the way we stub (mock) 3rd party services and their data output.

        Other benefits of the framework are:
            1. We no longer need to maintain projects/installers for Mock services that run within our local IIS 
            2. We can run tests on any environment where we may not be allowed to, or for whatever reason don’t want to, 
            set up our entire local Mock infrastructure (e.g. in staging environments)


        The main pieces of the framework are:
            1. BizTalkSendStop: An instance of this represents a send port / action that we expect the orchestration to stop at along
            the way
            2. MoxyServer: This acts as a proxy server.  if this BizTalkSendStop has simulated output, we return that, otherwise
            we continue on to the original request.  This allows us to slowly deprecate the mock services rather than have an "all
            or nothing" paradigm.  

        See HP.Practices.BizTalk.Testing\SystemTestVisualization.jpg to see how the pieces of the framework fit together.



    Setting up the tests 
    ----------------------
        1. While not necessary, it is convenient to use a _Convenience solution that contains at a minimum: 
	        - HP.Practices.BizTalk.Testing 
	        - Your test project 
           See \dev\Avista.ESB.BillPaymentManagement\_Convienence.sln as an example.

           The main reason for this is that you may find yourself in a situation where you wan to step into the MoxyServer
           which lives at HP.Practices.BizTalk.Testing\Integration while you're working on your system tests.

           You should be able to just step across the projects.  If you want to modify the HP project and your System test
           simultaneously, refer to \dev\Avista.ESB.BillPaymentManagement\HOW_TO_USE_THE_CONVENIENCE_SLN.txt for instructions.

        2. Follow the protoypical examples for your system tets scenario (happy path, failed routing, etc.) 
           pointed out in SystemTestBase.cs

    PRO TIPS
    ---------
        - If you don't have a BizTalkSendStop in your system test, consider whether or not you are doing something wrong. While
          you can leverage the framework's syntactic sugar to rewrite old system tests in a more DRY fashion that still do the exact
          same thing, that is not the main intent. Points (2) and (3) in section ("...main aspects..." above) are.
   
        - Your system should still theoretically work with all the mock services undeployed.  You may want the mock services
          deployed initially, but if the test is really "done" it shouldn't rely on them.
  
        - Put into your test base.SetPortsOnAllProxy() and base.ClearPortsOnAllProxy() to set / clear proxy on sendports (but remove after)  
           Helpful if you get into a funky state. 

    
 
    Parellization Considerations 
  -------------------------------
        - VS and the build server runs tests synchronously by defrault.  It may not even be needed to parallelize tests, but 
          here are some considerations.  Search this class for "parallelization". 
          Basic approach:  
            - Each test gets its own moxy server and the port is dynamically found and bound 
 * - Add a .testsettings file to the .sln with the content:
 <?xml version="1.0" encoding="UTF-8"?>
<TestSettings
  id="8cf5bc64-1197-4029-a250-4a3e4ac4937f"
  name="TestSettings1"
  enableDefaultDataCollectors="false"
  xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
  <Description><!--_locID_text="Description1"-->These are default test settings for a local test run.</Description>
  <Deployment enabled="true" />
<Execution parallelTestCount="5"/>
</TestSettings>
 (where "5" is whatever you want...) and set the Test->Test Settings file in VS to it.  Your tests will now be in parallel. 
 Tinker with setting ordering of tests so that they are segregated and won't step on each other.  At a minimum, you should be able to have
 each class 

  - Make sure you have your .testsettings file in the .sln and set the <Execution parallelTestRuns=
  - dynamically get a port (see commented out testinitialize code in this file)
  - Set the <eventLoggingProvider name="WindowsEventLogger" class="HP.Practices.EventLogging.WindowsEventLogger" assembly="HP.Practices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e99439949532ab91"/>
         to a custom event logger that you will create.  This will be the magic to be smart enough to get a logger-per-test.  Perhaps it creates a an entire new event log section on the fly e.g. application_mytest
  - Tests will need to be "binned" so they work with mutually exclusive ports.  This should provide
  enough parallelization. Unfortunately this might only be achievable with the xunit framework.
  
 */

#endregion

namespace Avista.ESB.Testing.Integration
{
    /// <summary>
    ///     Base class for system tests
    ///     When inheriting from this class, your tests in your test class will:
    ///     -  Have a local proxy to work with that will automatically be started and cleaned up at the end
    ///     -  Have the capability to, on a port by port basis, redirect to the proxy.
    ///     -  Tell the proxy what it should expect through a Queue of BizTalkSendStop(s)
    ///     -  When receiving a request, the proxy will dequeue the nex texpected BizTalkSendStop and perform the expected
    ///     actionToInvokeIntegration.
    /// </summary>
    /// <remarks>
    ///     Happy path example [no warnings / errors to test for]:
    ///     -Putting file in receive location
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTests.TestProcessRemittanceCustPmnt_SuccessfulProcessing" />
    ///     -Sending data to HTTP port
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ManageSchedulePaymentTest.AddSchedulePaymentSuccessTest" />
    ///     Exception pattern for Expected (handled) Error:
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTests.TestProcessFiservV2_CcbVerifyPaymentResponseContainsErrorDesc" />
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessAdjustmentTests.ProcessSolarCredits_WhenDuplicateCheckFails" />
    ///     Generalized pattern - for testing a path has a mixture of event types logged along the path to test for
    ///     YOU CAN USE THIS PATTERN INSTEAD OF THE HAPPY PATH / EXCEPTION PATTERNS
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.BP118_APSCancellationExtractFixture.TestAPSCancellation_CancelAutoPayWithoutAutoPaymentId_SuccessfulProcess" />
    ///     Failed routing path Pattern:
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTests.ProcessRemittanceConfig_SendPortRaisesFailedRoutingEvent" />
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTests.TestProcessRemittance_WhenRecievePortRaisesFailedRoutingException" />
    ///     Nth failed route:
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTests.GivenGoodOnRampData_WhenInvalidRouteOnThirdStop_ExpectFailedRoutingError" />
    ///     Exception happens within MOCK'd method:
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.RemoveWalletItemTest.TestRemoveWalletItem_When_FiservServiceRaisesSoapFault" />
    ///     Pay particular note to the way exceptions are handled at: Mock'd service
    ///     <see cref="HP.Practices.BizTalk.Testing.Mock.Fiserv.MockService.OD_RemoveWalletItem" />
    ///     OracleEBS example:
    ///     <see
    ///         cref="Avista.ESB.BillPaymentManagement.Itineraries.Test.ProcessRemittanceTest.TestProcessRemittanceUsBank_SuccessfulProcessing" />
    /// </remarks>
    /// Considerations:
    /// todo: Terminate all suspended instances between test runs, or at the beginning of the entire test run
    /// 
    /// todo: In the case of expected successful processing tests, should we verify that there are no Error (and maybe Warning) events?.
    /// 
    /// todo: Right now, the framework only sets the proxy on a SendPort if the associated BizTalkSendStop is expected in the system test.
    /// todo (continued):  Consider setting the proxy on EVERYTHING prior to each test then CLEARING it after .. this will ensure that you 
    /// todo (continued): are not missing any sendports in your test.  
    /// todo: SetSendPortProxyOnAll and ClearSendPortProxyOnAll need to be tweaked to accommodate for Oracle ports.
    ///       
    /// todo:  (nice to have) Flag for whether or not you want to wait full amount of time; if not, use EventLogHelper.WaitForEvent2, was a work in progress and abandoned when decided in first code review we weren't going to pursue it)
    [TestClass]
    public abstract class SystemTestBase
    {

        protected static readonly int MinPort = 25000;
        protected static readonly int MaxPort = 35000;
        protected static readonly float DefaultRunEventsWaitSecs = 2.0f;

        protected static readonly BizTalkManager BizTalkManager = new BizTalkManager();
        protected static readonly BizTalkOperations BizTalkOperations = new BizTalkOperations();
        
        protected static int CurrentPort = MinPort;


        public EventLogMonitor EventLogMonitor
        {
            get { return EventLogMonitor.Instance; }
        }


        /// <summary>
        /// TestContext
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///     Implement in an application's test base class, override in individual
        ///     test fixtures as needed, or in specific tests.
        /// </summary>
        protected abstract string EventSource { get; set; }


        /// <summary>
        ///     The maximum number of seconds to wait for events.  Defaults to 30.
        /// </summary>
        protected int MaxEventWaitTimeSeconds { get; set; }

        /// <summary>
        ///     Defaults to 1, set to different value on a per-test basis if needed.
        /// </summary>
        protected int EventCount { get; set; }

        /// <summary>
        ///     The <see cref="Uri" /> where the <see cref="MoxyServer" /> will run.
        /// </summary>
        protected Uri MoxyUri { get; set; }

        /// <summary>
        /// SendPort
        /// </summary>
        protected string SendPort { get; set; }

        /// <summary>
        /// OriginalSendPortUri
        /// </summary>
        protected string OriginalSendPortUri { get; set; }

        /// <summary>
        /// CreateOnRampAction
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resource"></param>
        /// <param name="receiveFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="xmlTemplateArgs"></param>
        /// <returns></returns>
        protected static Action CreateOnRampAction(Assembly assembly,
            string resource,
            string receiveFolder,
            string fileName,
            object[] xmlTemplateArgs)
        {
            Action action = () =>
            {
                var template = new XmlTemplate();
                template.LoadFromResource(assembly, resource);
                template.Execute(xmlTemplateArgs);
                string contents = template.InstanceAsXmlString();
                File.WriteAllText(Path.Combine(receiveFolder, fileName), contents);
            };
            return action;
        }



        /// <summary>
        /// CheckDBAvailible : verify that list of db names can be opened, returns false on any fail.
        /// </summary>
        /// <param name="databaseConnectionNames"></param>
        /// <returns></returns>
        protected void VerifyDatabaseAvailabilty(params string[] databaseConnectionNames)
        {
            if (databaseConnectionNames == null || databaseConnectionNames.Length <= 0)
                return;

            bool success = true;
            foreach (string dbName in databaseConnectionNames)
            {
                using (var connection = new Avista.ESB.Utilities.DataAccess.DatabaseConnection(dbName))
                {
                    try
                    {
                        connection.Open();
                        success = connection.IsOpen();
                    }
                    catch
                    {
                        success = false;
                    }
                    finally
                    {
                        if (connection.IsOpen())
                            connection.Close();
                    }
                }

                // loop quick out
                if (!success)
                {
                    break;
                }
            }

            if (!success)
            {
                var msg = string.Format("Test skipped: Database(s): \"{0}\" unavailable.", string.Join(",", databaseConnectionNames));
                Assert.Inconclusive(msg);
            }
        }

        /// <summary>
        ///     Checks that a Receive Location wrote a Failed Routing event to the Event Log.
        /// </summary>
        /// <param name="eventId">The Failed Routing Event ID.</param>
        /// <param name="receiveLocation">The path to the BizTalk Receive Location.</param>
        /// <param name="fileName">The file name to write to.</param>
        /// <param name="contents">The contents of the file to write.</param>
        protected void CheckReceiveLocationFailedRouting(int eventId,
            string receiveLocation,
            string fileName,
            string contents)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            if (string.IsNullOrEmpty(receiveLocation)) throw new ArgumentNullException("receiveLocation");
            EventLogHelper.ClearEventLog();
            File.WriteAllText(Path.Combine(receiveLocation, fileName), contents);
            IEnumerable<EventLogEntry> ret = EventLogHelper.WaitForEvent(eventId,
                EventSource,
                MaxEventWaitTimeSeconds * 1000,
                1,
                null,
                EventLogEntryType.Error.ToString());
            bool isFound = ret.Count() == 1;

            Assert.IsTrue(isFound, "Did not find event: " + eventId);
        }

        /// <summary>
        ///     Creates an <see cref="Action" /> that initiates a BizTalk integration by writing a file out to a receive location.
        /// </summary>
        /// <param name="resourceName">The name of the embedded resource to write out.</param>
        /// <param name="receiveLocation">The path to the BizTalk Receive Location.</param>
        /// <param name="receiveFileName">The name of the file to write to the BizTalk Receive Location.</param>
        /// <returns>An <see cref="Action" /> that will write an embedded resource file out to a folder.</returns>
        protected Action CreateWriteResourceFileAction(string resourceName,
            string receiveLocation,
            string receiveFileName)
        {
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");
            if (string.IsNullOrEmpty(receiveLocation)) throw new ArgumentNullException("receiveLocation");
            if (string.IsNullOrEmpty(receiveFileName)) throw new ArgumentNullException("receiveFileName");

            var template = new XmlTemplate();
            template.LoadFromResource(GetType().Assembly, resourceName);
            template.Execute();
            Action action =
                () => File.WriteAllText(Path.Combine(receiveLocation, receiveFileName), template.InstanceAsXmlString());
            return action;
        }

        /// <summary>
        ///     Invokes an integration by dropping an embedded resource file in a BizTalk Receive Location and then scans the Event
        ///     Log for certain events.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="resourceName">The name of the embedded resource to write out.</param>
        /// <param name="receiveLocation">The path to the BizTalk Receive Location.</param>
        /// <param name="receiveFileName">The name of the file to write to the BizTalk Receive Location.</param>
        /// <param name="preConditionAction">An <see cref="Action" /> to be executed before kicking off the integration.</param>
        /// <param name="eventType">A particular <see cref="EventLogEntryType" /> to filter the scanning for.</param>
        /// <param name="eventMessage">An optional event message to filter the scanning for.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        protected void ExpectIntegrationEvents(int finalEventId,
            string resourceName,
            string receiveLocation,
            string receiveFileName,
            Action preConditionAction,
            EventLogEntryType eventType,
            string eventMessage = null,
            params BizTalkSendStop[] sendStops)
        {
            Action action = CreateWriteResourceFileAction(resourceName, receiveLocation, receiveFileName);
            ExpectIntegrationEvents(finalEventId,
                eventType,
                action,
                eventMessage,
                preConditionAction,
                sendStops);
        }

        /// <summary>
        ///     Invokes an integration and redirects calls to downstream dependencies to mocked services using
        ///     <see cref="BizTalkSendStop" />.
        /// </summary>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="preConditionAction">An optional <see cref="Action" /> to be executed before kicking off the integration.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        protected void ExpectIntegrationSuccess(Action actionToInvokeIntegration,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectIntegrationEvents(Enumerable.Empty<EventInfo>().ToArray(),
                actionToInvokeIntegration,
                preConditionAction,
                sendStops);
        }

        /// <summary>
        ///     Invokes an integration and then scans the system Event Log for any instances of a collection of
        ///     <see cref="EventInfo" />s.
        /// </summary>
        /// <param name="eventInfos">An array of <see cref="EventInfo" />s to scan the event log for.</param>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="preConditionAction">An optional <see cref="Action" /> to be executed before kicking off the integration.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        protected void ExpectIntegrationEvents(EventInfo[] eventInfos,
            Action actionToInvokeIntegration,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
         {
            if (actionToInvokeIntegration == null) throw new ArgumentNullException("actionToInvokeIntegration");
            if (sendStops == null) throw new ArgumentNullException("sendStops");
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120))) // The Moxy Server will timeout after 120 seconds            
            {
                try
                {
                    // Tell each of the Send Stops to configure themselves.
                    TestContext.WriteLine("Test: Performing Setup on sendstops");
                    foreach (BizTalkSendStop sendStop in sendStops)
                    {
                        TestContext.WriteLine(String.Format("Test: Setup: sendport:{0} | soapaction:{1}", sendStop.SendPortName, sendStop.SoapAction));
                        sendStop.Setup(BizTalkManager, MoxyUri);
                    }

                    TestContext.WriteLine(String.Format("MOXY: Create Moxy Server, Uri={0}", MoxyUri.AbsoluteUri));
                    var server = new MoxyServer(MoxyUri.Port, sendStops, TestContext);

                    EventLogMonitor.StartCapture();

                    if (preConditionAction != null) preConditionAction();

                    Task invokingTask = Task.Run(actionToInvokeIntegration, cts.Token);
                    Task<bool> serverTask = server.Run(cts.Token);

                    Task.WaitAll(invokingTask, serverTask);

                    // ugh
                    Thread.Sleep(Convert.ToInt32(DefaultRunEventsWaitSecs * 1000));
                    
                    var allEventsFound = EventLogMonitor.AreAllEventsObserved(eventInfos);
                    MoxyEventWrapupReport(eventInfos, allEventsFound);
                    if (!allEventsFound)
                    {
                        Assert.Fail("Test: ** Error **, all provided events were not observed. **");
                    }
                    
                    Assert.IsTrue(serverTask.Result, GenerateMoxyStopStatusMessage(server));
                }
                catch (Exception exception)
                {
                    Assert.Fail("Error caught: {0}", exception);
                }
                finally
                {
                    EventLogHelper.WriteEventLogEntries();
                    TestContext.WriteLine("Test: Performing Cleanup on sendstops");
                    foreach (BizTalkSendStop sendStop in sendStops)
                    {
                        TestContext.WriteLine(String.Format("Test: Clean-up: sendport:{0} | soapaction:{1}", sendStop.SendPortName, sendStop.SoapAction)); 
                        sendStop.Cleanup(BizTalkManager);
                    }
                }
            }
        }

        /// Invokes an integration and then scans the system Event Log for any instances of a collection of
        ///     <see cref="EventInfo" />s.
        /// </summary>
        /// <param name="eventInfos">An array of <see cref="EventInfo" />s to scan the event log for.</param>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="eventInfos"></param>
        /// <param name="actionToInvokeIntegration"></param>
        /// <param name="preConditionAction"></param>
        protected void ExpectIntegrationEvents (EventInfo[ ] eventInfos,
            Action actionToInvokeIntegration,
            Action preConditionAction = null)
        {
              if ( actionToInvokeIntegration == null )
                    throw new ArgumentNullException( "actionToInvokeIntegration" );
              using ( var cts = new CancellationTokenSource( TimeSpan.FromSeconds( 90 ) ) ) // timeout after 60 seconds            
              {
                    try
                    {
                        if ( preConditionAction != null ) 
                            preConditionAction();

                        EventLogMonitor.StartCapture();

                        Task invokingTask = Task.Run( actionToInvokeIntegration, cts.Token );
                        Task.WaitAll( invokingTask );

                        // ugh
                        Thread.Sleep(Convert.ToInt32(DefaultRunEventsWaitSecs * 1000));
                        
                        var allEventsFound = EventLogMonitor.AreAllEventsObserved(eventInfos);
                        MoxyEventWrapupReport(eventInfos, allEventsFound);
                        if (!allEventsFound)
                        {
                            Assert.Fail("Test: ** Error **, all provided events were not observed. **");
                        }                 
                    }
                    catch ( Exception exception )
                    {
                          Assert.Fail( "Error caught: {0}", exception );
                    }
              }
        }

        /// <summary>
        /// ExpectIntegrationEvents
        /// </summary>
        /// <param name="eventInfos"></param>
        /// <param name="actionToInvokeIntegration"></param>
        /// <param name="fileAdapterSendPort"></param>
        protected void ExpectIntegrationEvents(EventInfo[] eventInfos,
            Action actionToInvokeIntegration,
            string fileAdapterSendPort)
        {
            if (string.IsNullOrEmpty(fileAdapterSendPort))
                throw new ArgumentNullException("fileAdapterSendPort name is empty.");
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90))) // timeout after 60 seconds            
            {

                try
                {
                    OriginalSendPortUri = BizTalkManager.GetSendPortPrimaryTransportAddress(fileAdapterSendPort);
                    BizTalkManager.ModifySendPortPrimaryTransportAddress(fileAdapterSendPort, @"C:\DummyUriForFileSendPortFailedRoutingTesting\Dummy.txt");

                    EventLogMonitor.StartCapture();

                    Task invokingTask = Task.Run(actionToInvokeIntegration, cts.Token);
                    Task.WaitAll(invokingTask);

                    // ugh
                    Thread.Sleep(Convert.ToInt32(DefaultRunEventsWaitSecs * 1000));

                    var allEventsFound = EventLogMonitor.AreAllEventsObserved(eventInfos);
                    MoxyEventWrapupReport(eventInfos, allEventsFound);
                    if (!allEventsFound)
                    {
                        Assert.Fail("Test: ** Error **, all provided events were not observed. **");
                    }
                }
                catch (Exception exception)
                {
                    Assert.Fail("Error caught: {0}", exception);
                }
                finally
                {
                    if (BizTalkManager != null && OriginalSendPortUri != "")
                    {
                        BizTalkManager.ModifySendPortPrimaryTransportAddress(fileAdapterSendPort, OriginalSendPortUri);

                    }

                }

            }
        }

        /// <summary>
        /// CleanupServiceInstances
        /// </summary>
        private void CleanupServiceInstances()
        {
            try
            {
                IEnumerable messages = BizTalkOperations.GetMessages();
                foreach (BizTalkMessage msg in messages)
                {
                    if (msg.InstanceStatus == InstanceStatus.Suspended)
                    {
                        BizTalkOperations.TerminateInstance(msg.InstanceID);
                    }
                }
            } 
            catch (Exception) // ignore exception, the cleanup is desirable but not fatal if fail
            {
                TestContext.WriteLine("Test: Unexpected non-fatal Exception caught while cleaning up service instances, continuing...");
            }
        }


        /// <summary>
        /// MoxyWrapupReport
        /// </summary>
        private void MoxyEventWrapupReport(EventInfo[] eventInfos, bool result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("==================== Wrap-up ===============================");
            sb.AppendLine("Test: " + ExpectedEventsFormatted(eventInfos));
            sb.AppendLine("Test: " + EventLogMonitor.ObservedEventsFormatted());
            sb.AppendLine("Test: " + EventLogMonitor.UnobservedEventsFormatted());
            sb.Append("\r\n");
            if (result)
            {
                sb.AppendLine("Test: Events: ** Success **, all provided events observed. **");
            }
            else
            {
                sb.AppendLine("Test: Events:  ** Error **, all provided events were not observed. **");
            }
            sb.Append("\r\n");
            sb.AppendLine("============================================================");
            Trace.WriteLine(sb.ToString());
        }


        /// <summary>
        /// ExpectedEventsFormatted
        /// </summary>
        /// <param name="eventInfos"></param>
        /// <returns></returns>
        private string ExpectedEventsFormatted(EventInfo[] eventInfos)
        {
                StringBuilder sb = new StringBuilder();
                sb.Append("Expected Events -");
                sb.Append("\r\n");
                foreach (EventInfo entry in eventInfos)
                {
                    
                    sb.Append(String.Format(@"   Count={0}, EventID={1}, Source={2}, Type={3}, Message={4}",
                        entry.EventCount, entry.EventId, entry.EventSource,
                        entry.EventType, entry.EventMessage));
                    sb.Append("\r\n");
                }
                return sb.ToString();
        }


        /// <summary>
        /// GenerateMoxyStopStatusMessage
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        private string GenerateMoxyStopStatusMessage(MoxyServer server)
        {
            var sb = new StringBuilder("Test: Status: UNSUCCESSFUL");
            sb.AppendFormat("=== Send Stop Statuses ==={0}", Environment.NewLine);

            foreach (var stop in server.BizTalkStops)
            {
                sb.AppendFormat("  * Port: {0} Type: {1} Was Hit: {2}{3}",
                    stop.SendPortName,
                    stop.GetType(),
                    stop.RequestCaught,
                    Environment.NewLine);
            }

            return sb.ToString();
        }


        /// <summary>
        ///     Invokes an integration and then scans the system Event Log for a certain event ID and type.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="eventType">A particular <see cref="EventLogEntryType" /> to filter the scanning for.</param>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="eventMessage">An optional event message to filter the scanning for.</param>
        /// <param name="preConditionAction">An optional <see cref="Action" /> to be executed before kicking off the integration.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        protected void ExpectIntegrationEvents(int finalEventId,
            EventLogEntryType eventType,
            Action actionToInvokeIntegration,
            string eventMessage = null,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            if (actionToInvokeIntegration == null) throw new ArgumentNullException("actionToInvokeIntegration");
            var eventInfo = new EventInfo
            {
                EventId = finalEventId,
                EventCount = EventCount,
                EventMessage = eventMessage,
                EventSource = EventSource,
                EventType = eventType
            };
            ExpectIntegrationEvents(new[] {eventInfo},
                actionToInvokeIntegration,
                preConditionAction,
                sendStops);
        }


        /// <summary>
        ///     Invokes an integration by dropping an embedded resource into a file recieve location and then scans the system
        ///     Event Log for a particular error event ID.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="resourceName">The name of an embedded resource to load and write to a file.</param>
        /// <param name="receiveLocation">The path to receive location where to drop the embedded resource file.</param>
        /// <param name="receiveFileName">The name of the file to drop the embedded resource as.</param>
        /// <param name="errorMessage">An optional error message to filter the scanning of the event log.</param>
        /// <param name="preConditionAction">An optional pre-condition to execute before dropping the embedded resource.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        ///
        [Obsolete("ExpectFailure is deprecated, please use an Events[] based method instead.")]
        protected void ExpectFailure(int finalEventId,
            string resourceName,
            string receiveLocation,
            string receiveFileName,
            string errorMessage = null,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectIntegrationEvents(finalEventId,
                resourceName,
                receiveLocation,
                receiveFileName,
                preConditionAction,
                EventLogEntryType.Error,
                errorMessage,
                sendStops);
        }


        /// <summary>
        ///     should deprecate this in favor of ExpectFailure
        /// </summary>
        /// <param name="finalEventId"></param>
        /// <param name="resourceName"></param>
        /// <param name="receiveLocation"></param>
        /// <param name="receiveFileName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="preConditionAction"></param>
        /// <param name="sendStops"></param>
        [Obsolete("ExpectIntegrationEvents is deprecated, please use an Events[] based method instead.")]
        protected void ExpectIntegrationEvents(int finalEventId,
            string resourceName,
            string receiveLocation,
            string receiveFileName,
            string errorMessage = null,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectFailure(finalEventId,
                resourceName,
                receiveLocation,
                receiveFileName,
                errorMessage,
                preConditionAction,
                sendStops);
        }


        /// <summary>
        ///     Invokes an integration by executing some <see cref="Action" /> and then scans the system Event Log for a certain
        ///     error event.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="errorMessage">An optional error message to filter the scanning of the event log.</param>
        /// <param name="preConditionAction">An optional pre-condition to execute before dropping the embedded resource.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        [Obsolete("ExpectIntegrationEvents is deprecated, please use an Events[] based method instead.")]
        protected void ExpectIntegrationEvents(int finalEventId,
            Action actionToInvokeIntegration,
            string errorMessage = null,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectIntegrationEvents(finalEventId,
                EventLogEntryType.Error,
                actionToInvokeIntegration,
                errorMessage,
                preConditionAction,
                sendStops);
        }

        /// <summary>
        ///     Invokes an integration by writing out an Embedded Resource to a file and then scans the system Event Log for any
        ///     instances of a collection of
        ///     <see cref="EventInfo" />s.
        /// </summary>
        /// <param name="eventInfos">An array of <see cref="EventInfo" />s to scan the event log for.</param>
        /// <param name="resourceName">The name of an embedded resource to load and write to a file.</param>
        /// <param name="receiveLocation">The path to receive location where to drop the embedded resource file.</param>
        /// <param name="receiveFileName">The name of the file to drop the embedded resource as.</param>
        /// <param name="preConditionAction">An optional pre-condition to execute before dropping the embedded resource.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        protected void ExpectIntegrationEvents(EventInfo[] eventInfos,
            string resourceName,
            string receiveLocation,
            string receiveFileName,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            Action action = CreateWriteResourceFileAction(resourceName, receiveLocation, receiveFileName);
            ExpectIntegrationEvents(eventInfos, action, preConditionAction, sendStops);
        }


        /// <summary>
        ///     Invokes an integration by writing out an Embedded Resource to a file and then scans the system Event Log for a
        ///     specifc instance of an Information message that denotes integration success.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="resourceName">The name of an embedded resource to load and write to a file.</param>
        /// <param name="receiveLocation">The path to receive location where to drop the embedded resource file.</param>
        /// <param name="receiveFileName">The name of the file to drop the embedded resource as.</param>
        /// <param name="informationMessage">
        ///     An optional string to compare the informational message with when scanning the Event
        ///     Log.
        /// </param>
        /// <param name="preConditionAction">An optional pre-condition to execute before dropping the embedded resource.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        [Obsolete("ExpectIntegrationSuccess is deprecated, please use an Events[] based method instead.")]
        protected void ExpectIntegrationSuccess(int finalEventId,
            string resourceName,
            string receiveLocation,
            string receiveFileName,
            string informationMessage = null,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectIntegrationEvents(finalEventId,
                resourceName,
                receiveLocation,
                receiveFileName,
                preConditionAction,
                EventLogEntryType.Information,
                informationMessage,
                sendStops);
        }


        /// <summary>
        ///     Invokes an integration with a specific <see cref="Action" /> and then scans the Event Log for a certain
        ///     Informational message to denote success.
        /// </summary>
        /// <param name="finalEventId">The event ID to scan the Event Log for.</param>
        /// <param name="informationMessage">
        ///     An optional string to compare the informational message with when scanning the Event
        ///     Log.
        /// </param>
        /// <param name="actionToInvokeIntegration">
        ///     An <see cref="Action" /> that kicks off the integration by sending a message to
        ///     the bus.
        /// </param>
        /// <param name="preConditionAction">An optional pre-condition to execute before dropping the embedded resource.</param>
        /// <param name="sendStops">A collection of <see cref="BizTalkSendStop" /> to respond to requests during this integration.</param>
        [Obsolete("ExpectIntegrationSuccess is deprecated, please use an Events[] based method instead.")]
        protected void ExpectIntegrationSuccess(int finalEventId,
            string informationMessage,
            Action actionToInvokeIntegration,
            Action preConditionAction = null,
            params BizTalkSendStop[] sendStops)
        {
            ExpectIntegrationEvents(finalEventId,
                EventLogEntryType.Information,
                actionToInvokeIntegration,
                informationMessage,
                preConditionAction,
                sendStops);
        }

        /// <summary>
        ///     Invokes an integration by writing an Embedded Resource to a file in a specific BizTalk Receive Location and expects
        ///     to receive a Failed Routing Event in the Event Log.
        /// </summary>
        /// <param name="sendPortName">The name of the BizTalk Send Port to throw the Failed Routing Event.</param>
        /// <param name="eventId">The Event ID of the Failed Routing event to scan the Event Log for.</param>
        /// <param name="resourceName">The name of an embedded resource to load and write to a file.</param>
        /// <param name="receiveLocation">The path to receive location where to drop the embedded resource file.</param>
        /// <param name="receiveFileName">The name of the file to drop the embedded resource as.</param>
        protected void CheckSendPortFailedRouting(string sendPortName,
            int eventId,
            string resourceName,
            string receiveLocation,
            string receiveFileName)
        {
            var eventInfo = new EventInfo
            {
                EventCount = EventCount,
                EventId = eventId,
                MaxWaitTime = MaxEventWaitTimeSeconds,
                EventMessage = null,
                EventSource = EventSource,
                EventType = EventLogEntryType.Error
            };
            var bss = new FaultingSendStop(sendPortName, null);
            ExpectIntegrationEvents(new[] {eventInfo},
                resourceName,
                receiveLocation,
                receiveFileName,
                sendStops: new BizTalkSendStop[] {bss});
        }


        /// <summary>
        ///     Return content param, soap 1.1 encapsulated
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string GetSoap11Encapsulated(string content)
        {
            return string.Format(Resource.soap11Envelope, content);
        }

        /// <summary>
        ///     Create generic Soap 1.1 fault based on params
        /// </summary>
        /// <param name="faultCode"></param>
        /// <param name="faultString"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        protected string CreateGenericSoap11Fault(string faultCode, string faultString, string detail = null)
        {
            return string.Format(Resource.soap11GenericFault, faultCode, faultString, detail ?? "");
        }


        /// <summary>
        ///     Return content, action params, Soap 1.2 encapsulated
        /// </summary>
        /// <param name="content"></param>
        /// <param name="action"></param>
        /// <param name="destServiceUrl"></param>
        /// <returns></returns>
        protected string GetSoap12Encapsulated(string content, string action, string destServiceUrl = "http://localhost")
        {
            string guid = Guid.NewGuid().ToString();
            return string.Format(Resource.soap12Envelope, action, guid, destServiceUrl ?? "", content);
        }


        /// <summary>
        ///     Create generic Soap 1.2 fault based on params
        /// </summary>
        /// <param name="faultCode"></param>
        /// <param name="faultSubCode"></param>
        /// <param name="reason"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        protected string CreateGenericSoap12Fault(string faultCode,
            string faultSubCode,
            string reason = null,
            string detail = null)
        {
            return string.Format(Resource.soap12GenericFault, faultCode, faultSubCode, reason ?? "", detail ?? "");
        }


        /// <summary>
        /// TestInitialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            MaxEventWaitTimeSeconds = 30;
            EventCount = 1;
            

            int port = GetFreePortInRange(CurrentPort, MaxPort);
            MoxyUri = new Uri(String.Format("http://localhost:{0}/", port));
            CurrentPort++;

            CleanupServiceInstances();


       }

        
        /// <summary>
        /// TestCleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            CleanupServiceInstances();
        }

        
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
        }


        [ClassCleanup()]
        public static void ClassCleanup()
        {
            
        }
         



        /// <summary>
        /// GetFreePortInRange
        /// </summary>
        /// <param name="PortStartIndex"></param>
        /// <param name="PortEndIndex"></param>
        /// <returns></returns>
        private int GetFreePortInRange(int PortStartIndex, int PortEndIndex)
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

                IPEndPoint[] tcpEndPoints = ipGlobalProperties.GetActiveTcpListeners();
                List<int> usedServerTCpPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();

                IPEndPoint[] udpEndPoints = ipGlobalProperties.GetActiveUdpListeners();
                List<int> usedServerUdpPorts = udpEndPoints.Select(p => p.Port).ToList<int>();

                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                List<int> usedPorts = tcpConnInfoArray.Where(p => p.State != TcpState.Closed).Select(p => p.LocalEndPoint.Port).ToList<int>();

                usedPorts.AddRange(usedServerTCpPorts.ToArray());
                usedPorts.AddRange(usedServerUdpPorts.ToArray());

                int unusedPort = 0;

                for (int port = PortStartIndex; port < PortEndIndex; port++)
                {
                    if (!usedPorts.Contains(port))
                    {
                        unusedPort = port;
                        break;
                    }
                }

                if (unusedPort == 0)
                {
                    throw new Exception("GetFreePortInRange, Out of ports");
                }

                return unusedPort;
            }
            catch (Exception ex)
            {

                string errorMessage = ex.Message;
                throw;
            }
        }
    }
}