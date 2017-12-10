using System.Runtime.InteropServices;
using DShowNET;

namespace MediaCap.Capture
{
    /// <summary>
    ///  Представляет собой физический коннектор или источник. Используется фильтрами,
    /// которые поддерживают интерфейс IAMCrossbar, например, TV-тюнерами
    /// </summary>
    public class CrossbarSource : Source
	{
		internal IAMCrossbar Crossbar;	//Кроссбар фильтр (COM объект)
		internal int OutputPin;	// Количество выходов кроссбара
		internal int InputPin;	//Количество входов кроссбара
		internal int RelatedInputPin = -1;	//Обычно представляет собой аудиовход
		internal CrossbarSource	RelatedInputSource;	// the Crossbar source associated with the RelatedInputPin
		internal PhysicalConnectorType ConnectorType; //Тип коннектора

        /// <summary> Активирует или деактивирует этот источник. </summary>
		public override bool Enabled
		{
			get 
			{
				int i;
				if (Crossbar.get_IsRoutedTo(OutputPin, out i) == 0)
					if (InputPin == i)
						return true;
				return false;
			}

			set
			{
				if (value)
				{
					// Активируем данный маршрут
					int hr = Crossbar.Route(OutputPin, InputPin);
					if ( hr < 0 )
                        Marshal.ThrowExceptionForHR(hr);

					// Активируем стандартный входной пин
					if ( RelatedInputSource != null )
					{
						hr = Crossbar.Route(RelatedInputSource.OutputPin, RelatedInputSource.InputPin);  
						if (hr < 0)
                            Marshal.ThrowExceptionForHR(hr);

					}
				}
				else
				{
					// Деактивируем данный маршрут направляя его на пин -1
					int hr = Crossbar.Route(OutputPin, -1);
					if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);

					// Деактивируем стандартный входной пин
					if ( RelatedInputSource != null )
					{
						hr = Crossbar.Route(RelatedInputSource.OutputPin, -1);  
						if (hr < 0)
                            Marshal.ThrowExceptionForHR(hr);

					}
				}
			}
		}
        
		internal CrossbarSource(IAMCrossbar crossbar, int outputPin, int inputPin, PhysicalConnectorType connectorType)
		{
			Crossbar = crossbar;
			OutputPin = outputPin;
			InputPin = inputPin;
			ConnectorType = connectorType;
			name = GetName(connectorType);
		}

		/// <summary> Constructor. This class cannot be created directly. </summary>
		internal CrossbarSource(IAMCrossbar crossbar, int outputPin, int inputPin, int relatedInputPin, PhysicalConnectorType connectorType)
		{
			Crossbar = crossbar;
			OutputPin = outputPin;
			InputPin = inputPin;
			RelatedInputPin = relatedInputPin; 
			ConnectorType = connectorType;
			name = GetName(connectorType);
		}

		/// <summary>Возвращает удобное для чтения название коннектора</summary>
		private string GetName(PhysicalConnectorType connectorType)
		{
			string name;
			switch( connectorType )
			{
				case PhysicalConnectorType.Video_Tuner:				name = "Video Tuner";			break;
				case PhysicalConnectorType.Video_Composite:			name = "Video Composite";		break;
				case PhysicalConnectorType.Video_SVideo:			name = "Video S-Video";			break;
				case PhysicalConnectorType.Video_RGB:				name = "Video RGB";				break;
				case PhysicalConnectorType.Video_YRYBY:				name = "Video YRYBY";			break;
				case PhysicalConnectorType.Video_SerialDigital:		name = "Video Serial Digital";	break;
				case PhysicalConnectorType.Video_ParallelDigital:	name = "Video Parallel Digital";break;
				case PhysicalConnectorType.Video_SCSI:				name = "Video SCSI";			break;
				case PhysicalConnectorType.Video_AUX:				name = "Video AUX";				break;
				case PhysicalConnectorType.Video_1394:				name = "Video Firewire";		break;
				case PhysicalConnectorType.Video_USB:				name = "Video USB";				break;
				case PhysicalConnectorType.Video_VideoDecoder:		name = "Video Decoder";			break;
				case PhysicalConnectorType.Video_VideoEncoder:		name = "Video Encoder";			break;
				case PhysicalConnectorType.Video_SCART:				name = "Video SCART";			break;

				case PhysicalConnectorType.Audio_Tuner:				name = "Audio Tuner";			break;
				case PhysicalConnectorType.Audio_Line:				name = "Audio Line In";			break;
				case PhysicalConnectorType.Audio_Mic:				name = "Audio Mic";				break;
				case PhysicalConnectorType.Audio_AESDigital:		name = "Audio AES Digital";		break;
				case PhysicalConnectorType.Audio_SPDIFDigital:		name = "Audio SPDIF Digital";	break;
				case PhysicalConnectorType.Audio_SCSI:				name = "Audio SCSI";			break;
				case PhysicalConnectorType.Audio_AUX:				name = "Audio AUX";				break;
				case PhysicalConnectorType.Audio_1394:				name = "Audio Firewire";		break;
				case PhysicalConnectorType.Audio_USB:				name = "Audio USB";				break;
				case PhysicalConnectorType.Audio_AudioDecoder:		name = "Audio Decoder";			break;

				default:											name = "Unknown Connector";		break;
			}
			return name;
		}
	}
}
