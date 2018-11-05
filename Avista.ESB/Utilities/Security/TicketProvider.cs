//-----------------------------------------------------------------------------
// This file is part of the HP.Practices Application Framework
//
// Copyright (c) HP Enterprise Services. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//-----------------------------------------------------------------------------
using System;
using Avista.ESB.Utilities.Components;
using Avista.ESB.Utilities.Configuration;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.Security.Configuration;

namespace Avista.ESB.Utilities.Security
{
    /// <summary>
    /// Provides a factory and manages a singleton instance of the ITicketProvider interface which is used
    /// obtain single-sign-on tickets.
    /// </summary>
    public class TicketProvider
    {
        /// <summary>
        /// TicketProvider instance.
        /// </summary>
        private static ITicketProvider ticketProviderInstance = null;

        /// <summary>
        /// TicketProvider instance lock.
        /// </summary>
        private static object ticketProviderLock = new Object();

        /// <summary>
        /// Creates an ITicketProvider implementation. The instance is a singleton.
        /// </summary>
        /// <returns>A reference to the ITicketProvider implementation.</returns>
        public static ITicketProvider GetTicketProvider()
        {
            string instanceName = "";
            string className = "";
            string assemblyName = "";
            ITicketProvider ticketProvider = null;
            try
            {
                lock (ticketProviderLock)
                {
                    if (ticketProviderInstance == null)
                    {
                        SecuritySection section = SecuritySection.GetSection();
                        ClassSpecificationElement spec = section.TicketProvider;
                        instanceName = spec.Name;
                        className = spec.Class;
                        assemblyName = spec.Assembly;
                        ticketProviderInstance = (ITicketProvider)Factory.CreateComponent(instanceName, className, assemblyName);
                    }
                    ticketProvider = ticketProviderInstance;
                }
            }
            catch (Exception exception)
            {
               
                throw new Exception("Failed to create ITicketProvider implementation.", exception);
            }
            return ticketProvider;
        }

        /// <summary>
        /// Issues a ticket using the singleton TicketProvider instance.
        /// </summary>
        /// <returns>The ticket issued by the provider.</returns>
        public static string IssueTicket()
        {
            ITicketProvider ticketProvider = GetTicketProvider();
            string ticket = ticketProvider.IssueTicket();
            return ticket;
        }

        /// <summary>
        /// Issues a ticket with flags using the singleton TicketProvider instance.
        /// </summary>
        /// <returns>The ticket issued by the provider.</returns>
        public static string IssueTicket(int flags)
        {
            ITicketProvider ticketProvider = GetTicketProvider();
            string ticket = ticketProvider.IssueTicket(flags);
            return ticket;
        }
    }
}
