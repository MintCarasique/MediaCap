using System;

namespace MediaCap.Capture
{
	/// <summary>
	///  Exception thrown when the device cannot be rendered or started.
	/// </summary>
	public class DeviceInUseException : SystemException
	{
        /// <summary>
        /// Initializes a new instance with the specified HRESULT
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="hResult"></param>
		public DeviceInUseException(string deviceName, int hResult) : base( deviceName + " is in use or cannot be rendered. (" + hResult + ")" )
		{
		}
	}
}
