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
    public enum OrchStatus : uint
    {
        Unknown = 0,
        Unbound = 1, // When the orch is unbound WMI returns 2 (Bound) but the docs says it should return 1 (Unbound).
        Bound_And_Unenlisted = 2,
        Enlisted_And_Stopped = 3,
        Started = 4
    }
}
