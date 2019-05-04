using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;

namespace TeensyHID.HID
{
    // Functions ShouldSerialize<PropertyName> are functions used by VS property browser to check if a particular property has to be serialized. These functions are added for all ValueType properties ( properties of type Int32, BOOL etc.. which cannot be set to null). These functions use Is<PropertyName>Null function. These functions are also used in the TypeConverter implementation for the properties to check for NULL value of property so that an empty value can be shown in Property browser in case of Drag and Drop in Visual studio.
    // Functions Is<PropertyName>Null() are used to check if a property is NULL.
    // Functions Reset<PropertyName> are added for Nullable Read/Write properties. These functions are used by VS designer in property browser to set a property to NULL.
    // Every property added to the class for WMI property has attributes set to define its behavior in Visual Studio designer and also to define a TypeConverter to be used.
    // An Early Bound class generated for the WMI class.Win32_USBControllerDevice
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class USBControllerDevice : Component
    {

        // Private property to hold the WMI namespace in which the class resides.
        private static readonly string _createdWmiNamespace = "root\\CimV2";

        // Private property to hold the name of WMI class which created this class.
        private static readonly string _createdClassName = "Win32_USBControllerDevice";

        // Underlying lateBound WMI object.
        private ManagementObject _privateLateBoundObject;

        // Flag to indicate if the instance is an embedded object.
        private bool _isEmbedded;

        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public USBControllerDevice()
        {
            InitializeObject(null, null, null);
        }

        public USBControllerDevice(ManagementPath keyAntecedent, ManagementPath keyDependent)
        {
            InitializeObject(null, new ManagementPath(ConstructPath(keyAntecedent, keyDependent)), null);
        }

        public USBControllerDevice(ManagementScope mgmtScope, ManagementPath keyAntecedent, ManagementPath keyDependent)
        {
            InitializeObject(mgmtScope, new ManagementPath(ConstructPath(keyAntecedent, keyDependent)), null);
        }

        public USBControllerDevice(ManagementPath path, ObjectGetOptions getOptions)
        {
            InitializeObject(null, path, getOptions);
        }

        public USBControllerDevice(ManagementScope mgmtScope, ManagementPath path)
        {
            InitializeObject(mgmtScope, path, null);
        }

        public USBControllerDevice(ManagementPath path)
        {
            InitializeObject(null, path, null);
        }

        public USBControllerDevice(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions getOptions)
        {
            InitializeObject(mgmtScope, path, getOptions);
        }

        public USBControllerDevice(ManagementObject theObject)
        {
            Initialize();
            if (CheckIfProperClass(theObject))
            {
                _privateLateBoundObject = theObject;
                SystemProperties = new ManagementSystemProperties(_privateLateBoundObject);
                LateBoundObject = _privateLateBoundObject;
            }
            else
            {
                throw new ArgumentException("Class name does not match.");
            }
        }

        public USBControllerDevice(ManagementBaseObject theObject)
        {
            Initialize();
            if (CheckIfProperClass(theObject))
            {
                SystemProperties = new ManagementSystemProperties(theObject);
                LateBoundObject = theObject;
                _isEmbedded = true;
            }
            else
            {
                throw new ArgumentException("Class name does not match.");
            }
        }

        // Property returns the namespace of the WMI class.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace => "root\\CimV2";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName
        {
            get
            {
                var strRet = _createdClassName;
                if (LateBoundObject == null) return strRet;
                strRet = (string)LateBoundObject["__CLASS"];
                if (string.IsNullOrEmpty(strRet))
                {
                    strRet = _createdClassName;
                }
                return strRet;
            }
        }

        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties { get; private set; }

        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementBaseObject LateBoundObject { get; private set; }

        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementScope Scope
        {
            get => !_isEmbedded ? _privateLateBoundObject.Scope : null;
            set
            {
                if (!_isEmbedded)
                {
                    _privateLateBoundObject.Scope = value;
                }
            }
        }

        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit { get; set; }

        // The ManagementPath of the underlying WMI object.
        [Browsable(true)]
        public ManagementPath Path
        {
            get => !_isEmbedded ? _privateLateBoundObject.Path : null;
            set
            {
                if (_isEmbedded) return;
                if (CheckIfProperClass(null, value, null) != true)
                {
                    throw new ArgumentException("Class name does not match.");
                }

                _privateLateBoundObject.Path = value;
            }
        }

        // Public static scope property which is used by the various methods.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static ManagementScope StaticScope { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAccessStateNull => LateBoundObject["AccessState"] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The AccessState property indicates whether the controller is actively commanding or accessing the device (value=1) or not (value=2).  Also, the value, ""Unknown"" (0), can be defined. This information is necessary when a logical device can be commanded by, or accessed through, multiple controllers.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public AccessStateValues AccessState =>
            LateBoundObject["AccessState"] == null
                ? (AccessStateValues) Convert.ToInt32(3)
                : (AccessStateValues) Convert.ToInt32(LateBoundObject["AccessState"]);

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The CIM_USBController antecedent reference represents the Universal Serial Bus (USB) controller associated with this device.")]
        public ManagementPath Antecedent => LateBoundObject["Antecedent"] != null ? new ManagementPath(LateBoundObject["Antecedent"].ToString()) : null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The CIM_LogicalDevice dependent reference represents the CIM_LogicalDevice connected to the Universal Serial Bus (USB) controller.")]
        public ManagementPath Dependent => LateBoundObject["Dependent"] != null ? new ManagementPath(LateBoundObject["Dependent"].ToString()) : null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNegotiatedDataWidthNull => LateBoundObject["NegotiatedDataWidth"] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"When several bus and/or connection data widths are possible, the NegotiatedDataWidth property defines the one in use between the devices.  Data width is specified in bits.  If data width is not negotiated, or if this information is not available/important to device management, the property should be set to 0.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public uint NegotiatedDataWidth => (uint?) LateBoundObject["NegotiatedDataWidth"] ?? Convert.ToUInt32(0);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNegotiatedSpeedNull => LateBoundObject["NegotiatedSpeed"] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"When several bus and/or connection speeds are possible, the NegotiatedSpeed property defines the one in use between the devices.  Speed is specified in bits per second.  If connection or bus speeds are not negotiated, or if this information is not available/important to device management, the property should be set to 0.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public ulong NegotiatedSpeed => (ulong?) LateBoundObject["NegotiatedSpeed"] ?? Convert.ToUInt64(0);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNumberOfHardResetsNull => LateBoundObject["NumberOfHardResets"] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Number of hard resets issued by the controller. A hard reset returns the device to its initialization or \'boot-up\' state. All internal device state information and data are lost.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public uint NumberOfHardResets =>
            (uint?) LateBoundObject["NumberOfHardResets"] ?? Convert.ToUInt32(0);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNumberOfSoftResetsNull => LateBoundObject["NumberOfSoftResets"] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Number of soft resets issued by the controller. A soft reset does not completely clear current device state and/or data. Exact semantics are dependent on the device, and on the protocols and mechanisms used to communicate to it.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public uint NumberOfSoftResets => (uint?) LateBoundObject["NumberOfSoftResets"] ?? Convert.ToUInt32(0);

        private bool CheckIfProperClass(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions optionsParam)
        {
            if (path != null
                && string.Compare(path.ClassName, ManagementClassName, true, CultureInfo.InvariantCulture) == 0)
            {
                return true;
            }

            return CheckIfProperClass(new ManagementObject(mgmtScope, path, optionsParam));
        }

        private bool CheckIfProperClass(ManagementBaseObject theObj)
        {
            if (theObj != null
                && string.Compare((string)theObj["__CLASS"], ManagementClassName, true, CultureInfo.InvariantCulture) == 0)
            {
                return true;
            }

            var parentClasses = (Array) theObj?["__DERIVATION"];
            if (parentClasses == null) return false;
            int count;
            for (count = 0; count < parentClasses.Length; count = count + 1)
            {
                if (string.Compare((string)parentClasses.GetValue(count), ManagementClassName, true, CultureInfo.InvariantCulture) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldSerializeAccessState()
        {
            return IsAccessStateNull == false;
        }

        private bool ShouldSerializeNegotiatedDataWidth()
        {
            return IsNegotiatedDataWidthNull == false;
        }

        private bool ShouldSerializeNegotiatedSpeed()
        {
            return IsNegotiatedSpeedNull == false;
        }

        private bool ShouldSerializeNumberOfHardResets()
        {
            return IsNumberOfHardResetsNull == false;
        }

        private bool ShouldSerializeNumberOfSoftResets()
        {
            return IsNumberOfSoftResetsNull == false;
        }

        [Browsable(true)]
        public void CommitObject()
        {
            if (_isEmbedded == false)
            {
                _privateLateBoundObject.Put();
            }
        }

        [Browsable(true)]
        public void CommitObject(PutOptions putOptions)
        {
            if (_isEmbedded == false)
            {
                _privateLateBoundObject.Put(putOptions);
            }
        }

        private void Initialize()
        {
            AutoCommit = true;
            _isEmbedded = false;
        }

        private static string ConstructPath(ManagementPath keyAntecedent, ManagementPath keyDependent)
        {
            var strPath = "root\\CimV2:Win32_USBControllerDevice";
            strPath = string.Concat(strPath, string.Concat(".Antecedent=", keyAntecedent.ToString()));
            strPath = string.Concat(strPath, string.Concat(",Dependent=", keyDependent.ToString()));
            return strPath;
        }

        private void InitializeObject(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions getOptions)
        {
            Initialize();
            if (path != null)
            {
                if (CheckIfProperClass(mgmtScope, path, getOptions) != true)
                {
                    throw new ArgumentException("Class name does not match.");
                }
            }
            _privateLateBoundObject = new ManagementObject(mgmtScope, path, getOptions);
            SystemProperties = new ManagementSystemProperties(_privateLateBoundObject);
            LateBoundObject = _privateLateBoundObject;
        }

        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static USBControllerDeviceCollection GetInstances()
        {
            return GetInstances(null, null, null);
        }

        public static USBControllerDeviceCollection GetInstances(string condition)
        {
            return GetInstances(null, condition, null);
        }

        public static USBControllerDeviceCollection GetInstances(string[] selectedProperties)
        {
            return GetInstances(null, null, selectedProperties);
        }

        public static USBControllerDeviceCollection GetInstances(string condition, string[] selectedProperties)
        {
            return GetInstances(null, condition, selectedProperties);
        }

        public static USBControllerDeviceCollection GetInstances(ManagementScope mgmtScope, EnumerationOptions enumOptions)
        {
            if (mgmtScope == null)
            {
                mgmtScope = StaticScope ?? new ManagementScope {Path = {NamespacePath = "root\\CimV2"}};
            }

            var pathObj = new ManagementPath {ClassName = "Win32_USBControllerDevice", NamespacePath = "root\\CimV2"};
            var clsObject = new ManagementClass(mgmtScope, pathObj, null);
            if (enumOptions != null) return new USBControllerDeviceCollection(clsObject.GetInstances(enumOptions));
            enumOptions = new EnumerationOptions {EnsureLocatable = true};
            return new USBControllerDeviceCollection(clsObject.GetInstances(enumOptions));
        }

        public static USBControllerDeviceCollection GetInstances(ManagementScope mgmtScope, string condition)
        {
            return GetInstances(mgmtScope, condition, null);
        }

        public static USBControllerDeviceCollection GetInstances(ManagementScope mgmtScope, string[] selectedProperties)
        {
            return GetInstances(mgmtScope, null, selectedProperties);
        }

        public static USBControllerDeviceCollection GetInstances(ManagementScope mgmtScope, string condition, string[] selectedProperties)
        {
            if (mgmtScope == null)
            {
                mgmtScope = StaticScope ?? new ManagementScope {Path = {NamespacePath = "root\\CimV2"}};
            }
            var objectSearcher = new ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_USBControllerDevice", condition, selectedProperties));
            var enumOptions = new EnumerationOptions {EnsureLocatable = true};
            objectSearcher.Options = enumOptions;
            return new USBControllerDeviceCollection(objectSearcher.Get());
        }

        [Browsable(true)]
        public static USBControllerDevice CreateInstance()
        {
            var mgmtScope = StaticScope ?? new ManagementScope {Path = {NamespacePath = _createdWmiNamespace}};
            var mgmtPath = new ManagementPath(_createdClassName);
            var tmpMgmtClass = new ManagementClass(mgmtScope, mgmtPath, null);
            return new USBControllerDevice(tmpMgmtClass.CreateInstance());
        }

        [Browsable(true)]
        public void Delete()
        {
            _privateLateBoundObject.Delete();
        }

        public enum AccessStateValues
        {

            Unknown0 = 0,

            Active = 1,

            Inactive = 2,

            NullEnumValue = 3
        }

        // Enumerator implementation for enumerating instances of the class.
        public class USBControllerDeviceCollection : object, ICollection
        {

            private readonly ManagementObjectCollection _privColObj;

            public USBControllerDeviceCollection(ManagementObjectCollection objCollection)
            {
                _privColObj = objCollection;
            }

            public virtual int Count => _privColObj.Count;

            public virtual bool IsSynchronized => _privColObj.IsSynchronized;

            public virtual object SyncRoot => this;

            public virtual void CopyTo(Array array, int index)
            {
                _privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; nCtr < array.Length; nCtr = nCtr + 1)
                {
                    array.SetValue(new USBControllerDevice((ManagementObject)array.GetValue(nCtr)), nCtr);
                }
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new USBControllerDeviceEnumerator(_privColObj.GetEnumerator());
            }

            public class USBControllerDeviceEnumerator : object, IEnumerator
            {

                private readonly ManagementObjectCollection.ManagementObjectEnumerator _privObjEnum;

                public USBControllerDeviceEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
                {
                    _privObjEnum = objEnum;
                }

                public virtual object Current => new USBControllerDevice((ManagementObject)_privObjEnum.Current);

                public virtual bool MoveNext()
                {
                    return _privObjEnum.MoveNext();
                }

                public virtual void Reset()
                {
                    _privObjEnum.Reset();
                }
            }
        }

        // TypeConverter to handle null values for ValueType properties
        public class WmiValueTypeConverter : TypeConverter
        {

            private readonly TypeConverter _baseConverter;

            private readonly Type _baseType;

            public WmiValueTypeConverter(Type inBaseType)
            {
                _baseConverter = TypeDescriptor.GetConverter(inBaseType);
                _baseType = inBaseType;
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
            {
                return _baseConverter.CanConvertFrom(context, srcType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return _baseConverter.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return _baseConverter.ConvertFrom(context, culture, value);
            }

            public override object CreateInstance(ITypeDescriptorContext context, IDictionary dictionary)
            {
                return _baseConverter.CreateInstance(context, dictionary);
            }

            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
            {
                return _baseConverter.GetCreateInstanceSupported(context);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributeVar)
            {
                return _baseConverter.GetProperties(context, value, attributeVar);
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return _baseConverter.GetPropertiesSupported(context);
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return _baseConverter.GetStandardValues(context);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return _baseConverter.GetStandardValuesExclusive(context);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return _baseConverter.GetStandardValuesSupported(context);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (_baseType.BaseType == typeof(Enum))
                {
                    if (value.GetType() == destinationType)
                    {
                        return value;
                    }
                    if (context.PropertyDescriptor != null && context.PropertyDescriptor.ShouldSerializeValue(context.Instance))
                    {
                        return "NULL_ENUM_VALUE";
                    }
                    return _baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (_baseType == typeof(bool)
                    && _baseType.BaseType == typeof(ValueType))
                {
                    if (context.PropertyDescriptor != null && value == null && context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)
                    {
                        return "";
                    }
                    return _baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (context.PropertyDescriptor != null && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))
                {
                    return "";
                }
                return _baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }

        // Embedded class to represent WMI system Properties.
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ManagementSystemProperties
        {

            private readonly ManagementBaseObject _privateLateBoundObject;

            public ManagementSystemProperties(ManagementBaseObject managedObject)
            {
                _privateLateBoundObject = managedObject;
            }

            [Browsable(true)]
            public int Genus => (int)_privateLateBoundObject["__GENUS"];

            [Browsable(true)]
            public string Class => (string)_privateLateBoundObject["__CLASS"];

            [Browsable(true)]
            public string Superclass => (string)_privateLateBoundObject["__SUPERCLASS"];

            [Browsable(true)]
            public string Dynasty => (string)_privateLateBoundObject["__DYNASTY"];

            [Browsable(true)]
            public string Relpath => (string)_privateLateBoundObject["__RELPATH"];

            [Browsable(true)]
            public int PropertyCount => (int)_privateLateBoundObject["__PROPERTY_COUNT"];

            [Browsable(true)]
            public string[] Derivation => (string[])_privateLateBoundObject["__DERIVATION"];

            [Browsable(true)]
            public string Server => (string)_privateLateBoundObject["__SERVER"];

            [Browsable(true)]
            public string Namespace => (string)_privateLateBoundObject["__NAMESPACE"];

            [Browsable(true)]
            public string Path => (string)_privateLateBoundObject["__PATH"];
        }
    }
}
