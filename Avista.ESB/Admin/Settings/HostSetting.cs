namespace Microsoft.BizTalk.Management {
    using System;
    using System.ComponentModel;
    using System.Management;
    using System.Collections;
    using System.Globalization;
    
    
    // Functions ShouldSerialize<PropertyName> are functions used by VS property browser to check if a particular property has to be serialized. These functions are added for all ValueType properties ( properties of type Int32, BOOL etc.. which cannot be set to null). These functions use Is<PropertyName>Null function. These functions are also used in the TypeConverter implementation for the properties to check for NULL value of property so that an empty value can be shown in Property browser in case of Drag and Drop in Visual studio.
    // Functions Is<PropertyName>Null() are used to check if a property is NULL.
    // Functions Reset<PropertyName> are added for Nullable Read/Write properties. These functions are used by VS designer in property browser to set a property to NULL.
    // Every property added to the class for WMI property has attributes set to define its behavior in Visual Studio designer and also to define a TypeConverter to be used.
    // An Early Bound class generated for the WMI class.MSBTS_HostSetting
    public class HostSetting : System.ComponentModel.Component {
        
        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "root\\MicrosoftBizTalkServer";
        
        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "MSBTS_HostSetting";
        
        // Private member variable to hold the ManagementScope which is used by the various methods.
        private static System.Management.ManagementScope statMgmtScope = null;
        
        private ManagementSystemProperties PrivateSystemProperties;
        
        // Underlying lateBound WMI object.
        private System.Management.ManagementObject PrivateLateBoundObject;
        
        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;
        
        // Private variable to hold the embedded property representing the instance.
        private System.Management.ManagementBaseObject embeddedObj;
        
        // The current WMI object used
        private System.Management.ManagementBaseObject curObj;
        
        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;
        
        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public HostSetting() {
            this.InitializeObject(null, null, null);
        }
        
        public HostSetting(string keyMgmtDbNameOverride, string keyMgmtDbServerOverride, string keyName) {
            this.InitializeObject(null, new System.Management.ManagementPath(HostSetting.ConstructPath(keyMgmtDbNameOverride, keyMgmtDbServerOverride, keyName)), null);
        }
        
        public HostSetting(System.Management.ManagementScope mgmtScope, string keyMgmtDbNameOverride, string keyMgmtDbServerOverride, string keyName) {
            this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(HostSetting.ConstructPath(keyMgmtDbNameOverride, keyMgmtDbServerOverride, keyName)), null);
        }
        
        public HostSetting(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(null, path, getOptions);
        }
        
        public HostSetting(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path) {
            this.InitializeObject(mgmtScope, path, null);
        }
        
        public HostSetting(System.Management.ManagementPath path) {
            this.InitializeObject(null, path, null);
        }
        
        public HostSetting(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(mgmtScope, path, getOptions);
        }
        
        public HostSetting(System.Management.ManagementObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        public HostSetting(System.Management.ManagementBaseObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        // Property returns the namespace of the WMI class.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace {
            get {
                return "root\\MicrosoftBizTalkServer";
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName {
            get {
                string strRet = CreatedClassName;
                if ((curObj != null)) {
                    if ((curObj.ClassPath != null)) {
                        strRet = ((string)(curObj["__CLASS"]));
                        if (((strRet == null) 
                                    || (strRet == string.Empty))) {
                            strRet = CreatedClassName;
                        }
                    }
                }
                return strRet;
            }
        }
        
        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties {
            get {
                return PrivateSystemProperties;
            }
        }
        
        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementBaseObject LateBoundObject {
            get {
                return curObj;
            }
        }
        
        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementScope Scope {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Scope;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }
        
        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit {
            get {
                return AutoCommitProp;
            }
            set {
                AutoCommitProp = value;
            }
        }
        
        // The ManagementPath of the underlying WMI object.
        [Browsable(true)]
        public System.Management.ManagementPath Path {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Path;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    if ((CheckIfProperClass(null, value, null) != true)) {
                        throw new System.ArgumentException("Class name does not match.");
                    }
                    PrivateLateBoundObject.Path = value;
                }
            }
        }
        
        // Public static scope property which is used by the various methods.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static System.Management.ManagementScope StaticScope {
            get {
                return statMgmtScope;
            }
            set {
                statMgmtScope = value;
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAllowMultipleResponsesNull {
            get {
                if ((curObj["AllowMultipleResponses"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property allow multiple responses to be sent back to a 2-way RL (Isolated-ho" +
            "st ONLY).")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool AllowMultipleResponses {
            get {
                if ((curObj["AllowMultipleResponses"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["AllowMultipleResponses"]));
            }
            set {
                curObj["AllowMultipleResponses"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAuthTrustedNull {
            get {
                if ((curObj["AuthTrusted"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates whether the BizTalk Host is trusted to collect authentica" +
            "tion information.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool AuthTrusted {
            get {
                if ((curObj["AuthTrusted"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["AuthTrusted"]));
            }
            set {
                curObj["AuthTrusted"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("A short description (one-line string) of the CIM_Setting object.")]
        public string Caption {
            get {
                return ((string)(curObj["Caption"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("When the host instances of this host are clustered, this property contains the cl" +
            "uster resource group name set by the Administrator.")]
        public string ClusterResourceGroupName {
            get {
                return ((string)(curObj["ClusterResourceGroupName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDBQueueSizeThresholdNull {
            get {
                if ((curObj["DBQueueSizeThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum number of items in the Database.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint DBQueueSizeThreshold {
            get {
                if ((curObj["DBQueueSizeThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["DBQueueSizeThreshold"]));
            }
            set {
                curObj["DBQueueSizeThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDBSessionThresholdNull {
            get {
                if ((curObj["DBSessionThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum number of DB Sessions (per CPU) allowed before throttling begins.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint DBSessionThreshold {
            get {
                if ((curObj["DBSessionThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["DBSessionThreshold"]));
            }
            set {
                curObj["DBSessionThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This is a comment field that allows to associate some friendly name with a decryp" +
            "tion certificate. Max length for this property is 256 characters.")]
        public string DecryptCertComment {
            get {
                return ((string)(curObj["DecryptCertComment"]));
            }
            set {
                curObj["DecryptCertComment"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"ThumbPrint of the Decryption certificate. The Certificate Thumbprint is a digest of the certificate data and is found in the Certificate Details, and is expressed as a hexadecimal value. Example: 'FD36 90F0 EB49 F7B8 D3AB 1C69 8E02 ED84 5738 7868'. Max length for this property is 80 characters.")]
        public string DecryptCertThumbprint {
            get {
                return ((string)(curObj["DecryptCertThumbprint"]));
            }
            set {
                curObj["DecryptCertThumbprint"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDehydrationBehaviorNull {
            get {
                if ((curObj["DehydrationBehavior"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls the dehydration behavior of the orhestration(XLANG) engine" +
            ". Only if Custom is selected then other xlang settings should be editable.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public DehydrationBehaviorValues DehydrationBehavior {
            get {
                if ((curObj["DehydrationBehavior"] == null)) {
                    return ((DehydrationBehaviorValues)(System.Convert.ToInt32(3)));
                }
                return ((DehydrationBehaviorValues)(System.Convert.ToInt32(curObj["DehydrationBehavior"])));
            }
            set {
                if ((DehydrationBehaviorValues.NULL_ENUM_VALUE == value)) {
                    curObj["DehydrationBehavior"] = null;
                }
                else {
                    curObj["DehydrationBehavior"] = value;
                }
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDeliveryQueueSizeNull {
            get {
                if ((curObj["DeliveryQueueSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Size of the in-memory Queue that the host maintains as a temporary placeholder fo" +
            "r delivering messages.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint DeliveryQueueSize {
            get {
                if ((curObj["DeliveryQueueSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["DeliveryQueueSize"]));
            }
            set {
                curObj["DeliveryQueueSize"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("A description of the CIM_Setting object.")]
        public string Description {
            get {
                return ((string)(curObj["Description"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsGlobalMemoryThresholdNull {
            get {
                if ((curObj["GlobalMemoryThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum System-wide Virtual Memory (in percent) usage allowed before throttling b" +
            "egins.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint GlobalMemoryThreshold {
            get {
                if ((curObj["GlobalMemoryThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["GlobalMemoryThreshold"]));
            }
            set {
                curObj["GlobalMemoryThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHostTrackingNull {
            get {
                if ((curObj["HostTracking"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates whether instances of this BizTalk Host will host the trac" +
            "king sub service.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool HostTracking {
            get {
                if ((curObj["HostTracking"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["HostTracking"]));
            }
            set {
                curObj["HostTracking"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHostTypeNull {
            get {
                if ((curObj["HostType"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates which runtime model the instances of the BizTalk Host wil" +
            "l be running in.  This property is required for instance creation.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public HostTypeValues HostType {
            get {
                if ((curObj["HostType"] == null)) {
                    return ((HostTypeValues)(System.Convert.ToInt32(0)));
                }
                return ((HostTypeValues)(System.Convert.ToInt32(curObj["HostType"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInflightMessageThresholdNull {
            get {
                if ((curObj["InflightMessageThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum number of in-memory in-flight messages allowed before throttling Message " +
            "Delivery begins.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint InflightMessageThreshold {
            get {
                if ((curObj["InflightMessageThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["InflightMessageThreshold"]));
            }
            set {
                curObj["InflightMessageThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsIsDefaultNull {
            get {
                if ((curObj["IsDefault"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates whether the BizTalk Host represented by this WMI instance" +
            " is the default BizTalk Host in the BizTalk group.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool IsDefault {
            get {
                if ((curObj["IsDefault"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["IsDefault"]));
            }
            set {
                curObj["IsDefault"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsIsHost32BitOnlyNull {
            get {
                if ((curObj["IsHost32BitOnly"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates whether the host instance process should be created as 32" +
            "-bit on both 32-bit and 64-bit servers.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool IsHost32BitOnly {
            get {
                if ((curObj["IsHost32BitOnly"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["IsHost32BitOnly"]));
            }
            set {
                curObj["IsHost32BitOnly"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property contains a default logon for the BizTalk Host instance creation UI." +
            " Max length for this property is 128 characters.")]
        public string LastUsedLogon {
            get {
                return ((string)(curObj["LastUsedLogon"]));
            }
            set {
                curObj["LastUsedLogon"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLegacyWhitespaceNull {
            get {
                if ((curObj["LegacyWhitespace"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property turns preservation of White Spaces with mapping on or off.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool LegacyWhitespace {
            get {
                if ((curObj["LegacyWhitespace"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["LegacyWhitespace"]));
            }
            set {
                curObj["LegacyWhitespace"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessageDeliveryMaximumDelayNull {
            get {
                if ((curObj["MessageDeliveryMaximumDelay"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum Delay (in milliseconds) imposed for Message Delivery Throttling. Zero ind" +
            "icates disable Message Delivery Throttling.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessageDeliveryMaximumDelay {
            get {
                if ((curObj["MessageDeliveryMaximumDelay"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessageDeliveryMaximumDelay"]));
            }
            set {
                curObj["MessageDeliveryMaximumDelay"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessageDeliveryOverdriveFactorNull {
            get {
                if ((curObj["MessageDeliveryOverdriveFactor"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Percent factor by which the system will overdrive the Input rate for Message Deli" +
            "very Throttling.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessageDeliveryOverdriveFactor {
            get {
                if ((curObj["MessageDeliveryOverdriveFactor"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessageDeliveryOverdriveFactor"]));
            }
            set {
                curObj["MessageDeliveryOverdriveFactor"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessageDeliverySampleSpaceSizeNull {
            get {
                if ((curObj["MessageDeliverySampleSpaceSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property indicates the number of samples that are used for determining the r" +
            "ate of the Message Delivery to all Service Classes of the Host.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessageDeliverySampleSpaceSize {
            get {
                if ((curObj["MessageDeliverySampleSpaceSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessageDeliverySampleSpaceSize"]));
            }
            set {
                curObj["MessageDeliverySampleSpaceSize"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessageDeliverySampleSpaceWindowNull {
            get {
                if ((curObj["MessageDeliverySampleSpaceWindow"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time-window (in milliseconds) beyond which samples will be deemed invalid for con" +
            "sideration.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessageDeliverySampleSpaceWindow {
            get {
                if ((curObj["MessageDeliverySampleSpaceWindow"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessageDeliverySampleSpaceWindow"]));
            }
            set {
                curObj["MessageDeliverySampleSpaceWindow"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagePublishMaximumDelayNull {
            get {
                if ((curObj["MessagePublishMaximumDelay"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum Delay (in milliseconds) imposed for Message Publishing Throttling. Zero i" +
            "ndicates disable Message Publishing Throttling.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagePublishMaximumDelay {
            get {
                if ((curObj["MessagePublishMaximumDelay"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagePublishMaximumDelay"]));
            }
            set {
                curObj["MessagePublishMaximumDelay"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagePublishOverdriveFactorNull {
            get {
                if ((curObj["MessagePublishOverdriveFactor"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Percent Factor by which the system will overdrive the Input rate.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagePublishOverdriveFactor {
            get {
                if ((curObj["MessagePublishOverdriveFactor"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagePublishOverdriveFactor"]));
            }
            set {
                curObj["MessagePublishOverdriveFactor"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagePublishSampleSpaceSizeNull {
            get {
                if ((curObj["MessagePublishSampleSpaceSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Number of samples that are used for determining the rate of the Message Publishin" +
            "g by the Service Classes.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagePublishSampleSpaceSize {
            get {
                if ((curObj["MessagePublishSampleSpaceSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagePublishSampleSpaceSize"]));
            }
            set {
                curObj["MessagePublishSampleSpaceSize"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagePublishSampleSpaceWindowNull {
            get {
                if ((curObj["MessagePublishSampleSpaceWindow"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time-window (in milliseconds) beyond which samples will be deemed invalid for con" +
            "sideration.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagePublishSampleSpaceWindow {
            get {
                if ((curObj["MessagePublishSampleSpaceWindow"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagePublishSampleSpaceWindow"]));
            }
            set {
                curObj["MessagePublishSampleSpaceWindow"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagingMaxReceiveIntervalNull {
            get {
                if ((curObj["MessagingMaxReceiveInterval"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property is the BizTalk Server polling behavior as the interval in milliseco" +
            "nds when BizTalk host instance is looking for new messages that have arrived. Al" +
            "lowed values is 50 to int max")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagingMaxReceiveInterval {
            get {
                if ((curObj["MessagingMaxReceiveInterval"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagingMaxReceiveInterval"]));
            }
            set {
                curObj["MessagingMaxReceiveInterval"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMessagingReqRespTTLNull {
            get {
                if ((curObj["MessagingReqRespTTL"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property is the default timeout for request response messages. Allowed value" +
            "s is 1 to 60.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MessagingReqRespTTL {
            get {
                if ((curObj["MessagingReqRespTTL"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MessagingReqRespTTL"]));
            }
            set {
                curObj["MessagingReqRespTTL"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This optional property can be used to override the initial catalog part of the Bi" +
            "zTalk Messaging Management database connect string, and represents the database " +
            "name. Max length for this property is 123 characters.")]
        public string MgmtDbNameOverride {
            get {
                return ((string)(curObj["MgmtDbNameOverride"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This optional property can be used to override the data source part of the BizTal" +
            "k Messaging Management database connect string. Max length for this property is " +
            "80 characters.")]
        public string MgmtDbServerOverride {
            get {
                return ((string)(curObj["MgmtDbServerOverride"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMsgAgentPerfCounterServiceClassIDNull {
            get {
                if ((curObj["MsgAgentPerfCounterServiceClassID"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls the service for which perf counters are shown")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public MsgAgentPerfCounterServiceClassIDValues MsgAgentPerfCounterServiceClassID {
            get {
                if ((curObj["MsgAgentPerfCounterServiceClassID"] == null)) {
                    return ((MsgAgentPerfCounterServiceClassIDValues)(System.Convert.ToInt32(5)));
                }
                return ((MsgAgentPerfCounterServiceClassIDValues)(System.Convert.ToInt32(curObj["MsgAgentPerfCounterServiceClassID"])));
            }
            set {
                if ((MsgAgentPerfCounterServiceClassIDValues.NULL_ENUM_VALUE == value)) {
                    curObj["MsgAgentPerfCounterServiceClassID"] = null;
                }
                else {
                    curObj["MsgAgentPerfCounterServiceClassID"] = value;
                }
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property contains the name of the BizTalk Host.  This property is required f" +
            "or instance creation. Max length for this property is 80 characters.")]
        public string Name {
            get {
                return ((string)(curObj["Name"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property contains the name of the Windows group. It can be either a local or a domain Windows group. This group is granted access to the BizTalk Host Queue that is created for this BizTalk Host. All accounts used to host BizTalk Host instances of this type must be members of this group.  This property is required for instance creation. Max length for this property is 63 characters.")]
        public string NTGroupName {
            get {
                return ((string)(curObj["NTGroupName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsProcessMemoryThresholdNull {
            get {
                if ((curObj["ProcessMemoryThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum Process Memory (in percent) allowed before throttling begins.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ProcessMemoryThreshold {
            get {
                if ((curObj["ProcessMemoryThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ProcessMemoryThreshold"]));
            }
            set {
                curObj["ProcessMemoryThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The identifier by which the CIM_Setting object is known.")]
        public string SettingID {
            get {
                return ((string)(curObj["SettingID"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSubscriptionPauseAtNull {
            get {
                if ((curObj["SubscriptionPauseAt"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("If any subscription has >= PauseAt messages waiting to be consumed, then stop del" +
            "ivering messages to subscription instance.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint SubscriptionPauseAt {
            get {
                if ((curObj["SubscriptionPauseAt"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["SubscriptionPauseAt"]));
            }
            set {
                curObj["SubscriptionPauseAt"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSubscriptionResumeAtNull {
            get {
                if ((curObj["SubscriptionResumeAt"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("If any subscription was paused due to  PauseAt setting and no of outstanding mess" +
            "ages got down to ResumeAt value, then messagebox will resume giving messages to " +
            "subscription.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint SubscriptionResumeAt {
            get {
                if ((curObj["SubscriptionResumeAt"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["SubscriptionResumeAt"]));
            }
            set {
                curObj["SubscriptionResumeAt"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThreadPoolSizeNull {
            get {
                if ((curObj["ThreadPoolSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum number of messaging engine threads per CPU.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThreadPoolSize {
            get {
                if ((curObj["ThreadPoolSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThreadPoolSize"]));
            }
            set {
                curObj["ThreadPoolSize"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThreadThresholdNull {
            get {
                if ((curObj["ThreadThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Maximum number of threads in the process (per CPU) allowed before throttling begi" +
            "ns.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThreadThreshold {
            get {
                if ((curObj["ThreadThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThreadThreshold"]));
            }
            set {
                curObj["ThreadThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingBatchMemoryThresholdPercentNull {
            get {
                if ((curObj["ThrottlingBatchMemoryThresholdPercent"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property controls the memory threshold beyond which to throttle the publishing of a batch of messages. The batch memory threshold is computed by multiplying this percentage factor by the Process memory usage threshold. If the memory estimated to execute a publishing batch exceeds the batch memory threshold, the batch will be subject to process memory based throttling. Otherwise the batch will be exempt from process memory based throttling even when the total process memory exceeds the Process memory usage threshold. A value of zero indicates that all publishing batches may be subject to process memory based throttling even if the memory estimated to execute the batch is minimal.. MinValue 0 and Max value 100")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingBatchMemoryThresholdPercent {
            get {
                if ((curObj["ThrottlingBatchMemoryThresholdPercent"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingBatchMemoryThresholdPercent"]));
            }
            set {
                curObj["ThrottlingBatchMemoryThresholdPercent"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingDeliveryOverrideNull {
            get {
                if ((curObj["ThrottlingDeliveryOverride"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls Throttling delivery  override")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ThrottlingDeliveryOverrideValues ThrottlingDeliveryOverride {
            get {
                if ((curObj["ThrottlingDeliveryOverride"] == null)) {
                    return ((ThrottlingDeliveryOverrideValues)(System.Convert.ToInt32(3)));
                }
                return ((ThrottlingDeliveryOverrideValues)(System.Convert.ToInt32(curObj["ThrottlingDeliveryOverride"])));
            }
            set {
                if ((ThrottlingDeliveryOverrideValues.NULL_ENUM_VALUE == value)) {
                    curObj["ThrottlingDeliveryOverride"] = null;
                }
                else {
                    curObj["ThrottlingDeliveryOverride"] = value;
                }
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingDeliveryOverrideSeverityNull {
            get {
                if ((curObj["ThrottlingDeliveryOverrideSeverity"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property increases / decreases the severity of an outbound throttling condit" +
            "ion.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingDeliveryOverrideSeverity {
            get {
                if ((curObj["ThrottlingDeliveryOverrideSeverity"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingDeliveryOverrideSeverity"]));
            }
            set {
                curObj["ThrottlingDeliveryOverrideSeverity"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingLimitToTriggerGCNull {
            get {
                if ((curObj["ThrottlingLimitToTriggerGC"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property controls when a .NET garbage collection (GC) will be triggered as process memory consumption increases and approaches the threshold. When the memory consumption exceeds this percentage value of the memory threshold, a GC is triggered. MinValue 50 and Max value 100")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingLimitToTriggerGC {
            get {
                if ((curObj["ThrottlingLimitToTriggerGC"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingLimitToTriggerGC"]));
            }
            set {
                curObj["ThrottlingLimitToTriggerGC"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingPublishOverrideNull {
            get {
                if ((curObj["ThrottlingPublishOverride"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls Throttling Publish Override")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ThrottlingPublishOverrideValues ThrottlingPublishOverride {
            get {
                if ((curObj["ThrottlingPublishOverride"] == null)) {
                    return ((ThrottlingPublishOverrideValues)(System.Convert.ToInt32(3)));
                }
                return ((ThrottlingPublishOverrideValues)(System.Convert.ToInt32(curObj["ThrottlingPublishOverride"])));
            }
            set {
                if ((ThrottlingPublishOverrideValues.NULL_ENUM_VALUE == value)) {
                    curObj["ThrottlingPublishOverride"] = null;
                }
                else {
                    curObj["ThrottlingPublishOverride"] = value;
                }
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingPublishOverrideSeverityNull {
            get {
                if ((curObj["ThrottlingPublishOverrideSeverity"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property increases / decreases the severity of an inbound throttling conditi" +
            "on.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingPublishOverrideSeverity {
            get {
                if ((curObj["ThrottlingPublishOverrideSeverity"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingPublishOverrideSeverity"]));
            }
            set {
                curObj["ThrottlingPublishOverrideSeverity"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingSeverityDatabaseSizeNull {
            get {
                if ((curObj["ThrottlingSeverityDatabaseSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls the severity of a database sized triggered throttling cond" +
            "ition. This is specified in percentage value and this parameter sets the severit" +
            "y of a throttling condition caused when the Message count in database threshold " +
            "is exceeded.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingSeverityDatabaseSize {
            get {
                if ((curObj["ThrottlingSeverityDatabaseSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingSeverityDatabaseSize"]));
            }
            set {
                curObj["ThrottlingSeverityDatabaseSize"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingSeverityInflightMessageNull {
            get {
                if ((curObj["ThrottlingSeverityInflightMessage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property controls the reaction time of throttling when the In-process size exceeds the threshold. This is specified in percentage value and this parameter sets the severity of a throttling condition caused when the In-process messages per CPU threshold is exceeded.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingSeverityInflightMessage {
            get {
                if ((curObj["ThrottlingSeverityInflightMessage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingSeverityInflightMessage"]));
            }
            set {
                curObj["ThrottlingSeverityInflightMessage"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingSeverityProcessMemoryNull {
            get {
                if ((curObj["ThrottlingSeverityProcessMemory"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls the severity of a process memory triggered throttling cond" +
            "ition. This is specified in percentage value and this parameter sets the severit" +
            "y of a throttling condition caused when the Process memory usage threshold is ex" +
            "ceeded.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingSeverityProcessMemory {
            get {
                if ((curObj["ThrottlingSeverityProcessMemory"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingSeverityProcessMemory"]));
            }
            set {
                curObj["ThrottlingSeverityProcessMemory"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingSpoolMultiplierNull {
            get {
                if ((curObj["ThrottlingSpoolMultiplier"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property specifies the factor by which the Message count in database threshold will be multiplied and then compared against the current record count in the spool table to determine whether the system should throttle on spool table size. If this is set to 0, spool table size is not used as a consideration for determining a throttling condition. Max value 1000")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingSpoolMultiplier {
            get {
                if ((curObj["ThrottlingSpoolMultiplier"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingSpoolMultiplier"]));
            }
            set {
                curObj["ThrottlingSpoolMultiplier"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThrottlingTrackingDataMultiplierNull {
            get {
                if ((curObj["ThrottlingTrackingDataMultiplier"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"This property specifies the factor by which the Message count in database threshold will be multiplied and then compared against the current record count in the tracking table to determine whether the system should throttle on tracking table size. If this is set to 0, tracking table size is not used as a consideration for determining a throttling condition. Max value 1000")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThrottlingTrackingDataMultiplier {
            get {
                if ((curObj["ThrottlingTrackingDataMultiplier"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThrottlingTrackingDataMultiplier"]));
            }
            set {
                curObj["ThrottlingTrackingDataMultiplier"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTimeBasedMaxThresholdNull {
            get {
                if ((curObj["TimeBasedMaxThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property is the max waittime (sec) an orchestration instance could block bef" +
            "ore being dehydrated.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint TimeBasedMaxThreshold {
            get {
                if ((curObj["TimeBasedMaxThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["TimeBasedMaxThreshold"]));
            }
            set {
                curObj["TimeBasedMaxThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTimeBasedMinThresholdNull {
            get {
                if ((curObj["TimeBasedMinThreshold"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property is the min waittime(sec) an orchestration instance could block befo" +
            "re being dehydrated.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint TimeBasedMinThreshold {
            get {
                if ((curObj["TimeBasedMinThreshold"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["TimeBasedMinThreshold"]));
            }
            set {
                curObj["TimeBasedMinThreshold"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUseDefaultAppDomainForIsolatedAdapterNull {
            get {
                if ((curObj["UseDefaultAppDomainForIsolatedAdapter"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property controls whether the isolated adapter runs in default app domain or" +
            " the domain of the caller.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool UseDefaultAppDomainForIsolatedAdapter {
            get {
                if ((curObj["UseDefaultAppDomainForIsolatedAdapter"] == null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["UseDefaultAppDomainForIsolatedAdapter"]));
            }
            set {
                curObj["UseDefaultAppDomainForIsolatedAdapter"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsXlangMaxReceiveIntervalNull {
            get {
                if ((curObj["XlangMaxReceiveInterval"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("This property is the for XLANG. Allowed values is 50 to int max")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint XlangMaxReceiveInterval {
            get {
                if ((curObj["XlangMaxReceiveInterval"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["XlangMaxReceiveInterval"]));
            }
            set {
                curObj["XlangMaxReceiveInterval"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam) {
            if (((path != null) 
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj) {
            if (((theObj != null) 
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
                if ((parentClasses != null)) {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1)) {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool ShouldSerializeAllowMultipleResponses() {
            if ((this.IsAllowMultipleResponsesNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetAllowMultipleResponses() {
            curObj["AllowMultipleResponses"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeAuthTrusted() {
            if ((this.IsAuthTrustedNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetAuthTrusted() {
            curObj["AuthTrusted"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDBQueueSizeThreshold() {
            if ((this.IsDBQueueSizeThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetDBQueueSizeThreshold() {
            curObj["DBQueueSizeThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDBSessionThreshold() {
            if ((this.IsDBSessionThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetDBSessionThreshold() {
            curObj["DBSessionThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private void ResetDecryptCertComment() {
            curObj["DecryptCertComment"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private void ResetDecryptCertThumbprint() {
            curObj["DecryptCertThumbprint"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDehydrationBehavior() {
            if ((this.IsDehydrationBehaviorNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetDehydrationBehavior() {
            curObj["DehydrationBehavior"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDeliveryQueueSize() {
            if ((this.IsDeliveryQueueSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetDeliveryQueueSize() {
            curObj["DeliveryQueueSize"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeGlobalMemoryThreshold() {
            if ((this.IsGlobalMemoryThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetGlobalMemoryThreshold() {
            curObj["GlobalMemoryThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeHostTracking() {
            if ((this.IsHostTrackingNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetHostTracking() {
            curObj["HostTracking"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeHostType() {
            if ((this.IsHostTypeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeInflightMessageThreshold() {
            if ((this.IsInflightMessageThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetInflightMessageThreshold() {
            curObj["InflightMessageThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeIsDefault() {
            if ((this.IsIsDefaultNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetIsDefault() {
            curObj["IsDefault"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeIsHost32BitOnly() {
            if ((this.IsIsHost32BitOnlyNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetIsHost32BitOnly() {
            curObj["IsHost32BitOnly"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private void ResetLastUsedLogon() {
            curObj["LastUsedLogon"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeLegacyWhitespace() {
            if ((this.IsLegacyWhitespaceNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetLegacyWhitespace() {
            curObj["LegacyWhitespace"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessageDeliveryMaximumDelay() {
            if ((this.IsMessageDeliveryMaximumDelayNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessageDeliveryMaximumDelay() {
            curObj["MessageDeliveryMaximumDelay"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessageDeliveryOverdriveFactor() {
            if ((this.IsMessageDeliveryOverdriveFactorNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessageDeliveryOverdriveFactor() {
            curObj["MessageDeliveryOverdriveFactor"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessageDeliverySampleSpaceSize() {
            if ((this.IsMessageDeliverySampleSpaceSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessageDeliverySampleSpaceSize() {
            curObj["MessageDeliverySampleSpaceSize"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessageDeliverySampleSpaceWindow() {
            if ((this.IsMessageDeliverySampleSpaceWindowNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessageDeliverySampleSpaceWindow() {
            curObj["MessageDeliverySampleSpaceWindow"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagePublishMaximumDelay() {
            if ((this.IsMessagePublishMaximumDelayNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagePublishMaximumDelay() {
            curObj["MessagePublishMaximumDelay"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagePublishOverdriveFactor() {
            if ((this.IsMessagePublishOverdriveFactorNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagePublishOverdriveFactor() {
            curObj["MessagePublishOverdriveFactor"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagePublishSampleSpaceSize() {
            if ((this.IsMessagePublishSampleSpaceSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagePublishSampleSpaceSize() {
            curObj["MessagePublishSampleSpaceSize"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagePublishSampleSpaceWindow() {
            if ((this.IsMessagePublishSampleSpaceWindowNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagePublishSampleSpaceWindow() {
            curObj["MessagePublishSampleSpaceWindow"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagingMaxReceiveInterval() {
            if ((this.IsMessagingMaxReceiveIntervalNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagingMaxReceiveInterval() {
            curObj["MessagingMaxReceiveInterval"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMessagingReqRespTTL() {
            if ((this.IsMessagingReqRespTTLNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMessagingReqRespTTL() {
            curObj["MessagingReqRespTTL"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeMsgAgentPerfCounterServiceClassID() {
            if ((this.IsMsgAgentPerfCounterServiceClassIDNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetMsgAgentPerfCounterServiceClassID() {
            curObj["MsgAgentPerfCounterServiceClassID"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeProcessMemoryThreshold() {
            if ((this.IsProcessMemoryThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetProcessMemoryThreshold() {
            curObj["ProcessMemoryThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeSubscriptionPauseAt() {
            if ((this.IsSubscriptionPauseAtNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetSubscriptionPauseAt() {
            curObj["SubscriptionPauseAt"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeSubscriptionResumeAt() {
            if ((this.IsSubscriptionResumeAtNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetSubscriptionResumeAt() {
            curObj["SubscriptionResumeAt"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThreadPoolSize() {
            if ((this.IsThreadPoolSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThreadPoolSize() {
            curObj["ThreadPoolSize"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThreadThreshold() {
            if ((this.IsThreadThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThreadThreshold() {
            curObj["ThreadThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingBatchMemoryThresholdPercent() {
            if ((this.IsThrottlingBatchMemoryThresholdPercentNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingBatchMemoryThresholdPercent() {
            curObj["ThrottlingBatchMemoryThresholdPercent"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingDeliveryOverride() {
            if ((this.IsThrottlingDeliveryOverrideNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingDeliveryOverride() {
            curObj["ThrottlingDeliveryOverride"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingDeliveryOverrideSeverity() {
            if ((this.IsThrottlingDeliveryOverrideSeverityNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingDeliveryOverrideSeverity() {
            curObj["ThrottlingDeliveryOverrideSeverity"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingLimitToTriggerGC() {
            if ((this.IsThrottlingLimitToTriggerGCNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingLimitToTriggerGC() {
            curObj["ThrottlingLimitToTriggerGC"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingPublishOverride() {
            if ((this.IsThrottlingPublishOverrideNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingPublishOverride() {
            curObj["ThrottlingPublishOverride"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingPublishOverrideSeverity() {
            if ((this.IsThrottlingPublishOverrideSeverityNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingPublishOverrideSeverity() {
            curObj["ThrottlingPublishOverrideSeverity"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingSeverityDatabaseSize() {
            if ((this.IsThrottlingSeverityDatabaseSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingSeverityDatabaseSize() {
            curObj["ThrottlingSeverityDatabaseSize"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingSeverityInflightMessage() {
            if ((this.IsThrottlingSeverityInflightMessageNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingSeverityInflightMessage() {
            curObj["ThrottlingSeverityInflightMessage"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingSeverityProcessMemory() {
            if ((this.IsThrottlingSeverityProcessMemoryNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingSeverityProcessMemory() {
            curObj["ThrottlingSeverityProcessMemory"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingSpoolMultiplier() {
            if ((this.IsThrottlingSpoolMultiplierNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingSpoolMultiplier() {
            curObj["ThrottlingSpoolMultiplier"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeThrottlingTrackingDataMultiplier() {
            if ((this.IsThrottlingTrackingDataMultiplierNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetThrottlingTrackingDataMultiplier() {
            curObj["ThrottlingTrackingDataMultiplier"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeTimeBasedMaxThreshold() {
            if ((this.IsTimeBasedMaxThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetTimeBasedMaxThreshold() {
            curObj["TimeBasedMaxThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeTimeBasedMinThreshold() {
            if ((this.IsTimeBasedMinThresholdNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetTimeBasedMinThreshold() {
            curObj["TimeBasedMinThreshold"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeUseDefaultAppDomainForIsolatedAdapter() {
            if ((this.IsUseDefaultAppDomainForIsolatedAdapterNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetUseDefaultAppDomainForIsolatedAdapter() {
            curObj["UseDefaultAppDomainForIsolatedAdapter"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeXlangMaxReceiveInterval() {
            if ((this.IsXlangMaxReceiveIntervalNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetXlangMaxReceiveInterval() {
            curObj["XlangMaxReceiveInterval"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject() {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions) {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put(putOptions);
            }
        }
        
        private void Initialize() {
            AutoCommitProp = true;
            isEmbedded = false;
        }
        
        private static string ConstructPath(string keyMgmtDbNameOverride, string keyMgmtDbServerOverride, string keyName) {
            string strPath = "root\\MicrosoftBizTalkServer:MSBTS_HostSetting";
            strPath = string.Concat(strPath, string.Concat(".MgmtDbNameOverride=", string.Concat("\"", string.Concat(keyMgmtDbNameOverride, "\""))));
            strPath = string.Concat(strPath, string.Concat(",MgmtDbServerOverride=", string.Concat("\"", string.Concat(keyMgmtDbServerOverride, "\""))));
            strPath = string.Concat(strPath, string.Concat(",Name=", string.Concat("\"", string.Concat(keyName, "\""))));
            return strPath;
        }
        
        private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            Initialize();
            if ((path != null)) {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true)) {
                    throw new System.ArgumentException("Class name does not match.");
                }
            }
            PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        
        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static HostSettingCollection GetInstances() {
            return GetInstances(null, null, null);
        }
        
        public static HostSettingCollection GetInstances(string condition) {
            return GetInstances(null, condition, null);
        }
        
        public static HostSettingCollection GetInstances(string[] selectedProperties) {
            return GetInstances(null, null, selectedProperties);
        }
        
        public static HostSettingCollection GetInstances(string condition, string[] selectedProperties) {
            return GetInstances(null, condition, selectedProperties);
        }
        
        public static HostSettingCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\MicrosoftBizTalkServer";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
            pathObj.ClassName = "MSBTS_HostSetting";
            pathObj.NamespacePath = "root\\MicrosoftBizTalkServer";
            System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions == null)) {
                enumOptions = new System.Management.EnumerationOptions();
                enumOptions.EnsureLocatable = true;
            }
            return new HostSettingCollection(clsObject.GetInstances(enumOptions));
        }
        
        public static HostSettingCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition) {
            return GetInstances(mgmtScope, condition, null);
        }
        
        public static HostSettingCollection GetInstances(System.Management.ManagementScope mgmtScope, string[] selectedProperties) {
            return GetInstances(mgmtScope, null, selectedProperties);
        }
        
        public static HostSettingCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, string[] selectedProperties) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\MicrosoftBizTalkServer";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("MSBTS_HostSetting", condition, selectedProperties));
            System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new HostSettingCollection(ObjectSearcher.Get());
        }
        
        [Browsable(true)]
        public static HostSetting CreateInstance() {
            System.Management.ManagementScope mgmtScope = null;
            if ((statMgmtScope == null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else {
                mgmtScope = statMgmtScope;
            }
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
            return new HostSetting(tmpMgmtClass.CreateInstance());
        }
        
        [Browsable(true)]
        public void Delete() {
            PrivateLateBoundObject.Delete();
        }
        
        public enum DehydrationBehaviorValues {
            
            Always = 0,
            
            Never = 1,
            
            Custom = 2,
            
            NULL_ENUM_VALUE = 3,
        }
        
        public enum HostTypeValues {
            
            In_process = 1,
            
            Isolated = 2,
            
            NULL_ENUM_VALUE = 0,
        }
        
        public enum MsgAgentPerfCounterServiceClassIDValues {
            
            Val__59F295B0_3123_416E_966B_A2C6D65FF8E6_ = 0,
            
            Val__683AEDF1_027D_4006_AE9A_443D1A5746FC_ = 1,
            
            Val__00000000_0000_0000_0000_000000000000_ = 4,
            
            NULL_ENUM_VALUE = 5,
        }
        
        public enum ThrottlingDeliveryOverrideValues {
            
            Val_0 = 0,
            
            Val_1 = 1,
            
            Val_2 = 2,
            
            NULL_ENUM_VALUE = 3,
        }
        
        public enum ThrottlingPublishOverrideValues {
            
            Val_0 = 0,
            
            Val_1 = 1,
            
            Val_2 = 2,
            
            NULL_ENUM_VALUE = 3,
        }
        
        // Enumerator implementation for enumerating instances of the class.
        public class HostSettingCollection : object, ICollection {
            
            private ManagementObjectCollection privColObj;
            
            public HostSettingCollection(ManagementObjectCollection objCollection) {
                privColObj = objCollection;
            }
            
            public virtual int Count {
                get {
                    return privColObj.Count;
                }
            }
            
            public virtual bool IsSynchronized {
                get {
                    return privColObj.IsSynchronized;
                }
            }
            
            public virtual object SyncRoot {
                get {
                    return this;
                }
            }
            
            public virtual void CopyTo(System.Array array, int index) {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1)) {
                    array.SetValue(new HostSetting(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }
            
            public virtual System.Collections.IEnumerator GetEnumerator() {
                return new HostSettingEnumerator(privColObj.GetEnumerator());
            }
            
            public class HostSettingEnumerator : object, System.Collections.IEnumerator {
                
                private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;
                
                public HostSettingEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) {
                    privObjEnum = objEnum;
                }
                
                public virtual object Current {
                    get {
                        return new HostSetting(((System.Management.ManagementObject)(privObjEnum.Current)));
                    }
                }
                
                public virtual bool MoveNext() {
                    return privObjEnum.MoveNext();
                }
                
                public virtual void Reset() {
                    privObjEnum.Reset();
                }
            }
        }
        
        // TypeConverter to handle null values for ValueType properties
        public class WMIValueTypeConverter : TypeConverter {
            
            private TypeConverter baseConverter;
            
            private System.Type baseType;
            
            public WMIValueTypeConverter(System.Type inBaseType) {
                baseConverter = TypeDescriptor.GetConverter(inBaseType);
                baseType = inBaseType;
            }
            
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType) {
                return baseConverter.CanConvertFrom(context, srcType);
            }
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
                return baseConverter.CanConvertTo(context, destinationType);
            }
            
            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
                return baseConverter.ConvertFrom(context, culture, value);
            }
            
            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary) {
                return baseConverter.CreateInstance(context, dictionary);
            }
            
            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetCreateInstanceSupported(context);
            }
            
            public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar) {
                return baseConverter.GetProperties(context, value, attributeVar);
            }
            
            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetPropertiesSupported(context);
            }
            
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValues(context);
            }
            
            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesExclusive(context);
            }
            
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesSupported(context);
            }
            
            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
                if ((baseType.BaseType == typeof(System.Enum))) {
                    if ((value.GetType() == destinationType)) {
                        return value;
                    }
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return  "NULL_ENUM_VALUE" ;
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((baseType == typeof(bool)) 
                            && (baseType.BaseType == typeof(System.ValueType)))) {
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return "";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((context != null) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }
        
        // Embedded class to represent WMI system Properties.
        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public class ManagementSystemProperties {
            
            private System.Management.ManagementBaseObject PrivateLateBoundObject;
            
            public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject) {
                PrivateLateBoundObject = ManagedObject;
            }
            
            [Browsable(true)]
            public int GENUS {
                get {
                    return ((int)(PrivateLateBoundObject["__GENUS"]));
                }
            }
            
            [Browsable(true)]
            public string CLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__CLASS"]));
                }
            }
            
            [Browsable(true)]
            public string SUPERCLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
                }
            }
            
            [Browsable(true)]
            public string DYNASTY {
                get {
                    return ((string)(PrivateLateBoundObject["__DYNASTY"]));
                }
            }
            
            [Browsable(true)]
            public string RELPATH {
                get {
                    return ((string)(PrivateLateBoundObject["__RELPATH"]));
                }
            }
            
            [Browsable(true)]
            public int PROPERTY_COUNT {
                get {
                    return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
                }
            }
            
            [Browsable(true)]
            public string[] DERIVATION {
                get {
                    return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
                }
            }
            
            [Browsable(true)]
            public string SERVER {
                get {
                    return ((string)(PrivateLateBoundObject["__SERVER"]));
                }
            }
            
            [Browsable(true)]
            public string NAMESPACE {
                get {
                    return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
                }
            }
            
            [Browsable(true)]
            public string PATH {
                get {
                    return ((string)(PrivateLateBoundObject["__PATH"]));
                }
            }
        }
    }
}
