﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Avista.ESB.Functoids {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Avista.ESB.Functoids.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to convert a string value to xsd:Boolean value.
        ///
        ///Arguments:
        ///	input[0] : Source value..
        /// </summary>
        internal static string CONVERTTOBOOLEAN_DESCRIPTION {
            get {
                return ResourceManager.GetString("CONVERTTOBOOLEAN_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ConvertToBoolean Functoid.
        /// </summary>
        internal static string CONVERTTOBOOLEAN_NAME {
            get {
                return ResourceManager.GetString("CONVERTTOBOOLEAN_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Convert the input value to xsd:Boolean value.
        /// </summary>
        internal static string CONVERTTOBOOLEAN_TOOLTIP {
            get {
                return ResourceManager.GetString("CONVERTTOBOOLEAN_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to execute a parameterized SQL statement.
        ///
        ///Arguments:
        ///	input[0] : format string for the sql statement.
        ///	input[1] : Connection string identifier name.
        ///	input[2] : parameter {0} in the format string.
        ///	...
        ///	input[12]: parameter {10} in the format string..
        /// </summary>
        internal static string EXECUTESQL_DESCRIPTION {
            get {
                return ResourceManager.GetString("EXECUTESQL_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ExecuteSql Functoid.
        /// </summary>
        internal static string EXECUTESQL_NAME {
            get {
                return ResourceManager.GetString("EXECUTESQL_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to execute a parameterized SQL statement..
        /// </summary>
        internal static string EXECUTESQL_TOOLTIP {
            get {
                return ResourceManager.GetString("EXECUTESQL_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to read a given value as date and produce an output date value in a provided format.
        ///
        ///Arguments:
        ///	input[0] : Value of source Date to be formatted.
        ///	input[1] : String used to specify the Date string format to read the input value. [ex: YYYY-mm-dd]
        ///	input[2] : String used to specify the Date string format to produce output value. [ex: YYYY-mm-dd]
        ///	input[3] : Label to describe what this is formatting.  Used to debug which instance of the functoid failed..
        /// </summary>
        internal static string FORMATDATETIME_DESCRIPTION {
            get {
                return ResourceManager.GetString("FORMATDATETIME_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FormatDateTime.
        /// </summary>
        internal static string FORMATDATETIME_NAME {
            get {
                return ResourceManager.GetString("FORMATDATETIME_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to convert input value to a DateValue by providing input and output date formats..
        /// </summary>
        internal static string FORMATDATETIME_TOOLTIP {
            get {
                return ResourceManager.GetString("FORMATDATETIME_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to return a newly generated GUID.
        ///
        ///Arguments:
        ///none.
        /// </summary>
        internal static string GENERATEGUID_DESCRIPTION {
            get {
                return ResourceManager.GetString("GENERATEGUID_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generate GUID Functoid.
        /// </summary>
        internal static string GENERATEGUID_NAME {
            get {
                return ResourceManager.GetString("GENERATEGUID_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns a newly generated GUID.
        /// </summary>
        internal static string GENERATEGUID_TOOLTIP {
            get {
                return ResourceManager.GetString("GENERATEGUID_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to lookup value from a Database table.
        ///
        ///Arguments:
        ///	input[0] : Column name of value to be returned.
        ///	input[1] : Table schema name.
        ///	input[2] : Table name.
        ///	input[3] : Filter column name.
        ///	input[4] : Filter column value.
        ///	input[5] : Connection string identifier name..
        /// </summary>
        internal static string LOOKUPDBVALUE_DESCRIPTION {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUE_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LookupDbValue Functoid.
        /// </summary>
        internal static string LOOKUPDBVALUE_NAME {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUE_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be use to lookup a value from a Database table..
        /// </summary>
        internal static string LOOKUPDBVALUE_TOOLTIP {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUE_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functiod can be used to join two tables and lookup value from a database.
        ///
        ///Arguments:
        ///
        ///	input[0] : Column name of value to be returned
        ///	input[1] : Schema name for the first table in a join query.
        ///	input[2] : First table name in the join query.
        ///	input[3] : First filter column name.
        ///	input[4] : First Filter column value
        ///	input[5] : Schemas name for the second table. 
        ///	input[6] : Second table name in the join query.
        ///	input[7] : Second filter column name.
        ///	input[8] : Second Filter column value
        /// </summary>
        internal static string LOOKUPDBVALUEUSINGJOIN_DESCRIPTION {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEUSINGJOIN_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LookUpDbValueUsingJoin Functiod.
        /// </summary>
        internal static string LOOKUPDBVALUEUSINGJOIN_NAME {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEUSINGJOIN_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to join two table to lookup value from a database..
        /// </summary>
        internal static string LOOKUPDBVALUEUSINGJOIN_TOOLTIP {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEUSINGJOIN_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to lookup value from a Database table.
        ///
        ///Arguments:
        ///	input[0] : Column name of value to be returned.
        ///	input[1] : Table schema name.
        ///	input[2] : Table name.
        ///	input[3] : Filter column name.
        ///	input[4] : Filter column value.
        ///	input[5] : Connection string identifier name..
        /// </summary>
        internal static string LOOKUPDBVALUEWITHLIKEOPERATOR_DESCRIPTION {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEWITHLIKEOPERATOR_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LookUpDbValueWithLikeOperator Functoid.
        /// </summary>
        internal static string LOOKUPDBVALUEWITHLIKEOPERATOR_NAME {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEWITHLIKEOPERATOR_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be use to lookup a value from a Database table using LIKE operator..
        /// </summary>
        internal static string LOOKUPDBVALUEWITHLIKEOPERATOR_TOOLTIP {
            get {
                return ResourceManager.GetString("LOOKUPDBVALUEWITHLIKEOPERATOR_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to map as functoid output, the static value set in the params.
        ///
        ///Arguments:
        ///	input[0] : Value to be returned..
        /// </summary>
        internal static string STATICVALUE_DESCRIPTION {
            get {
                return ResourceManager.GetString("STATICVALUE_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Map Static Value Functoid.
        /// </summary>
        internal static string STATICVALUE_NAME {
            get {
                return ResourceManager.GetString("STATICVALUE_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maps as functoid output, the static value set in params.
        /// </summary>
        internal static string STATICVALUE_TOOLTIP {
            get {
                return ResourceManager.GetString("STATICVALUE_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This functoid can be used to truncate a string to a maximum length.  This will only happen if the string currently exceeds the maximum lenght provided.
        ///
        ///Arguments:
        ///	input[0] : Source value.
        ///	input[1] : Maximimum Length..
        /// </summary>
        internal static string TRUNCATESTRING_DESCRIPTION {
            get {
                return ResourceManager.GetString("TRUNCATESTRING_DESCRIPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TruncateString Functoid.
        /// </summary>
        internal static string TRUNCATESTRING_NAME {
            get {
                return ResourceManager.GetString("TRUNCATESTRING_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Truncates a string if necessary to achieve the maximum length allowed..
        /// </summary>
        internal static string TRUNCATESTRING_TOOLTIP {
            get {
                return ResourceManager.GetString("TRUNCATESTRING_TOOLTIP", resourceCulture);
            }
        }
    }
}