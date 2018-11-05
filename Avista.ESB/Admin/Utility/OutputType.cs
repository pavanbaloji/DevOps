﻿//-----------------------------------------------------------------------------
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

namespace Avista.ESB.Admin.Utility
{
    /// <summary>
    /// The types of output that can be sent to the <see cref="ColorConsole"/>.
    /// </summary>
    public enum OutputType
    {
        Unknown,
        Trace,
        Info,
        Warning,
        Error
    }
}
