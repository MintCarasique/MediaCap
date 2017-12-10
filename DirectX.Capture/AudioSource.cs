using System;
using System.Runtime.InteropServices;
using DShowNET;

namespace MediaCap.Capture
{
	/// <summary>
	/// Представляет собой физический коннектор или источник аудиоустройства. Используется фильтрами,
	/// которые поддерживают интерфейс IAMAudioInputMixer, например, аудиокартами
	/// </summary>
	public class AudioSource : Source
	{
		internal IPin Pin;			// интерфейс аудио микшера (COM объект)
        
		internal AudioSource(IPin pin)
		{
			if ((pin as IAMAudioInputMixer) == null)
				throw new NotSupportedException("The input pin does not support the IAMAudioInputMixer interface");
			Pin = pin;
			name = getName(pin);
		}
        
		/// <summary> 
		/// Включает или отключает этот источник. Для источников аудио возможно использование нескольких аудиоустройств, но не одновременно
		/// Перед тем как включить источник звука, необходимо деактивировать все остальные.
		///  </summary>
		public override bool Enabled
		{
			get 
			{
				IAMAudioInputMixer mix = (IAMAudioInputMixer) Pin;
				bool e;
				try
				{
					mix.get_Enable(out e);
					return e;
				}
				catch
				{
					return false;
				}
			}

			set
			{
				IAMAudioInputMixer mix = (IAMAudioInputMixer) Pin;
				mix.put_Enable(value);
			}

		}
        
		/// <summary>Возвращает удобное для чтения имя коннектора. </summary>
		private string getName( IPin pin )
		{
			string s = "Unknown pin";
			PinInfo pinInfo = new PinInfo();

			// Direction matches, so add pin name to listbox
			int hr = pin.QueryPinInfo(out pinInfo);
			if (hr == 0)
			{ 
				s = pinInfo.name + "";
			}
			else
				Marshal.ThrowExceptionForHR(hr);

            //Структура PinInfo содержит ссылку на IBaseFilter, поэтому необходимо освободить ее, чтобы предотвратить утечку
			if (pinInfo.filter != null)
				Marshal.ReleaseComObject(pinInfo.filter);
            pinInfo.filter  = null;
			return(s);
		}

		public override void Dispose()
		{
			if (Pin != null)
				Marshal.ReleaseComObject(Pin);
			Pin = null;
			base.Dispose();
		}	
	}
}
