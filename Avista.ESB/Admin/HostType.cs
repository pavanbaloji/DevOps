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

namespace Avista.ESB.Admin
{
    public enum HostType
    {
        Invalid = 0,
        InProcess = 1,
        Isolated = 2
    }
    public enum BizTalkDehydrationBehavior
    {
          Always=0,
          Never=1,
          Custom = 2,
          Invalid=3,
    }
    public enum BizTalkServiceState
    {
          NotApplicable,
          Stopped,
          StartPending,
          StopPending,
          Running,
          ContinuePending,
          PausePending,
          Paused,
          Unknown,
    }
}
