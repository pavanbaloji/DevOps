using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avista.ESB.Utilities;
using Avista.ESB.Utilities.Logging;
using Microsoft.BizTalk.BaseFunctoids;

namespace Avista.ESB.Functoids
{
      public class FormatDateTimeFunctoid : BaseFunctoid
      {
            public FormatDateTimeFunctoid ()
                  : base()
            {
                  this.ID = 10006;
                  SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                  SetName( "FORMATDATETIME_NAME" );
                  SetTooltip( "FORMATDATETIME_TOOLTIP" );
                  SetDescription( "FORMATDATETIME_DESCRIPTION" );
                  SetBitmap( "FORMATDATETIME_BITMAP" );

                  this.Category = FunctoidCategory.DateTime;
                  this.SetMinParams( 4 );
                  this.SetMaxParams( 4 );
                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // sourceDate
                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // inputFormat
                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // outputFormat
                  AddInputConnectionType( ConnectionType.AllExceptRecord ); // label
                  
                  this.OutputConnectionType = ConnectionType.AllExceptRecord;

                  SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.FormatDateTimeFunctoid", "FormatDateTime" );
            }
            
            
            /// <summary>
            /// CustomFormatDateTime Custom Functoid
            /// </summary>
            /// <param name="sourceDate">Value of source Date to be formatted</param>
            /// <param name="inputFormat">String used to specify the format of the source date string [ex: YYYY-mm-dd]</param>
            /// <param name="outputFormat">String used to specify the Date string format [ex: YYYY-mm-dd]</param>
            /// <param name="label">String used to identify what date is being formatted.  Used for logging purposes only.</param>
            /// <returns>string </returns>
            public string FormatDateTime (string sourceDate, string inputFormat, string outputFormat, string label = "")
            {
                  DateTime localDate;
                  string returnDate = System.DateTime.MinValue.ToString();
                  if ( sourceDate.Length != 0 )
                  {
                        returnDate = sourceDate;
                  }

                  try
                  {
                        if ( inputFormat.Length == 0 )
                        {
                              localDate = DateTime.Parse( sourceDate );
                        }
                        else
                        {
                              localDate = DateTime.ParseExact( sourceDate, inputFormat, CultureInfo.CurrentCulture, DateTimeStyles.None );
                        }

                        if ( outputFormat.EndsWith( "Z" ) ) //Assumes you must convert to Utc before outputing the value
                        {
                              returnDate = localDate.ToUniversalTime().ToString( outputFormat ).ToString();
                        }
                        else
                        {
                              returnDate = localDate.ToString( outputFormat ).ToString();
                        }
                  }
                  catch ( Exception exception )
                  {

                        Logger.WriteError( "Error in FormatDateTime functoid. SourceDate = '" + sourceDate + "', Label = " + label + "\n" + exception.Message +"\n"+ exception.StackTrace.ToString());
                        throw;
                  }

                  return returnDate;
            }
      }
}
