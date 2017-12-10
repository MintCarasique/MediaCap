using System;
using System.Runtime.InteropServices;
using DShowNET;
using DShowNET.Device;

namespace MediaCap.Capture
{
	/// <summary>
	/// Представляет собой DirectShow фильтр (например, устройство видеозахвата, кодеки).
	/// </summary>
	public class Filter : IComparable 
	{
		/// <summary> Читаемое имя фильтра</summary>
		public string Name;

		/// <summary> Уникальная строка-ссылка на этот фильтр. Данная строка может использоваться для воссоздания этого фильтра</summary>
		public string MonikerString;

		/// <summary> Create a new filter from its moniker string. </summary>
		public Filter(string monikerString)
		{
			Name = GetName(monikerString);
			MonikerString = monikerString;
		}

		/// <summary>Создает новый фильтр с помощью его moniker</summary>
		internal Filter(UCOMIMoniker moniker)
		{
			Name = GetName(moniker);
			MonikerString = GetMonikerString(moniker);
		}

		/// <summary> Получает отоброжаемое имя moniker. Это уникальная строка </summary>
		protected string GetMonikerString(UCOMIMoniker moniker)
		{
			string s;
			moniker.GetDisplayName(null, null, out s);
			return s;
		}

		/// <summary> Получает читаемое имя фильтра с помощью его moniker</summary>
		protected string GetName(UCOMIMoniker moniker)
		{
			object bagObj = null;
			IPropertyBag bag;
			try 
			{
				Guid bagId = typeof(IPropertyBag).GUID;
				moniker.BindToStorage(null, null, ref bagId, out bagObj);
				bag = (IPropertyBag) bagObj;
				object val = "";
				int hr = bag.Read( "FriendlyName", ref val, IntPtr.Zero );
				if( hr != 0 )
					Marshal.ThrowExceptionForHR( hr );
				string ret = val as string;
				if( (ret == null) || (ret.Length < 1) )
					throw new NotImplementedException( "Device FriendlyName" );
				return ret;
			}
			catch( Exception )
			{
				return( "" );
			}
			finally
			{
			    if(bagObj != null)
					Marshal.ReleaseComObject(bagObj);
			}
		}

		/// <summary> Get a moniker's human-readable name based on a moniker string. </summary>
		protected string GetName(string monikerString)
		{
			UCOMIMoniker parser = null; 
			UCOMIMoniker moniker = null;
			try
			{
				parser = GetAnyMoniker();
			    parser.ParseDisplayName( null, null, monikerString, out _, out moniker );
				return( GetName( parser ) );
			}
			finally
			{
				if ( parser != null )
					Marshal.ReleaseComObject( parser );
			    if ( moniker != null )
					Marshal.ReleaseComObject( moniker );
			}
		}

		/// <summary>
		///  Получает объект типа UCOMIMoniker. 
		///  Требуется хотя бы один видеокомпрессор в системе
		/// </summary>
		protected UCOMIMoniker GetAnyMoniker()
		{
			Guid category = FilterCategory.VideoCompressorCategory;
			int hr;
			object comObj = null;
			ICreateDevEnum enumDev;
			UCOMIEnumMoniker enumMon = null;
			UCOMIMoniker[] mon = new UCOMIMoniker[1];

			try 
			{
				//Получаем system device enumerator
				Type srvType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
				if(srvType == null)
					throw new NotImplementedException("System Device Enumerator");
				comObj = Activator.CreateInstance(srvType);
				enumDev = (ICreateDevEnum) comObj;

				//Создаем enumerator чтобы найти фильтры в категории
				hr = enumDev.CreateClassEnumerator(ref category, out enumMon, 0);
				if( hr != 0 )
					throw new NotSupportedException("No devices of the category");

				//Получаем первый фильтр
			    hr = enumMon.Next(1, mon, out _);
				if((hr != 0))
					mon[0] = null;

				return(mon[0]);
			}
			finally
			{
			    if(enumMon != null)
					Marshal.ReleaseComObject(enumMon);
			    if(comObj != null)
					Marshal.ReleaseComObject(comObj);
			}
		}
	
		/// <summary>
		///  Переопределенный метод для сравнения двух фильтров
		/// </summary>
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			Filter f = (Filter) obj;
			return String.Compare(Name, f.Name, StringComparison.Ordinal);
		}

	}
}
