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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.BizTalk.XLANGs.BTXEngine;

[assembly: AssemblyTitle("Avista.ESB.Pipelines")]
[assembly: AssemblyDescription("Avista.ESB.Pipelines")]
[assembly: AssemblyProduct("Avista.ESB.Pipelines")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Avista")]
[assembly: AssemblyCopyright("Copyright 2017")]
[assembly: AssemblyTrademark("")]

[assembly: Microsoft.XLANGs.BaseTypes.BizTalkAssemblyAttribute(typeof(BTXService))]
[assembly: ComVisible(false)]
[assembly: Guid("9f51501c-863c-41a0-b8d3-a83ed6eeb58c")]

[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]
