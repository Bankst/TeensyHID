﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace TeensyHIDWindows.Hid
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class HidReport
	{
		private byte   _reportId;
		private byte[] _data = { };

		public HidReport(int reportSize)
		{
			Array.Resize(ref _data, reportSize - 1);
		}

		public HidReport(int reportSize, HidDeviceData deviceData)
		{
			ReadStatus = deviceData.Status;

			Array.Resize(ref _data, reportSize - 1);

			if ((deviceData.Data != null))
			{

				if (deviceData.Data.Length > 0)
				{
					_reportId = deviceData.Data[0];
					Exists = true;

					if (deviceData.Data.Length <= 1) return;

					var dataLength = reportSize - 1;
					if (deviceData.Data.Length < reportSize - 1) dataLength = deviceData.Data.Length;
					Array.Copy(deviceData.Data, 1, _data, 0, dataLength);
				}
				else Exists = false;
			}
			else Exists = false;
		}

		public bool                     Exists     { get; private set; }
		public HidDeviceData.ReadStatus ReadStatus { get; }

		public byte ReportId
		{
			get => _reportId;
			set
			{
				_reportId = value;
				Exists = true;
			}
		}

		public byte[] Data
		{
			get => _data;
			set
			{
				_data = value;
				Exists = true;
			}
		}

		public byte[] GetBytes()
		{
			byte[] data = null;
			Array.Resize(ref data, _data.Length + 1);
			data[0] = _reportId;
			Array.Copy(_data, 0, data, 1, _data.Length);
			return data;
		}
	}
}
