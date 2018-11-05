using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.BizTalk.BaseFunctoids;
using Avista.ESB.Utilities.Logging;
using Avista.ESB.Utilities.DataAccess;

namespace Avista.ESB.Functoids
{
      public class TruncateStringFunctoid:BaseFunctoid
      {
            /// <summary>
            /// Default constructor
            /// </summary>
            public TruncateStringFunctoid ()
                  : base()
            {
                  this.ID = 10008;

                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "TRUNCATESTRING_NAME" );
                  SetTooltip( "TRUNCATESTRING_TOOLTIP" );
                  SetDescription( "TRUNCATESTRING_DESCRIPTION" );
                  SetBitmap( "TRUNCATESTRING_BITMAP" );

                  this.Category = FunctoidCategory.String;
                  this.SetMinParams( 2 );
                  this.SetMaxParams( 2 );

                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.TruncateStringFunctoid", "TruncateString" );
            }

            /// <summary>
            /// Truncate the string if necessary
            /// </summary>
            /// <param name="inputString">Input value as string</param>
            /// <param name="maxLength">Maximum allowed string length</param>
            /// <returns>output value as xsd:String</returns>
            public string TruncateString (string inputString, int maxLength)
            {
                  if ( inputString.Length > maxLength )
                  {
                        return inputString.Substring( 0, maxLength );
                  }
                  return inputString;
            }
      }
}
