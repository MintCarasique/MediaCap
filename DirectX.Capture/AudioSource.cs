using System;
using System.Runtime.InteropServices;
using DShowNET;

namespace MediaCap.Capture
{
	/// <summary>
	/// ������������ ����� ���������� ��������� ��� �������� ���������������. ������������ ���������,
	/// ������� ������������ ��������� IAMAudioInputMixer, ��������, ������������
	/// </summary>
	public class AudioSource : Source
	{
		internal IPin Pin;			// ��������� ����� ������� (COM ������)
        
		internal AudioSource(IPin pin)
		{
			if ((pin as IAMAudioInputMixer) == null)
				throw new NotSupportedException("The input pin does not support the IAMAudioInputMixer interface");
			Pin = pin;
			name = getName(pin);
		}
        
		/// <summary> 
		/// �������� ��� ��������� ���� ��������. ��� ���������� ����� �������� ������������� ���������� ��������������, �� �� ������������
		/// ����� ��� ��� �������� �������� �����, ���������� �������������� ��� ���������.
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
        
		/// <summary>���������� ������� ��� ������ ��� ����������. </summary>
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

            //��������� PinInfo �������� ������ �� IBaseFilter, ������� ���������� ���������� ��, ����� ������������� ������
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
