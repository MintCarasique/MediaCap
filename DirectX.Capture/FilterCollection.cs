using System;
using System.Collections;
using System.Runtime.InteropServices;
using DShowNET;
using DShowNET.Device;

namespace MediaCap.Capture
{
	/// <summary>
	///	 A collection of Filter objects (DirectShow filters).
	///	 This is used by the <see cref="Capture"/> class to provide
	///	 lists of capture devices and compression filters. This class
	///	 cannot be created directly.
	/// </summary>
	public class FilterCollection : CollectionBase
	{
		/// <summary> Populate the collection with a list of filters from a particular category. </summary>
		internal FilterCollection(Guid category)
		{
			GetFilters( category );
		}

		/// <summary> Populate the InnerList with a list of filters from a particular category </summary>
		protected void GetFilters(Guid category)
		{
			int					hr;
			object				comObj = null;
			ICreateDevEnum		enumDev;
			UCOMIEnumMoniker	enumMon = null;
			UCOMIMoniker[]		mon = new UCOMIMoniker[1];

			try 
			{
				// Get the system device enumerator
				Type srvType = Type.GetTypeFromCLSID( Clsid.SystemDeviceEnum );
				if( srvType == null )
					throw new NotImplementedException( "System Device Enumerator" );
				comObj = Activator.CreateInstance( srvType );
				enumDev = (ICreateDevEnum) comObj;

				// Create an enumerator to find filters in category
				hr = enumDev.CreateClassEnumerator( ref category, out enumMon, 0 );
				if( hr != 0 )
				{
					if(category == FilterCategory.VideoInputDevice)
					{
						throw new NotSupportedException( "No devices of the category VideoInputDevice" );
					}
					else
						if(category == FilterCategory.AudioInputDevice)
					{
						throw new NotSupportedException( "No devices of the category AudioInputDevice" );
					}
					else
						if(category == FilterCategory.VideoCompressorCategory)
					{
						throw new NotSupportedException( "No devices of the category VideoCompressorCategory" );
					}
					else
						if(category == FilterCategory.AudioCompressorCategory)
					{
						throw new NotSupportedException( "No devices of the category AudioCompressorCategory" );
					}
					else
					{
						throw new NotSupportedException( "No devices of the category " + category );
					}
				}

				// Loop through the enumerator
			    do
				{
					// Next filter
					hr = enumMon.Next( 1, mon, out _ );
					if( (hr != 0) || (mon[0] == null) )
						break;
					
					// Add the filter
					Filter filter = new Filter( mon[0] );
					InnerList.Add( filter );

					// Release resources
					Marshal.ReleaseComObject( mon[0] );
					mon[0] = null;
				}
				while(true);

				// Sort
				InnerList.Sort();
			}
			finally
			{
			    if( mon[0] != null )
					Marshal.ReleaseComObject( mon[0] ); mon[0] = null;
				if( enumMon != null )
					Marshal.ReleaseComObject( enumMon );
			    if( comObj != null )
					Marshal.ReleaseComObject( comObj );
			}
		}

		/// <summary> Get the filter at the specified index. </summary>
		public Filter this[int index] => (Filter) InnerList[index];
	}
}
