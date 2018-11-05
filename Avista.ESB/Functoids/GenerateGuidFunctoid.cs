using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.BaseFunctoids;
using System.Reflection;
namespace Avista.ESB.Functoids   
{
     public class GenerateGuidFunctoid : BaseFunctoid
     {  /// <summary>
           /// Default constructor
           /// </summary>
           public GenerateGuidFunctoid ()
                 : base()
           {
                 this.ID = 10009;

                 SetupResourceAssembly( "Avista.ESB.Functoids.Resources", Assembly.GetExecutingAssembly() );
                 SetName( "GENERATEGUID_NAME" );
                 SetTooltip( "GENERATEGUID_TOOLTIP" );
                 SetDescription( "GENERATEGUID_DESCRIPTION" );
                 SetBitmap( "GENERATEGUID_BITMAP" );

                 this.Category = FunctoidCategory.String;
                 this.SetMinParams( 0 );
                 this.SetMaxParams( 0 );

                 //AddInputConnectionType(ConnectionType.AllExceptRecord);
                 this.OutputConnectionType = ConnectionType.AllExceptRecord;

                 SetExternalFunctionName( GetType().Assembly.FullName, "Avista.ESB.Functoids.GenerateGuidFunctoid", "GenerateGuid" );
           }

           /// <summary>
           /// Return new GUID
           /// </summary>
           /// <returns>output value as xsd:String</returns>
           public string GenerateGuid ()
           {
                 return Guid.NewGuid().ToString();
           }
     }
}
