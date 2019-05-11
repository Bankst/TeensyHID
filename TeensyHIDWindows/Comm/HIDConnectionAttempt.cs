namespace TeensyHIDWindows.Comm
{
    public class HIDConnectionAttempt
	{
		public HIDConnectionResult Result;
		public string DevicePath { get; }

		public HIDConnectionAttempt(string devicePath)
		{
			DevicePath = devicePath;
		}
	}
}
