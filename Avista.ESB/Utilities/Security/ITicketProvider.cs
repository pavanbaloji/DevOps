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

namespace Avista.ESB.Utilities.Security
{
    public interface ITicketProvider : IComponent
    {
        string IssueTicket();
        string IssueTicket(int flags);
        string[] RedeemTicket(string applicationName, string sender, string ticket, int flags, out string externalUserName);
    }
}
