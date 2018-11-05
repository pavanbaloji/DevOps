using System;
using System.Collections.Generic;
using System.Threading;
using Avista.ESB.Utilities.Logging;


namespace Avista.ESB.Testing.Components
{
      public class RecursionMonitor
      {
            /// <summary>
            /// A dictionary list used to keep track of the depth of nested calls encountered for each thread.
            /// The index of the list is the thread id and the value associated will be the number of nested calls encountered.
            /// </summary>
            private static Dictionary<Int32, Int32> depthCounterList = new Dictionary<int, int>();

            /// <summary>
            /// An object used for locking access to the depthCounterList.
            /// </summary>
            private static object depthCounterListLock = new object();

            /// <summary>
            /// The maximum depth to which recursion is allowed.
            /// </summary>
            private int maxDepth;

            /// <summary>
            /// Constructs a RecursionMonitor which can be used for monitoring the call stack for recursion beyond a specified depth.
            /// A RecursionMonitor is normally defined and constructed as a private static variable within a class conatining a method that needs
            /// to be checked for unwanted (deep) recursion.
            /// </summary>
            public RecursionMonitor (int maxDepth)
            {
                  this.maxDepth = maxDepth;
            }

            /// <summary>
            /// Increments the depth counter and determines if the maximum depth has been exceeded.
            /// </summary>
            /// <returns>True if depth is still at or below the maximum depth. False if the maximum depth has been exceeded.</returns>
            public bool Increment ()
            {
                  bool depthOk = false;
                  try
                  {
                        int depthCounter = IncrementDepthCounter();
                        if ( depthCounter < maxDepth )
                        {
                              depthOk = true;
                        }
                  }
                  catch ( Exception )
                  {
                  }
                  return depthOk;
            }

            /// <summary>
            /// Decrement the depth counter.
            /// </summary>
            public void Decrement ()
            {
                  try
                  {
                        DecrementDepthCounter();
                  }
                  catch ( Exception )
                  {
                  }
            }

            /// <summary>
            /// Increments a depth counter used for monitoring the level of recursion within a thread.
            /// This allows the number of nested calls to be monitoring to prevent infinite loops.
            /// The method uses a lock to increment the counter in a thread safe manner.
            /// </summary>
            /// <returns>The count of nested calls for the current thread.</returns>
            private int IncrementDepthCounter ()
            {
                  int depthCounter = maxDepth + 1; // Default value in case we throw an exception trying to access the list.
                  try
                  {
                        int threadId = Thread.CurrentThread.ManagedThreadId;
                        // Lock the list
                        lock ( depthCounterList )
                        {
                              // Check to see if depth is already being counted for this thread.
                              if ( depthCounterList.ContainsKey( threadId ) )
                              {
                                    // If it is then increment the counter.
                                    depthCounter = depthCounterList[ threadId ];
                                    depthCounter++;
                                    depthCounterList[ threadId ] = depthCounter;
                              }
                              else
                              {
                                    // If it is not then initialize the counter to 1.
                                    depthCounter = 1;
                                    depthCounterList.Add( threadId, depthCounter );
                              }
                        }
                  }
                  catch ( Exception exception )
                  {
                        Logger.WriteError( "RecursionMonitor.IncrementDepthCounter() failed."+ exception );
                  }
                  // Return the depth counter.
                  return depthCounter;
            }

            /// <summary>
            /// Decrements a depth counter used for monitoring the level of recursion within a thread.
            /// This allows the number of nested calls to be monitoring to prevent infinite loops.
            /// The method uses a lock to increment the counter in a thread safe manner.
            /// </summary>
            /// <returns>The count of nested exceptions for the given thread after it was decremented.</returns>
            private int DecrementDepthCounter ()
            {
                  int depthCounter = maxDepth + 1; // Default value in case we throw an exception trying to access the list.
                  try
                  {
                        int threadId = Thread.CurrentThread.ManagedThreadId;
                        // Lock the list
                        lock ( depthCounterList )
                        {
                              // Decrement the counter and then store it back in the list.
                              if ( depthCounterList.ContainsKey( threadId ) )
                              {
                                    depthCounter = depthCounterList[ threadId ];
                                    depthCounter--;
                                    depthCounterList[ threadId ] = depthCounter;
                              }
                        }
                  }
                  catch ( Exception exception )
                  {
                        Logger.WriteError( "RecursionMonitor.DecrementDepthCounter() failed."+ exception );
                  }
                  // Return the depth counter.
                  return depthCounter;
            }
      }
}
