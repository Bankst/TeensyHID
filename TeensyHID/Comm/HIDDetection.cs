using System;
using System.Linq;
using System.Management;
using TeensyHID.HID;

namespace TeensyHID.Comm
{
    public static class HIDDetection
    {
        private static readonly Guid GUID_DEVCLASS_USB = new Guid("{36fc9e60-c465-11cf-8056-444553540000}");

        private static ManagementEventWatcher _eventWatcher;
        private static WqlEventQuery _eventQuery;

        public static event InsertedEventHandler InsertEvent = (s, e) => { };
        public static event DetachedEventHandler DetachEvent = (s, e) => { };

        public delegate void InsertedEventHandler(object sender, EventArrivedEventArgs e);
        public delegate void DetachedEventHandler(object sender, EventArrivedEventArgs e);

        public static void StartDeviceDetection()
        {

            _eventQuery = new WqlEventQuery
            {
                EventClassName = "__InstanceOperationEvent",
                WithinInterval = TimeSpan.FromMilliseconds(250),
                Condition = @"TargetInstance ISA 'Win32_USBControllerDevice' "
            };

            try
            {
                _eventWatcher = new ManagementEventWatcher(_eventQuery);
                _eventWatcher.EventArrived += UsbEventArrived;
                _eventWatcher.Start();
            }
            catch (Exception e)
            {
                Debug.LogException($"Failed to start DeviceDetection.\n{e.StackTrace}");
            }
        }

        public static void StopDeviceDetection()
        {
            _eventWatcher?.Stop();
        }

        public static void UsbEventArrived(object sender, EventArrivedEventArgs e)
        {
            var mbo = e.NewEvent;
            var instance = (ManagementBaseObject)mbo["TargetInstance"];
            var devPath = instance.Properties["Dependent"];
            var toFind = @"HID\\VID_16C0";
            var mobjDevices = new ManagementObjectSearcher($@"Select * From Win32_PnPEntity").Get();
            var mobjDevice = from ManagementObject x in mobjDevices where x.Path.RelativePath.Contains("16C0") select x;
//            var whatever = mobjDevice.Where(x => x.Contains(@"16C0"));
            //            var changedDevice = new USBControllerDevice(devPath);
            var isCreationEvent = mbo.ClassPath.ClassName == "__InstanceCreationEvent";
            var vid = instance;
//            var changedDevice;


            if (isCreationEvent)
            {
                Debug.Log("New device inserted.");
                InsertEvent.Invoke(sender, e);
            }
            else
            {
                Debug.Log("Device detached.");
                DetachEvent.Invoke(sender, e);
            }
        }

    }
}
