using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DShowNET;

namespace MediaCap.Capture
{
	/// <summary>
	///  Возможности аудиоустройства, такие как минимальный и максимальный битрейты, а также доступное количество каналов
	/// </summary>
	public class AudioCapabilities
	{
		/// <summary> Минимальное число аудиоканалов. </summary>
		public int MinimumChannels { get; private set; }

		/// <summary> Максимальное число аудиоканалов. </summary>
		public int MaximumChannels { get; private set; }

		/// <summary> Granularity of the channels. For example, channels 2 through 4, in steps of 2. </summary>
		public int ChannelsGranularity { get; private set; }

		/// <summary> Минимальный битрейт аудио. </summary>
		public int MinimumSampleSize { get; private set; }

		/// <summary> Максимальный битрейт аудио. </summary>
		public int MaximumSampleSize { get; private set; }

		/// <summary> Granularity of the bits per sample. For example, 8 bits per sample through 32 bits per sample, in steps of 8. </summary>
		public int SampleSizeGranularity { get; private set; }

		/// <summary> Minimum sample frequency. </summary>
		public int MinimumSamplingRate { get; private set; }

		/// <summary> Maximum sample frequency. </summary>
		public int MaximumSamplingRate { get; private set; }

		/// <summary> Granularity of the frequency. For example, 11025 Hz to 44100 Hz, in steps of 11025 Hz. </summary>
		public int SamplingRateGranularity { get; private set; }

		/// <summary> Запрашивает возможности аудиоустройства </summary>
		internal AudioCapabilities(IAMStreamConfig audioStreamConfig)
		{
			if (audioStreamConfig == null) 
				throw new ArgumentNullException(nameof(audioStreamConfig));

			AMMediaType mediaType = null;
			AudioStreamConfigCaps caps = null;
			IntPtr pCaps = IntPtr.Zero;
			IntPtr pMediaType;
			try
			{
				//Проверка того, что устройство может выдавать свои аудиовозможности
				int c, size;
				int hr = audioStreamConfig.GetNumberOfCapabilities(out c, out size);
				if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
				if (c <= 0) 
					throw new NotSupportedException( "This audio device does not report capabilities." );
				if (size > Marshal.SizeOf(typeof(AudioStreamConfigCaps)))
				{
					throw new NotSupportedException( "Unable to retrieve audio device capabilities. This audio device requires a larger AudioStreamConfigCaps structure." );
				}
				if (c > 1)
					Debug.WriteLine("WARNING: This audio device supports " + c + " capability structures. Only the first structure will be used." );

				//Выделение памяти для структуры
				pCaps = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AudioStreamConfigCaps))); 

				// Запрос первой структуры с аудиовозможностями
				hr = audioStreamConfig.GetStreamCaps(0, out pMediaType, pCaps);
				if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);

				//Каст указателей в управляемые ресурсы
				mediaType = (AMMediaType) Marshal.PtrToStructure(pMediaType, typeof(AMMediaType));
				caps = (AudioStreamConfigCaps) Marshal.PtrToStructure(pCaps, typeof(AudioStreamConfigCaps));

				// Извлечение информации
				MinimumChannels	= caps.MinimumChannels;
				MaximumChannels	= caps.MaximumChannels;
				ChannelsGranularity	= caps.ChannelsGranularity;
				MinimumSampleSize = caps.MinimumBitsPerSample;
				MaximumSampleSize = caps.MaximumBitsPerSample;
				SampleSizeGranularity = caps.BitsPerSampleGranularity;
				MinimumSamplingRate	= caps.MinimumSampleFrequency;
				MaximumSamplingRate	= caps.MaximumSampleFrequency;
				SamplingRateGranularity	= caps.SampleFrequencyGranularity;
				
			}
			finally
			{
				if (pCaps != IntPtr.Zero)
					Marshal.FreeCoTaskMem(pCaps);
                pCaps = IntPtr.Zero;
				if (mediaType != null)
					DsUtils.FreeAMMediaType(mediaType);
                mediaType = null;
			}
		}
	}
}
