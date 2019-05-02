using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Win32.SafeHandles;

namespace TeensyHID
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	internal static class UsbNotification
	{
		public const int DbtDevicearrival = 0x8000; // system detected a new device        
		public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
		public const int WmDevicechange = 0x0219; // device change event      
		private const int DbtDevtypDeviceinterface = 5;
		private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
		private static IntPtr _notificationHandle;

		/// <summary>
		/// Registers a window to receive notifications when USB devices are plugged or unplugged.
		/// </summary>
		/// <param name="windowHandle">Handle to the window receiving notifications.</param>
		public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
		{
			var dbi = new DevBroadcastDeviceinterface
			{
				DeviceType = DbtDevtypDeviceinterface,
				Reserved = 0,
				ClassGuid = GuidDevinterfaceUSBDevice,
				Name = 0
			};

			dbi.Size = Marshal.SizeOf(dbi);
			var buffer = Marshal.AllocHGlobal(dbi.Size);
			Marshal.StructureToPtr(dbi, buffer, true);

			_notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
		}

		/// <summary>
		/// Unregisters the window for USB device notifications
		/// </summary>
		public static void UnregisterUsbDeviceNotification()
		{
			UnregisterDeviceNotification(_notificationHandle);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

		[DllImport("user32.dll")]
		private static extern bool UnregisterDeviceNotification(IntPtr handle);

		[StructLayout(LayoutKind.Sequential)]
		private struct DevBroadcastDeviceinterface
		{
			internal int Size;
			internal int DeviceType;
			internal int Reserved;
			internal Guid ClassGuid;
			internal short Name;
		}
	}

	//A class representing a USB device
	public class Device : EventArgs
	{
		public ushort Vid { get; }
		public ushort Pid { get; }
		public string DeviceID { get; }
		public string ClassGuid { get; }
		public string Caption { get; }
		public string Manufacturer { get; }

		public Device()
		{
			Vid = 0x0000;
			Pid = 0x0000;
			DeviceID = "";
			ClassGuid = "";
			Caption = "";
			Manufacturer = "";
		}

		public Device(ushort vid, ushort pid)
		{
			Vid = vid;
			Pid = pid;
			DeviceID = "";
			ClassGuid = "";
			Caption = "";
			Manufacturer = "";
		}

		public Device(ManagementObject wmiObj)
		{
			DeviceID = wmiObj["DeviceID"].ToString().ToUpper();
			ClassGuid = wmiObj["ClassGuid"].ToString();
			Caption = wmiObj["Caption"].ToString();
			Manufacturer = wmiObj["Manufacturer"].ToString();
			var match = Regex.Match(DeviceID, "PID_(.{4})", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				var pidString = match.Groups[1].Value;
				Pid = ushort.Parse(pidString, NumberStyles.HexNumber);
			}
			match = Regex.Match(DeviceID, "VID_(.{4})", RegexOptions.IgnoreCase);

			if (!match.Success) return;

			var vidString = match.Groups[1].Value;
			Vid = ushort.Parse(vidString, NumberStyles.HexNumber);
		}

		public override string ToString()
		{
			return $"{Caption} (VID=0x{Vid:X4} PID=0x{Pid:X4})";
		}
	}


	//A class representing a USB buffer
	public class UsbBuffer : EventArgs
	{
		public byte[] Buffer
		{
			get;
			set;
		}
		public bool RequestTransfer
		{
			get;
			set;
		}
		public bool TransferSuccessful
		{
			get;
			set;
		}

		public UsbBuffer(byte[] buffer)
		{
			//buffer = new byte[65];
			Buffer = buffer;
			RequestTransfer = false;
			TransferSuccessful = false;
			Clear();
		}

		public void Clear()
		{
			for (var i = 0; i < 65; ++i)
			{
				Buffer[i] = 0xFF;
			}
		}
	}


	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "RedundantAssignment")]
	public class HidUtility
	{
		public delegate void DeviceAddedEventHandler(object sender, Device dev);
		public delegate void DeviceRemovedEventHandler(object sender, Device dev);
		public delegate void ConnectionStatusChangedEventHandler(object sender, ConnectionStatusEventArgs e);
		public delegate void SendPacketEventHandler(object sender, UsbBuffer outBuffer);
		public delegate void PacketSentEventHandler(object sender, UsbBuffer outBuffer);
		public delegate void ReceivePacketEventHandler(object sender, UsbBuffer inBuffer);
		public delegate void PacketReceivedEventHandler(object sender, UsbBuffer inBuffer);

		public event DeviceAddedEventHandler RaiseDeviceAddedEvent;
		public event DeviceRemovedEventHandler RaiseDeviceRemovedEvent;
		public event ConnectionStatusChangedEventHandler RaiseConnectionStatusChangedEvent;
		public event SendPacketEventHandler RaiseSendPacketEvent;
		public event PacketSentEventHandler RaisePacketSentEvent;
		public event ReceivePacketEventHandler RaiseReceivePacketEvent;
		public event PacketReceivedEventHandler RaisePacketReceivedEvent;

		private readonly List<string> _deviceIdList;
		public List<Device> DeviceList { get; }
		private Device _deviceToConnectTo;
		SafeFileHandle _writeHandleToUSBDevice;
		SafeFileHandle _readHandleToUSBDevice;

		public enum UsbConnectionStatus
		{
			Disconnected,
			Connected,
			NotWorking
		}

		public UsbConnectionStatus ConnectionStatus { get; private set; }

		//A class representing a USB device
		public class ConnectionStatusEventArgs : EventArgs
		{
			public UsbConnectionStatus ConnectionStatus { get; }

			public ConnectionStatusEventArgs(UsbConnectionStatus status)
			{
				ConnectionStatus = status;
			}

			public override string ToString()
			{
				return ConnectionStatus.ToString();
			}
		}


		//Constant definitions from setupapi.h, which we aren't allowed to include directly since this is C#
		internal const uint DigcfPresent = 0x02;
		internal const uint DigcfDeviceinterface = 0x10;
		//Constants for CreateFile() and other file I/O functions
		internal const short FileAttributeNormal = 0x80;
		internal const short InvalidHandleValue = -1;
		internal const uint GenericRead = 0x80000000;
		internal const uint GenericWrite = 0x40000000;
		internal const uint CreateNew = 1;
		internal const uint CreateAlways = 2;
		internal const uint OpenExisting = 3;
		internal const uint FileShareRead = 0x00000001;
		internal const uint FileShareWrite = 0x00000002;
		//Constant definitions for certain WM_DEVICECHANGE messages
		internal const uint WmDevicechange = 0x0219;
		internal const uint DbtDevicearrival = 0x8000;
		internal const uint DbtDeviceremovepending = 0x8003;
		internal const uint DbtDeviceremovecomplete = 0x8004;
		internal const uint DbtConfigchanged = 0x0018;
		//Other constant definitions
		internal const uint DbtDevtypDeviceinterface = 0x05;
		internal const uint DeviceNotifyWindowHandle = 0x00;
		internal const uint ErrorSuccess = 0x00;
		internal const uint ErrorNoMoreItems = 0x00000103;
		internal const uint SpdrpHardwareid = 0x00000001;

		//Various structure definitions for structures that this code will be using
		internal struct SpDeviceInterfaceData
		{
			internal uint CbSize;               //DWORD
			internal Guid InterfaceClassGuid;   //GUID
			internal uint Flags;                //DWORD
			internal uint Reserved;             //ULONG_PTR MSDN says ULONG_PTR is "typedef unsigned __int3264 ULONG_PTR;"  
		}

		internal struct SpDeviceInterfaceDetailData
		{
			internal uint CbSize;               //DWORD
			internal char[] DevicePath;         //TCHAR array of any size
		}

		internal struct SpDevinfoData
		{
			internal uint CbSize;       //DWORD
			internal Guid ClassGuid;    //GUID
			internal uint DevInst;      //DWORD
			internal uint Reserved;     //ULONG_PTR  MSDN says ULONG_PTR is "typedef unsigned __int3264 ULONG_PTR;"  
		}

		internal struct DevBroadcastDeviceinterface
		{
			internal uint DbccSize;            //DWORD
			internal uint DbccDevicetype;      //DWORD
			internal uint DbccReserved;        //DWORD
			internal Guid DbccClassguid;       //GUID
			internal char[] DbccName;          //TCHAR array
		}

		//DLL Imports.  Need these to access various C style unmanaged functions contained in their respective DLL files.
		//--------------------------------------------------------------------------------------------------------------
		//Returns a HDEVINFO type for a device information set.  We will need the 
		//HDEVINFO as in input parameter for calling many of the other SetupDixxx() functions.
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern IntPtr SetupDiGetClassDevs(
			ref Guid classGuid,     //LPGUID    Input: Need to supply the class GUID. 
			IntPtr enumerator,      //PCTSTR    Input: Use NULL here, not important for our purposes
			IntPtr hwndParent,      //HWND      Input: Use NULL here, not important for our purposes
			uint flags);            //DWORD     Input: Flags describing what kind of filtering to use.

		//Gives us "PSP_DEVICE_INTERFACE_DATA" which contains the Interface specific GUID (different
		//from class GUID).  We need the interface GUID to get the device path.
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiEnumDeviceInterfaces(
			IntPtr deviceInfoSet,           //Input: Give it the HDEVINFO we got from SetupDiGetClassDevs()
			IntPtr deviceInfoData,          //Input (optional)
			ref Guid interfaceClassGuid,    //Input 
			uint memberIndex,               //Input: "Index" of the device you are interested in getting the path for.
			ref SpDeviceInterfaceData deviceInterfaceData);    //Output: This function fills in an "SP_DEVICE_INTERFACE_DATA" structure.

		//SetupDiDestroyDeviceInfoList() frees up memory by destroying a DeviceInfoList
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiDestroyDeviceInfoList(
			IntPtr deviceInfoSet);          //Input: Give it a handle to a device info list to deallocate from RAM.

		//SetupDiEnumDeviceInfo() fills in an "SP_DEVINFO_DATA" structure, which we need for SetupDiGetDeviceRegistryProperty()
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiEnumDeviceInfo(
			IntPtr deviceInfoSet,
			uint memberIndex,
			ref SpDevinfoData deviceInterfaceData);

		//SetupDiGetDeviceRegistryProperty() gives us the hardware ID, which we use to check to see if it has matching VID/PID
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiGetDeviceRegistryProperty(
			IntPtr deviceInfoSet,
			ref SpDevinfoData deviceInfoData,
			uint property,
			ref uint propertyRegDataType,
			IntPtr propertyBuffer,
			uint propertyBufferSize,
			ref uint requiredSize);

		//SetupDiGetDeviceInterfaceDetail() gives us a device path, which is needed before CreateFile() can be used.
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr deviceInfoSet,                   //Input: Wants HDEVINFO which can be obtained from SetupDiGetClassDevs()
			ref SpDeviceInterfaceData deviceInterfaceData,                    //Input: Pointer to an structure which defines the device interface.  
			IntPtr deviceInterfaceDetailData,      //Output: Pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA structure, which will receive the device path.
			uint deviceInterfaceDetailDataSize,     //Input: Number of bytes to retrieve.
			ref uint requiredSize,                  //Output (optional): The number of bytes needed to hold the entire struct 
			IntPtr deviceInfoData);                 //Output (optional): Pointer to a SP_DEVINFO_DATA structure

		//Overload for SetupDiGetDeviceInterfaceDetail().  Need this one since we can't pass NULL pointers directly in C#.
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr deviceInfoSet,                   //Input: Wants HDEVINFO which can be obtained from SetupDiGetClassDevs()
			ref SpDeviceInterfaceData deviceInterfaceData,               //Input: Pointer to an structure which defines the device interface.  
			IntPtr deviceInterfaceDetailData,       //Output: Pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA structure, which will contain the device path.
			uint deviceInterfaceDetailDataSize,     //Input: Number of bytes to retrieve.
			IntPtr requiredSize,                    //Output (optional): Pointer to a DWORD to tell you the number of bytes needed to hold the entire struct 
			IntPtr deviceInfoData);                 //Output (optional): Pointer to a SP_DEVINFO_DATA structure

		//Need this function for receiving all of the WM_DEVICECHANGE messages.  See MSDN documentation for
		//description of what this function does/how to use it. Note: name is remapped "RegisterDeviceNotificationUM" to
		//avoid possible build error conflicts.
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern IntPtr RegisterDeviceNotification(
			IntPtr hRecipient,
			IntPtr notificationFilter,
			uint flags);

		//Takes in a device path and opens a handle to the device.
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern SafeFileHandle CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		//Uses a handle (created with CreateFile()), and lets us write USB data to the device.
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool WriteFile(
			SafeFileHandle hFile,
			byte[] lpBuffer,
			uint nNumberOfBytesToWrite,
			ref uint lpNumberOfBytesWritten,
			IntPtr lpOverlapped);

		//Uses a handle (created with CreateFile()), and lets us read USB data from the device.
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool ReadFile(
			SafeFileHandle hFile,
			IntPtr lpBuffer,
			uint nNumberOfBytesToRead,
			ref uint lpNumberOfBytesRead,
			IntPtr lpOverlapped);

		//--------------- Global Varibles Section ------------------
		//USB related variables that need to have wide scope.
		//bool AttachedState = false;                     //Need to keep track of the USB device attachment status for proper plug and play operation.
		//bool AttachedButBroken = false;

		//String DevicePath = null;   //Need the find the proper device path before you can open file handles.

		//Globally Unique Identifier (GUID) for HID class devices.  Windows uses GUIDs to identify things.
		Guid _interfaceClassGuid = new Guid(0x4d1e55b2, 0xf16f, 0x11cf, 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30);

		private const int WmCopydata = 0x004A;

		private async void OnDeviceRemoved()
		{
			//Return immediately and do all the work asynchronously
			await Task.Yield();
			//Get a list with the device IDs of all removed devices
			var newDeviceIdList = GetDeviceIdList();
			var removedDeviceIdList = new List<string>();
			foreach (var devId in _deviceIdList)
			{
				if (!newDeviceIdList.Contains(devId))
				{
					removedDeviceIdList.Add(devId);
				}
			}
			//Get removed devices
			var removedDeviceList = new List<Device>();
			foreach (var dev in DeviceList)
			{
				if (removedDeviceIdList.Contains(dev.DeviceID))
				{
					removedDeviceList.Add(dev);
				}
			}
			//Loop through removed devices
			foreach (var removedDevice in removedDeviceList)
			{
				//Remove removedDevice from DeviceList
				DeviceList.Remove(removedDevice);
				//Remove removedDevice's device ID from DeviceIdList
				_deviceIdList.Remove(removedDevice.DeviceID);
				//Raise event if there are any subscribers
				RaiseDeviceRemovedEvent?.Invoke(this, removedDevice);
			}
			// Check if our device has been disconnected
			if (ConnectionStatus != UsbConnectionStatus.Disconnected)
			{
				var devicePath = GetDevicePath(_deviceToConnectTo);
				// Try to connect if a device path has been obtained
				if (devicePath == null)
				{
					CloseDevice();
				}
			}
		}

		async void OnDeviceAdded()
		{
			// Return immediately and do all the work asynchronously
			await Task.Yield();
			// Loop through devices
			var newDeviceList = GetDeviceList();
			foreach (var dev in newDeviceList)
			{
				if (!(_deviceIdList.Contains(dev.DeviceID)))
				{
					_deviceIdList.Add(dev.DeviceID);
					DeviceList.Add(dev);
					// Raise event if there are any subscribers
					RaiseDeviceAddedEvent?.Invoke(this, dev);
				}
			}
			// Try to connect to the device if we are not already connected
			if (ConnectionStatus != UsbConnectionStatus.Connected)
			{
				SelectDevice(_deviceToConnectTo);
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			if (msg == UsbNotification.WmDevicechange)
			{
				switch ((int)wparam)
				{
					case UsbNotification.DbtDeviceremovecomplete:
						OnDeviceRemoved();
						break;
					case UsbNotification.DbtDevicearrival:
						OnDeviceAdded();
						break;
				}
			}
			return IntPtr.Zero;
		}

		private IntPtr CreateMessageOnlyWindow()
		{
			var hwndMessage = new IntPtr(-3);
			var sourceParam = new HwndSourceParameters { ParentWindow = hwndMessage };
			var source = new HwndSource(sourceParam);
			source.AddHook(WndProc);
			return source.Handle;
		}

		//--------------- End of Global Varibles ------------------

		//public HidUtility(sendPacket_delegate sendPacket_h, packetSent_delegate packetSent_h, receivePacket_delegate receivePacket_h, packetReceived_delegate packetReceived_h)
		public HidUtility()
		{
			_deviceToConnectTo = new Device();
			_deviceIdList = GetDeviceIdList();
			DeviceList = GetDeviceList();

			var sourceHandle = CreateMessageOnlyWindow();
			UsbNotification.RegisterUsbDeviceNotification(sourceHandle);

			var usbThread = new BackgroundWorker();
			usbThread.DoWork += UsbThread_DoWork;
			usbThread.RunWorkerAsync();
		}

		public void SelectDevice(Device dev)
		{
			// Save the device for future use
			_deviceToConnectTo = dev;
			// Close any device already connected
			CloseDevice();
			// Try to obtain a device path
			var devicePath = GetDevicePath(_deviceToConnectTo);
			// Try to connect if a device path has been obtained
			if (devicePath != null)
			{
				OpenDevice(devicePath);
			}
		}

		// Returns a list with the device IDs of all HID devices
		// Filters may be removed if a complete list of USB devices is desired
		private List<string> GetDeviceIdList()
		{
			var deviceIDs = new List<string>();
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice");
			var objs = searcher.Get();
			foreach (var o in objs)
			{
				var wmiHd = (ManagementObject) o;
				var dep = wmiHd["Dependent"].ToString();
				var match = Regex.Match(dep, "\"(.+VID.+PID.+)\"$", RegexOptions.IgnoreCase);
				if (!match.Success) continue;

				var devId = match.Groups[1].Value;
				devId = devId.Replace(@"\\", @"\");
				devId = devId.ToUpper();
				if (devId.Substring(0, 3) == "HID")
				{
					deviceIDs.Add(devId);
				}
			}
			return deviceIDs;
		}

		// Returns a list of Device object representing all devices returned by getDeviceIdList()
		private List<Device> GetDeviceList()
		{
			var devices = new List<Device>();
			var deviceIDs = GetDeviceIdList();
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
			var objs = searcher.Get();

			foreach (var o in objs)
			{
				var wmiHd = (ManagementObject) o;
				var deviceId = wmiHd["DeviceID"].ToString();
				if (!deviceIDs.Contains(deviceId)) continue;
//					var caption = wmiHd["Caption"].ToString();
				var dev = new Device(wmiHd);
				devices.Add(dev);
			}
			return devices;
		}

		private String GetDevicePath(Device dev)
		{
			/*
           Before we can "connect" our application to our USB embedded device, we must first find the device.
           A USB bus can have many devices simultaneously connected, so somehow we have to find our device only.
           This is done with the Vendor ID (VID) and Product ID (PID).  Each USB product line should have
           a unique combination of VID and PID.  

           Microsoft has created a number of functions which are useful for finding plug and play devices.  Documentation
           for each function used can be found in the MSDN library.  We will be using the following functions (unmanaged C functions):

           SetupDiGetClassDevs()					//provided by setupapi.dll, which comes with Windows
           SetupDiEnumDeviceInterfaces()			//provided by setupapi.dll, which comes with Windows
           GetLastError()							//provided by kernel32.dll, which comes with Windows
           SetupDiDestroyDeviceInfoList()			//provided by setupapi.dll, which comes with Windows
           SetupDiGetDeviceInterfaceDetail()		//provided by setupapi.dll, which comes with Windows
           SetupDiGetDeviceRegistryProperty()		//provided by setupapi.dll, which comes with Windows
           CreateFile()							//provided by kernel32.dll, which comes with Windows

           In order to call these unmanaged functions, the Marshal class is very useful.

           We will also be using the following unusual data types and structures.  Documentation can also be found in
           the MSDN library:

           PSP_DEVICE_INTERFACE_DATA
           PSP_DEVICE_INTERFACE_DETAIL_DATA
           SP_DEVINFO_DATA
           HDEVINFO
           HANDLE
           GUID

           The ultimate objective of the following code is to get the device path, which will be used elsewhere for getting
           read and write handles to the USB device.  Once the read/write handles are opened, only then can this
           PC application begin reading/writing to the USB device using the WriteFile() and ReadFile() functions.

           Getting the device path is a multi-step round about process, which requires calling several of the
           SetupDixxx() functions provided by setupapi.dll.
           */

			//Device path we are trying to get
			String devicePath;
			// The device ID from the registry should contain this string (when ignoring upper/lower case)
			var deviceIdSubstring = string.Format("Vid_{0:X4}&Pid_{1:X4}", dev.Vid, dev.Pid);
			deviceIdSubstring = deviceIdSubstring.ToLowerInvariant();
			// The device path should contain this string (when ignoring upper/lower case)
			var devicePathSubstring = dev.DeviceID.Replace(@"\", @"#");
			devicePathSubstring = devicePathSubstring.ToLowerInvariant();

			try
			{
				var deviceInfoTable = IntPtr.Zero;
				var interfaceDataStructure = new SpDeviceInterfaceData();
				var detailedInterfaceDataStructure = new SpDeviceInterfaceDetailData();
				var devInfoData = new SpDevinfoData();

				uint interfaceIndex = 0;
				uint dwRegType = 0;
				uint dwRegSize = 0;
				uint dwRegSize2 = 0;
				uint structureSize = 0;
				var propertyValueBuffer = IntPtr.Zero;
				uint errorStatus;
				uint loopCounter = 0;

				//First populate a list of plugged in devices (by specifying "DIGCF_PRESENT"), which are of the specified class GUID. 
				deviceInfoTable = SetupDiGetClassDevs(ref _interfaceClassGuid, IntPtr.Zero, IntPtr.Zero, DigcfPresent | DigcfDeviceinterface);

				if (deviceInfoTable != IntPtr.Zero)
				{
					//Now look through the list we just populated.  We are trying to see if any of them match our device. 
					while (true)
					{
						interfaceDataStructure.CbSize = (uint)Marshal.SizeOf(interfaceDataStructure);
						if (SetupDiEnumDeviceInterfaces(deviceInfoTable, IntPtr.Zero, ref _interfaceClassGuid, interfaceIndex, ref interfaceDataStructure))
						{
							errorStatus = (uint)Marshal.GetLastWin32Error();
							if (errorStatus == ErrorNoMoreItems) //Did we reach the end of the list of matching devices in the DeviceInfoTable?
							{   //Cound not find the device.  Must not have been attached.
								SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure we no longer need.
								return null;
							}
						}
						else    //Else some other kind of unknown error ocurred...
						{
							errorStatus = (uint)Marshal.GetLastWin32Error();
							SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure we no longer need.
							return null;
						}

						//Now retrieve the hardware ID from the registry.  The hardware ID contains the VID and PID, which we will then 
						//check to see if it is the correct device or not.

						//Initialize an appropriate SP_DEVINFO_DATA structure.  We need this structure for SetupDiGetDeviceRegistryProperty().
						devInfoData.CbSize = (uint)Marshal.SizeOf(devInfoData);
						SetupDiEnumDeviceInfo(deviceInfoTable, interfaceIndex, ref devInfoData);

						//First query for the size of the hardware ID, so we can know how big a buffer to allocate for the data.
						SetupDiGetDeviceRegistryProperty(deviceInfoTable, ref devInfoData, SpdrpHardwareid, ref dwRegType, IntPtr.Zero, 0, ref dwRegSize);

						//Allocate a buffer for the hardware ID.
						//Should normally work, but could throw exception "OutOfMemoryException" if not enough resources available.
						propertyValueBuffer = Marshal.AllocHGlobal((int)dwRegSize);

						//Retrieve the hardware IDs for the current device we are looking at.  PropertyValueBuffer gets filled with a 
						//REG_MULTI_SZ (array of null terminated strings).  To find a device, we only care about the very first string in the
						//buffer, which will be the "device ID".  The device ID is a string which contains the VID and PID, in the example 
						//format "Vid_04d8&Pid_003f".
						SetupDiGetDeviceRegistryProperty(deviceInfoTable, ref devInfoData, SpdrpHardwareid, ref dwRegType, propertyValueBuffer, dwRegSize, ref dwRegSize2);

						//Now check if the first string in the hardware ID matches the device ID of the USB device we are trying to find.
						var deviceIDFromRegistry = Marshal.PtrToStringUni(propertyValueBuffer); //Make a new string, fill it with the contents from the PropertyValueBuffer

						Marshal.FreeHGlobal(propertyValueBuffer);       //No longer need the PropertyValueBuffer, free the memory to prevent potential memory leaks

						//Now check if the hardware ID we are looking at contains the correct VID/PID (ignore upper/lower case)
						if (deviceIDFromRegistry != null && deviceIDFromRegistry.ToLowerInvariant().Contains(deviceIdSubstring))
						{
							//Device must have been found.  In order to open I/O file handle(s), we will need the actual device path first.
							//We can get the path by calling SetupDiGetDeviceInterfaceDetail(), however, we have to call this function twice:  The first
							//time to get the size of the required structure/buffer to hold the detailed interface data, then a second time to actually 
							//get the structure (after we have allocated enough memory for the structure.)
							detailedInterfaceDataStructure.CbSize = (uint)Marshal.SizeOf(detailedInterfaceDataStructure);
							//First call populates "StructureSize" with the correct value
							SetupDiGetDeviceInterfaceDetail(deviceInfoTable, ref interfaceDataStructure, IntPtr.Zero, 0, ref structureSize, IntPtr.Zero);
							//Need to call SetupDiGetDeviceInterfaceDetail() again, this time specifying a pointer to a SP_DEVICE_INTERFACE_DETAIL_DATA buffer with the correct size of RAM allocated.
							//First need to allocate the unmanaged buffer and get a pointer to it.
							var pUnmanagedDetailedInterfaceDataStructure = Marshal.AllocHGlobal((int)structureSize);
							detailedInterfaceDataStructure.CbSize = 6; //Initialize the cbSize parameter (4 bytes for DWORD + 2 bytes for unicode null terminator)
							Marshal.StructureToPtr(detailedInterfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, false); //Copy managed structure contents into the unmanaged memory buffer.

							//Now call SetupDiGetDeviceInterfaceDetail() a second time to receive the device path in the structure at pUnmanagedDetailedInterfaceDataStructure.
							if (SetupDiGetDeviceInterfaceDetail(deviceInfoTable, ref interfaceDataStructure, pUnmanagedDetailedInterfaceDataStructure, structureSize, IntPtr.Zero, IntPtr.Zero))
							{
								//Need to extract the path information from the unmanaged "structure".  The path starts at (pUnmanagedDetailedInterfaceDataStructure + sizeof(DWORD)).
								var pToDevicePath = new IntPtr((uint)pUnmanagedDetailedInterfaceDataStructure.ToInt32() + 4);  //Add 4 to the pointer (to get the pointer to point to the path, instead of the DWORD cbSize parameter)
								devicePath = Marshal.PtrToStringUni(pToDevicePath); //Now copy the path information into the globally defined DevicePath String.

								//Now check if the device path we are looking at contains the substring (ignore upper/lower case) 
								if (devicePath != null && devicePath.ToLowerInvariant().Contains(devicePathSubstring))
								{
									//We now have the proper device path, and we can finally use the path to open I/O handle(s) to the device.
									SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure we no longer need.
									Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
									return devicePath;    //Returning the device path
								}
							}
							else //Some unknown failure occurred
							{
								//errorCode = (uint)Marshal.GetLastWin32Error();
								SetupDiDestroyDeviceInfoList(deviceInfoTable);  //Clean up the old structure.
								Marshal.FreeHGlobal(pUnmanagedDetailedInterfaceDataStructure);  //No longer need this unmanaged SP_DEVICE_INTERFACE_DETAIL_DATA buffer.  We already extracted the path information.
								return null;
							}
						}

						interfaceIndex++;
						//Keep looping until we either find a device with matching VID and PID, or until we run out of devices to check.
						//However, just in case some unexpected error occurs, keep track of the number of loops executed.
						//If the number of loops exceeds a very large number, exit anyway, to prevent inadvertent infinite looping.
						loopCounter++;
						if (loopCounter == 10000)    //Surely there aren't more than 10'000 devices attached to any forseeable PC...
						{
							return null;
						}
					}//end of while(true)
				}
				return null;
			}//end of try
			catch
			{
				//Something went wrong if PC gets here.  Maybe a Marshal.AllocHGlobal() failed due to insufficient resources or something.
				return null;
			}
		} //findDevice


		[SuppressMessage("ReSharper", "FunctionNeverReturns")]
		public void UsbThread_DoWork(object sender, DoWorkEventArgs e)
		{
			var outBufferArray = new byte[65];
			var inBufferArray = new byte[65];
			var outBuffer = new UsbBuffer(outBufferArray);
			var inBuffer = new UsbBuffer(inBufferArray);
			uint bytesWritten = 0;
			uint bytesRead = 0;

			while (true)
			{
				//Do not try to use the read/write handles unless the USB device is attached and ready
				if (ConnectionStatus == UsbConnectionStatus.Connected)
				{

					// Raise SendPacket event if there are any subscribers
					//Ask the application if a packet should be sent and let it prepare the data to be sent
					RaiseSendPacketEvent?.Invoke(this, outBuffer);

					//Send packet if the application requested so
					if (outBuffer.RequestTransfer)
					{
						try
						{
							/*
                            byte[] buf = new byte[65];
                            buf[0] = OutBuffer.buffer[0];
                            buf[1] = OutBuffer.buffer[1];
                            buf[2] = OutBuffer.buffer[2];
                            */
							outBuffer.TransferSuccessful = WriteFile(_writeHandleToUSBDevice, outBufferArray, 65, ref bytesWritten, IntPtr.Zero);
						}
						catch
						{
							outBuffer.TransferSuccessful = false;
						}

						// A packet has been sent (or the transfer has failed)
						// Inform the application by raising a PacketSent event if there are any subscribers
						RaisePacketSentEvent?.Invoke(this, outBuffer);
					}

					// Raise ReceivePacket event if there are any subscribers
					// Ask the application if a packet should be requested
					RaiseReceivePacketEvent?.Invoke(this, inBuffer);

					// Receive packet if the application requested so
					if (inBuffer.RequestTransfer)
					{
						try
						{
							inBuffer.TransferSuccessful = ReadFileManagedBuffer(_readHandleToUSBDevice, inBufferArray, 65, ref bytesRead, IntPtr.Zero);
						}
						catch
						{
							inBuffer.TransferSuccessful = false;
						}

						// A packet has been received (or the transfer has failed)
						// Inform the application by raising a PacketReceived event if there are any subscribers
						RaisePacketReceivedEvent?.Invoke(this, inBuffer);
					}


				} // end of: if(AttachedState == true)
				else
				{
					Thread.Sleep(5); // Add a small delay to avoid unnecessary CPU utilization
				}
			} // end of while(true) loop
		} // end of ReadWriteThread_DoWork

		// WAS UNSAFE
		//--------------------------------------------------------------------------------------------------------------------------
		//FUNCTION:	ReadFileManagedBuffer()
		//PURPOSE:	Wrapper function to call ReadFile()
		//
		//INPUT:	Uses managed versions of the same input parameters as ReadFile() uses.
		//
		//OUTPUT:	Returns boolean indicating if the function call was successful or not.
		//          Also returns data in the byte[] INBuffer, and the number of bytes read. 
		//
		//Notes:    Wrapper function used to call the ReadFile() function.  ReadFile() takes a pointer to an unmanaged buffer and deposits
		//          the bytes read into the buffer.  However, can't pass a pointer to a managed buffer directly to ReadFile().
		//          This ReadFileManagedBuffer() is a wrapper function to make it so application code can call ReadFile() easier
		//          by specifying a managed buffer.
		//--------------------------------------------------------------------------------------------------------------------------
		public bool ReadFileManagedBuffer(SafeFileHandle hFile, byte[] inBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped)
		{
			var pInBuffer = IntPtr.Zero;

			try
			{
				pInBuffer = Marshal.AllocHGlobal((int)nNumberOfBytesToRead);    //Allocate some unmanged RAM for the receive data buffer.

				if (ReadFile(hFile, pInBuffer, nNumberOfBytesToRead, ref lpNumberOfBytesRead, lpOverlapped))
				{
					Marshal.Copy(pInBuffer, inBuffer, 0, (int)lpNumberOfBytesRead);    //Copy over the data from unmanged memory into the managed byte[] INBuffer
					Marshal.FreeHGlobal(pInBuffer);
					return true;
				}

				Marshal.FreeHGlobal(pInBuffer);
				return false;

			}
			catch
			{
				if (pInBuffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(pInBuffer);
				}
				return false;
			}
		}



		public void OpenDevice(String devicePath)
		{
			uint errorStatusWrite;
			uint errorStatusRead;
			// Close device first
			CloseDevice();
			// Open WriteHandle
			_writeHandleToUSBDevice = CreateFile(devicePath, GenericWrite, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);
			errorStatusWrite = (uint)Marshal.GetLastWin32Error();
			// Open ReadHandle
			_readHandleToUSBDevice = CreateFile(devicePath, GenericRead, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);
			errorStatusRead = (uint)Marshal.GetLastWin32Error();
			// Check if both handles were opened successfully
			if ((errorStatusWrite == ErrorSuccess) && (errorStatusRead == ErrorSuccess))
			{
				ConnectionStatus = UsbConnectionStatus.Connected;
			}
			else // For some reason the device was physically plugged in, but one or both of the read/write handles didn't open successfully
			{
				ConnectionStatus = UsbConnectionStatus.NotWorking;
				if (errorStatusWrite == ErrorSuccess)
				{
					_writeHandleToUSBDevice.Close();
				}

				if (errorStatusRead == ErrorSuccess)
				{
					_readHandleToUSBDevice.Close();
				}
			}
			// Raise event if there are any subscribers
			RaiseConnectionStatusChangedEvent?.Invoke(this, new ConnectionStatusEventArgs(ConnectionStatus));
			// Start async thread if connection has been established
			if (ConnectionStatus == UsbConnectionStatus.Connected)
			{
				//UsbThread.RunWorkerAsync();
			}
		}

		// Close connection to the USB device
		public void CloseDevice()
		{
			// Save current status
			var previousStatus = ConnectionStatus;
			// Close write and read handles if a device is connected
			if (ConnectionStatus == UsbConnectionStatus.Connected)
			{
				_writeHandleToUSBDevice.Close();
				_readHandleToUSBDevice.Close();
			}
			// Set status to disconnected
			ConnectionStatus = UsbConnectionStatus.Disconnected;
			// Stop async thread if connection has been established
			//UsbThread.CancelAsync();
			// Raise event if the status has changed and if there are any subscribers
			if (ConnectionStatus != previousStatus)
			{
				RaiseConnectionStatusChangedEvent?.Invoke(this, new ConnectionStatusEventArgs(ConnectionStatus));
			}
		}


	}//hid_utility

} //namespace hid