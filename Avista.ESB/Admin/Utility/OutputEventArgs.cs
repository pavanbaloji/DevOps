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

namespace Avista.ESB.Admin.Utility
{
    /// <summary>
    /// This class is used for the event argument that accompanies an event handled by <see cref="OutputEventHandler"/>.
    /// These events are generated buy the <see cref="ColorConsole"/> whenever output is written to the console.
    /// </summary>
    public class OutputEventArgs : EventArgs
    {
        /// <summary>
        /// The type of the output message.
        /// </summary>
        private OutputType _outputType = OutputType.Unknown;

        /// <summary>
        /// The message being output.
        /// </summary>
        private string _message = null;

        /// <summary>
        /// Default constructor. Constructs an OutputEventArgs object with unknown output type.
        /// </summary>
        public OutputEventArgs(OutputType outputType, string message)
        {
            _outputType = outputType;
            _message = message;
        }

        /// <summary>
        /// The type of the output message.
        /// </summary>
        public OutputType OutputType
        {
            get
            {
                return _outputType;
            }
        }

        /// <summary>
        /// The message being output.
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
