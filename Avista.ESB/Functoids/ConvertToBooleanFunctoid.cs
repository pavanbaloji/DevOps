using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.BaseFunctoids;
using System.Reflection;
namespace Avista.ESB.Functoids
{
      public class ConvertToBooleanFunctoid:BaseFunctoid
      {  /// <summary>
            /// Default constructor
            /// </summary>
            public ConvertToBooleanFunctoid ()
                  : base()
            {
                  this.ID = 10007;

                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "CONVERTTOBOOLEAN_NAME" );
                  SetTooltip( "CONVERTTOBOOLEAN_TOOLTIP" );
                  SetDescription( "CONVERTTOBOOLEAN_DESCRIPTION" );
                  SetBitmap( "CONVERTTOBOOLEAN_BITMAP" );

                  this.Category = FunctoidCategory.Logical;
                  this.SetMinParams( 1 );
                  this.SetMaxParams( 1 );

                  AddInputConnectionType( ConnectionType.AllExceptRecord );
                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.ConvertToBooleanFunctoid", "ConvertToBoolean" );
            }

            /// <summary>
            /// Convert the input value to xsd:Boolean value.
            /// </summary>
            /// <param name="inputValue">Input value as string</param>
            /// <returns>output value as xsd:Boolean</returns>
            public string ConvertToBoolean (string inputValue)
            {


                  if ( inputValue.Equals( "Y", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "true";
                  }
                  else if ( inputValue.Equals( "1", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "true";
                  }
                  else if ( inputValue.Equals( "Yes", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "true";
                  }
                  else if ( inputValue.Equals( "true", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "true";
                  }
                  else if ( inputValue.Equals( "n", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "false";
                  }
                  else if ( inputValue.Equals( "0", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "false";
                  }
                  else if ( inputValue.Equals( "No", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "false";
                  }
                  else if ( inputValue.Equals( "false", StringComparison.OrdinalIgnoreCase ) )
                  {
                        return "false";
                  }


                  return (!string.IsNullOrEmpty( inputValue )).ToString().ToLower();
            }

      }
}
