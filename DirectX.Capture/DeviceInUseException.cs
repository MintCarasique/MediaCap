using System;

namespace MediaCap.Capture
{
	/// <summary>
	/// Исключение, вызываемое в случае, если устройство не запускается, или не происходит рендеринг
	/// </summary>
	public class DeviceInUseException : SystemException
	{
		public DeviceInUseException(string deviceName, int hResult) : base( deviceName + " is in use or cannot be rendered. (" + hResult + ")" )
		{
		}
	}
}
