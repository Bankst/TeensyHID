namespace TeensyHID.HID
{
	public class HidDeviceAttributes
	{
		internal HidDeviceAttributes(NativeMethods.HIDD_ATTRIBUTES attributes)
		{
			VendorId = attributes.VendorID;
			ProductId = attributes.ProductID;
			Version = attributes.VersionNumber;

			VendorHexId = $"0x{attributes.VendorID:X4}";
			ProductHexId = $"0x{attributes.ProductID:X4}";
		}

		public int    VendorId     { get; }
		public int    ProductId    { get; }
		public int    Version      { get; }
		public string VendorHexId  { get; set; }
		public string ProductHexId { get; set; }
	}
}
