using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DShowNET;

namespace MediaCap.Capture
{

	/// <summary>
	///  Use the Capture class to capture audio and video to AVI files.
	/// </summary>
	/// <remarks>
	///  This is the core class of the Capture Class Library. The following 
	///  sections introduce the Capture class and how to use this library.
	///  
	/// <br/><br/>
	/// <para><b>Basic Usage</b></para>
	/// 
	/// <para>
	///  The Capture class only requires a video device and/or audio device
	///  to begin capturing. The <see cref="Filters"/> class provides
	///  lists of the installed video and audio devices. 
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  // Remember to add a reference to DirectX.Capture.dll
	///  using DirectX.Capture
	///  ...
	///  Capture capture = new Capture( Filters.VideoInputDevices[0], 
	///                                 Filters.AudioInputDevices[0] );
	///  capture.Start();
	///  ...
	///  capture.Stop();
	/// </div></code>
	/// <para>
	///  This will capture video and audio using the first video and audio devices
	///  installed on the system. To capture video only, pass a null as the second
	///  parameter of the constructor.
	/// </para>
	/// <para> 
	///  The class is initialized to a valid temporary file in the Windows temp
	///  folder. To capture to a different file, set the 
	///  <see cref="Capture.Filename"/> property before you begin
	///  capturing. Remember to add DirectX.Capture.dll to 
	///  your project references.
	/// </para>
	///
	/// <br/>
	/// <para><b>Setting Common Properties</b></para>
	/// 
	/// <para>
	///  The example below shows how to change video and audio settings. 
	///  Properties such as <see cref="Capture.FrameRate"/> and 
	///  <see cref="AudioSampleSize"/> allow you to programmatically adjust
	///  the capture. Use <see cref="Capture.VideoCaps"/> and 
	///  <see cref="Capture.AudioCaps"/> to determine valid values for these
	///  properties.
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  Capture capture = new Capture( Filters.VideoInputDevices[0], 
	///                                 Filters.AudioInputDevices[1] );
	///  capture.VideoCompressor = Filters.VideoCompressors[0];
	///  capture.AudioCompressor = Filters.AudioCompressors[0];
	///  capture.FrameRate = 29.997;
	///  capture.FrameSize = new Size( 640, 480 );
	///  capture.AudioSamplingRate = 44100;
	///  capture.AudioSampleSize = 16;
	///  capture.Filename = "C:\MyVideo.avi";
	///  capture.Start();
	///  ...
	///  capture.Stop();
	/// </div></code>
	/// <para>
	///  The example above also shows the use of video and audio compressors. In most 
	///  cases you will want to use compressors. Uncompressed video can easily
	///  consume over a 1GB of disk space per minute. Whenever possible, set 
	///  the <see cref="Capture.VideoCompressor"/> and <see cref="Capture.AudioCompressor"/>
	///  properties as early as possible. Changing them requires the internal filter
	///  graph to be rebuilt which often causes most of the other properties to
	///  be reset to default values.
	/// </para>
	///
	/// <br/>
	/// <para><b>Listing Devices</b></para>
	/// 
	/// <para>
	///  Use the <see cref="Filters.VideoInputDevices"/> collection to list
	///  video capture devices installed on the system.
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  foreach ( Filter f in Filters.VideoInputDevices )
	///  {
	///		Debug.WriteLine( f.Name );
	///  }
	/// </div></code>
	/// The <see cref="Filters"/> class also provides collections for audio 
	/// capture devices, video compressors and audio compressors.
	///
	/// <br/>
	/// <para><b>Preview</b></para>
	/// 
	/// <para>
	///  Video preview is controled with the <see cref="Capture.PreviewWindow"/>
	///  property. Setting this property to a visible control will immediately 
	///  begin preview. Set to null to stop the preview. 
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  // Enable preview
	///  capture.PreviewWindow = myPanel;
	///  // Disable preview
	///  capture.PreviewWindow = null;
	/// </div></code>
	/// <para>
	///  The control used must have a window handle (HWND), good controls to 
	///  use are the Panel or the form itself.
	/// </para>
	/// <para>
	///  Retrieving or changing video/audio settings such as FrameRate, 
	///  FrameSize, AudioSamplingRate, and AudioSampleSize will cause
	///  the preview window to flash. This is beacuse the preview must be
	///  temporarily stopped. Disable the preview if you need to access
	///  several properties at the same time.
	/// </para>
	/// 
	/// <br/>
	/// <para><b>Property Pages</b></para>
	///  
	/// <para>
	///  Property pages exposed by the devices and compressors are
	///  available through the <see cref="Capture.PropertyPages"/> 
	///  collection.
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  // Display the first property page
	///  capture.PropertyPages[0].Show();
	/// </div></code>
	/// <para>
	///  The property pages will often expose more settings than
	///  the Capture class does directly. Some examples are brightness, 
	///  color space, audio balance and bass boost. The disadvantage
	///  to using the property pages is the user's choices cannot be
	///  saved and later restored. The exception to this is the video
	///  and audio compressor property pages. Most compressors support
	///  the saving and restoring state, see the 
	///  <see cref="PropertyPage.State"/> property for more information.
	/// </para>
	/// <para>
	///  Changes made in the property page will be reflected 
	///  immediately in the Capture class properties (e.g. Capture.FrameSize).
	///  However, the reverse is not always true. A change made directly to
	///  FrameSize, for example, may not be reflected in the associated
	///  property page. Fortunately, the filter will use requested FrameSize
	///  even though the property page shows otherwise.
	/// </para>
	/// 
	/// <br/>
	/// <para><b>Saving and Restoring Settings</b></para>
	///  
	/// <para>
	///  To save the user's choice of devices and compressors, 
	///  save <see cref="Filter.MonikerString"/> and user it later
	///  to recreate the Filter object.
	/// </para>
	/// <para>
	///  To save a user's choices from a property page use
	///  <see cref="PropertyPage.State"/>. However, only the audio
	///  and video compressor property pages support this.
	/// </para>
	/// <para>
	///  The last items to save are the video and audio settings such
	///  as FrameSize and AudioSamplingRate. When restoring, remember
	///  to restore these properties after setting the video and audio
	///  compressors.
	/// </para>
	/// <code><div style="background-color:whitesmoke;">
	///  // Disable preview
	///  capture.PreviewWindow = null;
	///  
	///  // Save settings
	///  string videoDevice = capture.VideoDevice.MonikerString;
	///  string audioDevice = capture.AudioDevice.MonikerString;
	///  string videoCompressor = capture.VideoCompressor.MonikerString;
	///  string audioCompressor = capture.AudioCompressor.MonikerString;
	///  double frameRate = capture.FrameRate;
	///  Size frameSize = capture.FrameSize;
	///  short audioChannels = capture.AudioChannels;
	///  short audioSampleSize = capture.AudioSampleSize;
	///  int audioSamplingRate = capture.AudioSamplingRate;
	///  ArrayList pages = new ArrayList();
	///  foreach ( PropertyPage p in capture.PropertyPages )
	///  {
	///		if ( p.SupportsPersisting )
	///			pages.Add( p.State );
	///	}
	///			
	///  
	///  // Restore settings
	///  Capture capture = new Capture( new Filter( videoDevice), 
	///				new Filter( audioDevice) );
	///  capture.VideoCompressor = new Filter( videoCompressor );
	///  capture.AudioCompressor = new Filter( audioCompressor );
	///  capture.FrameRate = frameRate;
	///  capture.FrameSize = frameSize;
	///  capture.AudioChannels = audioChannels;
	///  capture.AudioSampleSize = audioSampleSize;
	///  capture.AudioSamplingRate = audioSamplingRate;
	///  foreach ( PropertyPage p in capture.PropertyPages )
	///  {
	///		if ( p.SupportsPersisting )
	///		{
	///			p.State = (byte[]) pages[0]
	///			pages.RemoveAt( 0 );
	///		}
	///	 }
	///  // Enable preview
	///  capture.PreviewWindow = myPanel;
	/// </div></code>
	///  
	/// <br/>
	/// 
	/// <br/>
	/// <para><b>Troubleshooting</b></para>
	/// 
	/// <para>
	///  This class library uses COM Interop to access the full
	///  capabilities of DirectShow, so if there is another
	///  application that can successfully use a hardware device
	///  then it should be possible to modify this class library
	///  to use the device.
	/// </para>
	/// <para>
	///  Try the <b>AMCap</b> sample from the DirectX SDK 
	///  (DX9\Samples\C++\DirectShow\Bin\AMCap.exe) or 
	///  <b>Virtual VCR</b> from http://www.DigTV.ws 
	/// </para>
	/// 
	/// <br/>
	/// <para><b>Credits</b></para>
	/// 
	/// <para>
	///  This class library would not be possible without the
	///  DirectShowLib project by NETMaster: 
	///  http://www.codeproject.com/useritems/directshownet.asp
	/// </para>
	/// <para>
	///  Documentation is generated by nDoc available at
	///  http://ndoc.sourceforge.net
	/// </para>
	/// 
	/// <br/>
	/// <para><b>Feedback</b></para>
	/// 
	///  Feel free to send comments and questions to me at
	///  mportobello@hotmail.com. If the the topic may be of interest
	///  to others, post your question on the www.codeproject.com
	///  page for DirectX.Capture.
	/// </remarks>
	public partial class Capture : ISampleGrabberCB
    {

		// ------------------ Private Enumerations --------------------

		/// <summary> Possible states of the interal filter graph </summary>
		public enum GraphState
		{
			Null,			// No filter graph at all
			Created,		// Filter graph created with device filters added
			Rendered,		// Filter complete built, ready to run (possibly previewing)
			Capturing		// Filter is capturing
        }

		/// <summary>
		/// Recording file mode type enumerations
		/// </summary>
		public enum RecFileModeType
		{
			/// <summary> Avi video (+audio) </summary>
			Avi
		}

        // ------------------ Public Properties --------------------

		/// <summary> Is the class currently capturing. Read-only. </summary>
		public bool Capturing => graphState==GraphState.Capturing;

        /// <summary> Has the class been cued to begin capturing. Read-only. </summary>
		public bool Cued => isCaptureRendered && graphState==GraphState.Rendered;

        /// <summary> Is the class currently stopped. Read-only. </summary>
		public bool Stopped => graphState != GraphState.Capturing;

        /// <summary> 
		///  Name of file to capture to. Initially set to
		///  a valid temporary file.
		/// </summary>		
		/// <remarks>
		///  If the file does not exist, it will be created. If it does 
		///  exist, it will be overwritten. An overwritten file will 
		///  not be shortened if the captured data is smaller than the 
		///  original file. The file will be valid, it will just contain 
		///  extra, unused, data after the audio/video data. 
		/// 
		/// <para>
		///  A future version of this class will provide a method to copy 
		///  only the valid audio/video data to a new file. </para>
		/// 
		/// <para>
		///  This property cannot be changed while capturing or cued. </para>
		/// </remarks> 
		
		public string Filename 
		{ 
			get => ( filename );
            set 
			{ 
				assertStopped();
				if ( Cued )
					throw new InvalidOperationException( "The Filename cannot be changed once cued. Use Stop() before changing the filename." );
				filename = value; 
				if ( fileWriterFilter != null )
				{
					string s;
					AMMediaType mt = new AMMediaType(); 
					int hr = fileWriterFilter.GetCurFile( out s, mt );
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
					if ( mt.formatSize > 0 )
						Marshal.FreeCoTaskMem( mt.formatPtr ); 
					hr = fileWriterFilter.SetFileName( filename, mt );
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				}
			} 
		}

		/// <summary>
		/// Recording file modes
		/// </summary>
		public RecFileModeType RecFileMode
		{
			get => recFileMode;
		    set
			{
				if(graphState == GraphState.Capturing)
				{
					// Value may not be changed now
					return;
				}

				recFileMode = value;
                
				// Change filename extension
				filename = Path.ChangeExtension(filename, RecFileMode.ToString().ToLower());
			}
		}
        
        /// <summary>
		///  The control that will host the preview window. 
		/// </summary>
		/// <remarks>
		///  Setting this property will begin video preview
		///  immediately. Set this property after setting all
		///  other properties to avoid unnecessary changes
		///  to the internal filter graph (some properties like
		///  FrameSize require the internal filter graph to be 
		///  stopped and disconnected before the property
		///  can be retrieved or set).
		///  
		/// <para>
		///  To stop video preview, set this property to null. </para>
		/// </remarks>
		public Control PreviewWindow
		{
			get => previewWindow;
            set
			{
				assertStopped();
				DerenderGraph();
				previewWindow = value;
				wantPreviewRendered = ( ( previewWindow != null ) && ( videoDevice != null ) );
				RenderGraph();
				StartPreviewIfNeeded();
			}
		}


		/// <summary>
		///  The capabilities of the video device.
		/// </summary>
		/// <remarks>
		///  It may be required to cue the capture (see <see cref="Cue"/>) 
		///  before all capabilities are correctly reported. If you 
		///  have such a device, the developer would be interested to
		///  hear from you.
		/// 
		/// <para>
		///  The information contained in this property is retrieved and
		///  cached the first time this property is accessed. Future
		///  calls to this property use the cached results. This was done 
		///  for performance. </para>
		///  
		/// <para>
		///  However, this means <b>you may get different results depending 
		///  on when you access this property first</b>. If you are experiencing 
		///  problems, try accessing the property immediately after creating 
		///  the Capture class or immediately after setting the video and 
		///  audio compressors. Also, inform the developer. </para>
		/// </remarks>
		public VideoCapabilities VideoCaps 
		{ 
			get 
			{ 
				if ( videoCaps == null )
				{
					if ( videoStreamConfig != null )
					{
						try 
						{
							videoCaps = new VideoCapabilities( videoStreamConfig ); 
						}
						catch ( Exception ex ) { Debug.WriteLine( "VideoCaps: unable to create videoCaps." + ex ); }
					}
				}
				return videoCaps; 
			}
		}

		/// <summary>
		///  The capabilities of the audio device.
		/// </summary>
		/// <remarks>
		///  It may be required to cue the capture (see <see cref="Cue"/>) 
		///  before all capabilities are correctly reported. If you 
		///  have such a device, the developer would be interested to
		///  hear from you.
		/// 
		/// <para>
		///  The information contained in this property is retrieved and
		///  cached the first time this property is accessed. Future
		///  calls to this property use the cached results. This was done 
		///  for performance. </para>
		///  
		/// <para>
		///  However, this means <b>you may get different results depending 
		///  on when you access this property first</b>. If you are experiencing 
		///  problems, try accessing the property immediately after creating 
		///  the Capture class or immediately after setting the video and 
		///  audio compressors. Also, inform the developer. </para>
		/// </remarks>
		public AudioCapabilities AudioCaps 
		{ 
			get 
			{ 
				if ( audioCaps == null )
				{
					if ( audioStreamConfig != null )
					{
						try 
						{ 
							audioCaps = new AudioCapabilities( audioStreamConfig ); 
						}
						catch ( Exception ex ) { Debug.WriteLine( "AudioCaps: unable to create audioCaps." + ex ); }
					}
				}
				return audioCaps; 
			} 
		}

		/// <summary> 
		///  The video capture device filter. Read-only. To use a different 
		///  device, dispose of the current Capture instance and create a new 
		///  instance with the desired device. 
		/// </summary>
		public Filter VideoDevice => videoDevice;

        /// <summary> 
		///  The audio capture device filter. Read-only. To use a different 
		///  device, dispose of the current Capture instance and create a new 
		///  instance with the desired device. 
		/// </summary>
		public Filter AudioDevice => audioDevice;

        /// <summary> 
		///  The video compression filter. When this property is changed 
		///  the internal filter graph is rebuilt. This means that some properties
		///  will be reset. Set this property as early as possible to avoid losing 
		///  changes. This property cannot be changed while capturing.
		/// </summary>
		public Filter VideoCompressor { 
			get => videoCompressor;
            set 
			{ 
				assertStopped();
				DestroyGraph();
				videoCompressor = value;
				RenderGraph();
                StartPreviewIfNeeded();
			}
		}

		/// <summary> 
		///  The audio compression filter. 
		/// </summary>
		/// <remarks>
		///  When this property is changed 
		///  the internal filter graph is rebuilt. This means that some properties
		///  will be reset. Set this property as early as possible to avoid losing 
		///  changes. This property cannot be changed while capturing.
		/// </remarks>
		public Filter AudioCompressor 
		{ 
			get => audioCompressor;
		    set 
			{ 
				assertStopped();
				DestroyGraph();
				audioCompressor = value;
				RenderGraph();
				StartPreviewIfNeeded();
			}
		}

		/// <summary> 
		///  The current video source. Use Capture.VideoSources to 
		///  list available sources. Set to null to disable all 
		///  sources (mute).
		/// </summary>
		public Source VideoSource 
		{ 
			get => VideoSources.CurrentSource;
		    set => VideoSources.CurrentSource = value;
		}

		/// <summary> 
		///  The current audio source. Use Capture.AudioSources to 
		///  list available sources. Set to null to disable all 
		///  sources (mute).
		/// </summary>
		public Source AudioSource 
		{
			get => AudioSources.CurrentSource;
		    set => AudioSources.CurrentSource = value;
		}

		/// <summary> 
		///  Collection of available video sources/physical connectors 
		///  on the current video device. 
		/// </summary>
		/// <remarks>
		///  In most cases, if the device has only one source, 
		///  this collection will be empty. 
		/// 
		/// <para>
		///  The information contained in this property is retrieved and
		///  cached the first time this property is accessed. Future
		///  calls to this property use the cached results. This was done 
		///  for performance. </para>
		///  
		/// <para>
		///  However, this means <b>you may get different results depending 
		///  on when you access this property first</b>. If you are experiencing 
		///  problems, try accessing the property immediately after creating 
		///  the Capture class or immediately after setting the video and 
		///  audio compressors. Also, inform the developer. </para>
		/// </remarks>
		public SourceCollection VideoSources
		{
			get 
			{ 
				if ( videoSources == null )
				{
					try
					{
						if ( videoDevice != null )
							videoSources = new SourceCollection( captureGraphBuilder, videoDeviceFilter, true );
						else
							videoSources = new SourceCollection();
					}
					catch ( Exception ex ) { Debug.WriteLine( "VideoSources: unable to create VideoSources." + ex ); }
				}
				return videoSources;
			}
            set
            {
                if (value == null)
                {
                    videoSources = null;
                    PropertyPages = null;
                }
            }
		}


		/// <summary> 
		///  Collection of available audio sources/physical connectors 
		///  on the current audio device. 
		/// </summary>
		/// <remarks>
		///  In most cases, if the device has only one source, 
		///  this collection will be empty. For audio
		///  there are 2 different methods for enumerating audio sources
		///  an audio crossbar (usually TV tuners?) or an audio mixer 
		///  (usually sound cards?). This class will first look for an 
		///  audio crossbar. If no sources or only one source is available
		///  on the crossbar, this class will then look for an audio mixer.
		///  This class does not support both methods.
		/// 
		/// <para>
		///  The information contained in this property is retrieved and
		///  cached the first time this property is accessed. Future
		///  calls to this property use the cached results. This was done 
		///  for performance. </para>
		///  
		/// <para>
		///  However, this means <b>you may get different results depending 
		///  on when you access this property first</b>. If you are experiencing 
		///  problems, try accessing the property immediately after creating 
		///  the Capture class or immediately after setting the video and 
		///  audio compressors. Also, inform the developer. </para>
		///  </remarks>
		public SourceCollection AudioSources
		{
			get 
			{ 
				if ( audioSources == null )
				{
					try
					{
                        if (audioDevice != null)
                        {
                            audioSources = new SourceCollection(captureGraphBuilder, audioDeviceFilter, false);
							// Try to get a good reason to add additional
							// sources.

							// If there is an audioDevice, but there are no
							// sources found yet, than it might be possible
							// that this might be a video device with audio
							// capture via PCI bus. Than the crossbar related
							// to the video device must be checked.
							if((_audioViaPci)&&(videoDevice != null)&&(audioSources.Count == 0))
							{
								// Maybe there should be a check whether the
								// Audio device and the video device belongs
								// together. Too many checks however, might
								// limit functionality or make the code
								// complex. For now, keep it simple ...
								audioSources.addFromGraph( captureGraphBuilder, videoDeviceFilter, false );
							}
                        }
                        else
                        {
                            audioSources = new SourceCollection();
							// Try to get a good reason to add additional
							// sources.

							// If there is an audioDevice, but there are no
							// sources found yet, than it might be possible
							// that this might be a video device with audio
							// capture via PCI bus. Than the crossbar related
							// to the video device must be checked.
							if((_audioViaPci)&&(videoDevice != null)/*&&(audioSources.Count == 0)*/)
							{
								// Maybe there should be a check whether the
								// Audio device and the video device belongs
								// together. Too many checks however, might
								// limit functionality or make the code
								// complex. For now, keep it simple ...
								audioSources.addFromGraph( captureGraphBuilder, videoDeviceFilter, false );
							}
                        }
					}
					catch ( Exception ex ) { Debug.WriteLine( "AudioSources: unable to create AudioSources." + ex ); }
				}
				return ( audioSources );
			}
            set
            {
                if (value == null)
                {
                    audioSources = null;
                    PropertyPages = null;
                }
            }
		}

		/// <summary>
		///  Available property pages. 
		/// </summary>
		/// <remarks>
		///  These are property pages exposed by the DirectShow filters. 
		///  These property pages allow users modify settings on the 
		///  filters directly. 
		/// 
		/// <para>
		///  The information contained in this property is retrieved and
		///  cached the first time this property is accessed. Future
		///  calls to this property use the cached results. This was done 
		///  for performance. </para>
		///  
		/// <para>
		///  However, this means <b>you may get different results depending 
		///  on when you access this property first</b>. If you are experiencing 
		///  problems, try accessing the property immediately after creating 
		///  the Capture class or immediately after setting the video and 
		///  audio compressors. Also, inform the developer. </para>
		/// </remarks>
		public PropertyPageCollection PropertyPages 
		{
			get
			{
				if ( propertyPages == null )
				{
					try 
					{
						if( (_audioViaPci)&&
							(audioDeviceFilter == null)&&(videoDeviceFilter != null) )
						{
							propertyPages = new PropertyPageCollection( 
								captureGraphBuilder, 
								videoDeviceFilter, videoDeviceFilter, 
								videoCompressorFilter, audioCompressorFilter, 
								VideoSources, AudioSources );
						}
						else
						{
                        propertyPages = new PropertyPageCollection( 
							captureGraphBuilder, 
							videoDeviceFilter, audioDeviceFilter, 
							videoCompressorFilter, audioCompressorFilter, 
							VideoSources, AudioSources );
						}
                    }
					catch ( Exception ex ) { Debug.WriteLine( "PropertyPages: unable to get property pages." + ex ); }

				}
				return( propertyPages );
			}
            set
            {
                if (value == null)
                {
                    // Reload any property pages exposed by filters
                    if (propertyPages != null)
                    {
                        propertyPages.Dispose();
                        propertyPages = null;
                    }
                }
            }
		}
	
		/// <summary>
		///  Gets and sets the frame rate used to capture video.
		/// </summary>
		/// <remarks>
		///  Common frame rates: 24 fps for film, 25 for PAL, 29.997
		///  for NTSC. Not all NTSC capture cards can capture at 
		///  exactly 29.997 fps. Not all frame rates are supported. 
		///  When changing the frame rate, the closest supported 
		///  frame rate will be used. 
		///  
		/// <para>
		///  Not all devices support getting/setting this property.
		///  If this property is not supported, accessing it will
		///  throw and exception. </para>
		///  
		/// <para>
		///  This property cannot be changed while capturing. Changing 
		///  this property while preview is enabled will cause some 
		///  fickering while the internal filter graph is partially
		///  rebuilt. Changing this property while cued will cancel the
		///  cue. Call Cue() again to re-cue the capture. </para>
		/// </remarks>
		public double FrameRate
		{
			get
			{
				long avgTimePerFrame = (long) GetStreamConfigSetting( videoStreamConfig, "AvgTimePerFrame" );
				return( (double) 10000000 / avgTimePerFrame );
			}
			set
			{
				long avgTimePerFrame = (long) ( 10000000 / value );
				SetStreamConfigSetting( videoStreamConfig, "AvgTimePerFrame", avgTimePerFrame );
			}
		}

		/// <summary>
		///  Gets and sets the frame size used to capture video.
		/// </summary>
		/// <remarks>
		///  To change the frame size, assign a new Size object 
		///  to this property <code>capture.Size = new Size( w, h );</code>
		///  rather than modifying the size in place 
		///  (capture.Size.Width = w;). Not all frame
		///  rates are supported.
		///  
		/// <para>
		///  Not all devices support getting/setting this property.
		///  If this property is not supported, accessing it will
		///  throw and exception. </para>
		/// 
		/// <para> 
		///  This property cannot be changed while capturing. Changing 
		///  this property while preview is enabled will cause some 
		///  fickering while the internal filter graph is partially
		///  rebuilt. Changing this property while cued will cancel the
		///  cue. Call Cue() again to re-cue the capture. </para>
		/// </remarks>
		public Size FrameSize
		{
			get
			{
				BitmapInfoHeader bmiHeader;
				bmiHeader = (BitmapInfoHeader) GetStreamConfigSetting( videoStreamConfig, "BmiHeader" );
				Size size = new Size( bmiHeader.Width, bmiHeader.Height );
				return( size );
			}
			set
			{
				BitmapInfoHeader bmiHeader;
				bmiHeader = (BitmapInfoHeader) GetStreamConfigSetting( videoStreamConfig, "BmiHeader" );
				bmiHeader.Width = value.Width;
				bmiHeader.Height = value.Height;
				SetStreamConfigSetting( videoStreamConfig, "BmiHeader", bmiHeader );
				videoCaps = null;
			}		
		}

		/// <summary>
		///  Get or set the number of channels in the waveform-audio data. 
		/// </summary>
		/// <remarks>
		///  Monaural data uses one channel and stereo data uses two channels. 
		///  
		/// <para>
		///  Not all devices support getting/setting this property.
		///  If this property is not supported, accessing it will
		///  throw and exception. </para>
		///  
		/// <para>
		///  This property cannot be changed while capturing. Changing 
		///  this property while preview is enabled will cause some 
		///  fickering while the internal filter graph is partially
		///  rebuilt. Changing this property while cued will cancel the
		///  cue. Call Cue() again to re-cue the capture. </para>
		/// </remarks>
		public short AudioChannels
		{
			get
			{
				short audioChannels = (short) GetStreamConfigSetting( audioStreamConfig, "nChannels" );
				return( audioChannels );
			}
			set => SetStreamConfigSetting( audioStreamConfig, "nChannels", value );
		}

		/// <summary>
		///  Get or set the number of audio samples taken per second.
		/// </summary>
		/// <remarks>
		///  Common sampling rates are 8.0 kHz, 11.025 kHz, 22.05 kHz, and 
		///  44.1 kHz. Not all sampling rates are supported.
		///  
		/// <para>
		///  Not all devices support getting/setting this property.
		///  If this property is not supported, accessing it will
		///  throw and exception. </para>
		///  
		/// <para>
		///  This property cannot be changed while capturing. Changing 
		///  this property while preview is enabled will cause some 
		///  fickering while the internal filter graph is partially
		///  rebuilt. Changing this property while cued will cancel the
		///  cue. Call Cue() again to re-cue the capture. </para>
		/// </remarks>
		public int AudioSamplingRate
		{
			get
			{
				int samplingRate = (int) GetStreamConfigSetting( audioStreamConfig, "nSamplesPerSec" );
				return( samplingRate );
			}
			set => SetStreamConfigSetting( audioStreamConfig, "nSamplesPerSec", value );
		}

		/// <summary>
		///  Get or set the number of bits recorded per sample. 
		/// </summary>
		/// <remarks>
		///  Common sample sizes are 8 bit and 16 bit. Not all
		///  samples sizes are supported.
		///  
		/// <para>
		///  Not all devices support getting/setting this property.
		///  If this property is not supported, accessing it will
		///  throw and exception. </para>
		///  
		/// <para>
		///  This property cannot be changed while capturing. Changing 
		///  this property while preview is enabled will cause some 
		///  fickering while the internal filter graph is partially
		///  rebuilt. Changing this property while cued will cancel the
		///  cue. Call Cue() again to re-cue the capture. </para>
		/// </remarks>
		public short AudioSampleSize
		{
			get
			{
				short sampleSize = (short) GetStreamConfigSetting( audioStreamConfig, "wBitsPerSample" );
				return( sampleSize );
			}
			set => SetStreamConfigSetting( audioStreamConfig, "wBitsPerSample", value );
		}
        
		/// <summary> Fired when a capture is completed (manually or automatically). </summary>
		public event EventHandler					CaptureComplete;
        
		protected GraphState		graphState = GraphState.Null;		// State of the internal filter graph
		protected bool				isPreviewRendered;			// When graphState==Rendered, have we rendered the preview stream?
		protected bool				isCaptureRendered;			// When graphState==Rendered, have we rendered the capture stream?
		protected bool				wantPreviewRendered;		// Do we need the preview stream rendered (VideoDevice and PreviewWindow != null)
		protected bool				wantCaptureRendered;		// Do we need the capture stream rendered
        
		protected int				rotCookie;						// Cookie into the Running Object Table
        protected Filter			videoDevice;					// Property Backer: Video capture device filter
		protected Filter			audioDevice;					// Property Backer: Audio capture device filter
		protected Filter			videoCompressor;				// Property Backer: Video compression filter
		protected Filter			audioCompressor;				// Property Backer: Audio compression filter
		protected string			filename = "";						// Property Backer: Name of file to capture to
		protected Control			previewWindow;				// Property Backer: Owner control for preview
		protected VideoCapabilities	videoCaps;					// Property Backer: capabilities of video device
		protected AudioCapabilities	audioCaps;					// Property Backer: capabilities of audio device
		protected SourceCollection	videoSources;				// Property Backer: list of physical video sources
		protected SourceCollection	audioSources;				// Property Backer: list of physical audio sources
		protected PropertyPageCollection propertyPages;			// Property Backer: list of property pages exposed by filters
		protected IGraphBuilder		graphBuilder;						// DShow Filter: Graph builder 
		protected IMediaControl		mediaControl;						// DShow Filter: Start/Stop the filter graph -> copy of graphBuilder
		protected IVideoWindow		videoWindow;						// DShow Filter: Control preview window -> copy of graphBuilder
		protected ICaptureGraphBuilder2		captureGraphBuilder;	// DShow Filter: building graphs for capturing video
		protected IAMStreamConfig	videoStreamConfig;			// DShow Filter: configure frame rate, size
		protected IAMStreamConfig	audioStreamConfig;			// DShow Filter: configure sample rate, sample size
		protected IBaseFilter		videoDeviceFilter;			// DShow Filter: selected video device
		protected IBaseFilter		videoCompressorFilter;		// DShow Filter: selected video compressor
		protected IBaseFilter		audioDeviceFilter;			// DShow Filter: selected audio device
		protected IBaseFilter		audioCompressorFilter;		// DShow Filter: selected audio compressor
		protected IBaseFilter		muxFilter;					// DShow Filter: multiplexor (combine video and audio streams)
		protected IFileSinkFilter	fileWriterFilter;			// DShow Filter: file writer

		/// <summary> Recording file mode (e.g. Windows Media Audio) </summary>		 
		protected RecFileModeType recFileMode = RecFileModeType.Avi;

		// Option for selection audio rendering via the pci bus of the TV card
		// For wired audio connections the value must be false!
		// For TV-cards, like the Hauppauge PVR150, the value must be true!
		// This TV-card does not have a wired audio connection. However, this
		// option will work only if the TV-card driver has an audio device!
		private readonly bool _audioViaPci;

		/// <summary>
		/// Check if there is an Audio Device
		/// </summary>
		public bool AudioAvailable =>(_audioViaPci)&&(VideoDevice != null)||(AudioDevice != null);

        // Initialize AsfFormat class
		//private AsfFormat asfFormat = new AsfFormat(AsfFormat.AsfFormatSelection.Video);

        // ------------- Constructors/Destructors --------------

		/// <summary> 
		///  Create a new Capture object. 
		///  videoDevice and audioDevice can be null if you do not 
		///  wish to capture both audio and video. However at least
		///  one must be a valid device. Use the <see cref="Filters"/> 
		///  class to list available devices.
		///  </summary>
		public Capture(Filter videoDevice, Filter audioDevice, bool audioViaPci)
        {
			if ( videoDevice == null && audioDevice == null )
				throw new ArgumentException( "The videoDevice and/or the audioDevice parameter must be set to a valid Filter.\n" );
			this.videoDevice = videoDevice;
			this.audioDevice = audioDevice;
			Filename = GetTempFilename();
			_audioViaPci = audioViaPci;
            CreateGraph();
		}
        
		// --------------------- Public Methods -----------------------

		/// <summary>
		///  Prepare for capturing. Use this method when capturing 
		///  must begin as quickly as possible. 
		/// </summary>
		/// <remarks>
		///  This will create/overwrite a zero byte file with 
		///  the name set in the Filename property. 
		///  
		/// <para>
		///  This will disable preview. Preview will resume
		///  once capture begins. This problem can be fixed
		///  if someone is willing to make the change. </para>
		///  
		/// <para>
		///  This method is optional. If Cue() is not called, 
		///  Start() will call it before capturing. This method
		///  cannot be called while capturing. </para>
		/// </remarks>
		public void Cue()
		{
			assertStopped();

			// We want the capture stream rendered
			wantCaptureRendered = true;

			// Re-render the graph (if necessary)
			RenderGraph();

			// Pause the graph
			int hr = mediaControl.Pause();
			if ( hr != 0 ) Marshal.ThrowExceptionForHR( hr ); 
		}

		/// <summary> Begin capturing. </summary>
		public void Start()
		{
			assertStopped();

			// We want the capture stream rendered
			wantCaptureRendered = true;

			// Re-render the graph (if necessary)
			RenderGraph();

			// Start the filter graph: begin capturing
			int hr = mediaControl.Run();
			if ( hr != 0 ) Marshal.ThrowExceptionForHR( hr ); 

			// Update the state
			graphState = GraphState.Capturing;
		}

		/// <summary> 
		///  Stop the current capture capture. If there is no
		///  current capture, this method will succeed.
		/// </summary>
		public void Stop()
		{
			wantCaptureRendered = false;

			// Stop the graph if it is running
			// If we have a preview running we should only stop the
			// capture stream. However, if we have a preview stream
			// we need to re-render the graph anyways because we 
			// need to get rid of the capture stream. To re-render
			// we need to stop the entire graph
		    mediaControl?.Stop();

		    // Update the state
			if ( graphState == GraphState.Capturing )
			{
				graphState = GraphState.Rendered;
			    CaptureComplete?.Invoke( this, null );
			}

			// So we destroy the capture stream IF 
			// we need a preview stream. If we don't
			// this will leave the graph as it is.
		    try
		    {
		        RenderGraph();
		    }
		    catch
		    {
		        // ignored
		    }
		    try
		    {
		        StartPreviewIfNeeded();
		    }
		    catch
		    {
		        // ignored
		    }
		}

		/// <summary> 
		///  Calls Stop, releases all references. If a capture is in progress
		///  it will be stopped, but the CaptureComplete event will NOT fire.
		/// </summary>
		public void Dispose()
		{
			wantPreviewRendered = false;
			wantCaptureRendered = false;
			CaptureComplete = null;

		    try
		    {
		        DestroyGraph();
		    }
		    catch
		    {
		        // ignored
		    }

		    videoSources?.Dispose();
		    videoSources = null;
		    audioSources?.Dispose();
		    audioSources = null;

		}



		// --------------------- Private Methods -----------------------
		
		/// <summary> 
		///  Create a new filter graph and add filters (devices, compressors, 
		///  misc), but leave the filters unconnected. Call renderGraph()
		///  to connect the filters.
		/// </summary>
		protected void CreateGraph()
		{
			Guid cat;
			Guid med;
			int	hr;

			// Ensure required properties are set
			if ( videoDevice == null && audioDevice == null )
				throw new ArgumentException( "The video and/or audio device have not been set. Please set one or both to valid capture devices.\n" );

			// Skip if we are already created
		    if ((int) graphState >= (int) GraphState.Created) return;

		    // Make a new filter graph
		    graphBuilder = (IGraphBuilder)Activator.CreateInstance(Type.GetTypeFromCLSID(Clsid.FilterGraph, true));

		    // Get the Capture Graph Builder
		    Guid clsid = Clsid.CaptureGraphBuilder2;
		    Guid riid = typeof(ICaptureGraphBuilder2).GUID;
		    captureGraphBuilder = (ICaptureGraphBuilder2)DsBugWO.CreateDsInstance(ref clsid, ref riid);

		    // Link the CaptureGraphBuilder to the filter graph
		    hr = captureGraphBuilder.SetFiltergraph(graphBuilder);
		    if (hr < 0) Marshal.ThrowExceptionForHR(hr);

		    // Add the graph to the Running Object Table so it can be
		    // viewed with GraphEdit
#if DEBUG
		    DsROT.AddGraphToRot(graphBuilder, out rotCookie);
#endif
		    // Get the video device and add it to the filter graph
		    if ( VideoDevice != null )
		    {
		        videoDeviceFilter = (IBaseFilter) Marshal.BindToMoniker( VideoDevice.MonikerString );
		        hr = graphBuilder.AddFilter( videoDeviceFilter, "Video Capture Device" );
		        if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
		    }

		    // Get the audio device and add it to the filter graph
		    if ( AudioDevice != null )
		    {
		        audioDeviceFilter = (IBaseFilter) Marshal.BindToMoniker( AudioDevice.MonikerString );
		        hr = graphBuilder.AddFilter( audioDeviceFilter, "Audio Capture Device" );
		        if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
		    }

		    // Get the video compressor and add it to the filter graph
		    if ( VideoCompressor != null )
		    {
		        videoCompressorFilter = (IBaseFilter) Marshal.BindToMoniker( VideoCompressor.MonikerString ); 
		        hr = graphBuilder.AddFilter( videoCompressorFilter, "Video Compressor" );
		        if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
		    }

		    // Get the audio compressor and add it to the filter graph
		    if ( AudioCompressor != null )
		    {
		        audioCompressorFilter = (IBaseFilter) Marshal.BindToMoniker( AudioCompressor.MonikerString ); 
		        hr = graphBuilder.AddFilter( audioCompressorFilter, "Audio Compressor" );
		        if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
		    }

		    // Retrieve the stream control interface for the video device
		    // FindInterface will also add any required filters
		    // (WDM devices in particular may need additional
		    // upstream filters to function).

		    // Try looking for an interleaved media type
		    object o;
		    cat = PinCategory.Capture;
		    med = MediaType.Interleaved;
		    Guid iid = typeof(IAMStreamConfig).GUID;
		    hr = captureGraphBuilder.FindInterface(
		        ref cat, ref med, videoDeviceFilter, ref iid, out o);
		    if ( hr != 0 )
		    {
		        // If not found, try looking for a video media type
		        med = MediaType.Video;
		        hr = captureGraphBuilder.FindInterface(
		            ref cat, ref med, videoDeviceFilter, ref iid, out o);
		        if ( hr != 0 )
		            o = null;
		    }
		    videoStreamConfig = o as IAMStreamConfig;
                
		    o = null;
		    cat = PinCategory.Preview;
		    med = MediaType.Interleaved;
		    iid = typeof(IAMStreamConfig).GUID;
		    hr = captureGraphBuilder.FindInterface(
		        ref cat, ref med, videoDeviceFilter, ref iid, out o);

		    if ( hr != 0 )
		    {
		        // If not found, try looking for a video media type
		        med = MediaType.Video;
		        hr = captureGraphBuilder.FindInterface(
		            ref cat, ref med, videoDeviceFilter, ref iid, out o);
		        if ( hr != 0 )
		            o = null;
		    }
		    previewStreamConfig = o as IAMStreamConfig;
		    o = null;
		    cat = PinCategory.Capture;
		    med = MediaType.Audio ;
		    iid = typeof(IAMStreamConfig).GUID;
		    if( (_audioViaPci)&&
		        (audioDeviceFilter == null)&&(videoDeviceFilter != null) )
		    {
		        hr = captureGraphBuilder.FindInterface(ref cat, ref med, videoDeviceFilter, ref iid, out o );
		    }
		    else
		    {
		        hr = captureGraphBuilder.FindInterface(ref cat, ref med, audioDeviceFilter, ref iid, out o);
		    }

		    if (hr != 0)
		        o = null;
		    audioStreamConfig = o as IAMStreamConfig;

		    // Retreive the media control interface (for starting/stopping graph)
		    mediaControl = graphBuilder as IMediaControl;

		    // Reload any video crossbars
		    videoSources?.Dispose();
		    videoSources = null;

		    // Reload any audio crossbars
		    audioSources?.Dispose();
		    audioSources = null;
				
		    // Reload any property pages exposed by filters
		    PropertyPages = null;

		    // Reload capabilities of video device
		    videoCaps = null;
		    previewCaps = null;

		    // Reload capabilities of video device
		    audioCaps = null;


		    graphState = GraphState.Created;
		}

		/// <summary>
		///  Connects the filters of a previously created graph 
		///  (created by createGraph()). Once rendered the graph
		///  is ready to be used. This method may also destroy
		///  streams if we have streams we no longer want.
		/// </summary>
		protected void RenderGraph()
		{
			Guid					cat;
			Guid					med;
			int						hr;
			bool					didSomething = false;
			const int WS_CHILD			= 0x40000000;	
			const int WS_CLIPCHILDREN	= 0x02000000;
			const int WS_CLIPSIBLINGS	= 0x04000000;

			assertStopped();

			// Ensure required properties set
			if ( filename == null )
				throw new ArgumentException( "The Filename property has not been set to a file.\n" );

			// Stop the graph
		    mediaControl?.Stop();

		    // Create the graph if needed (group should already be created)
			CreateGraph();

			// Derender the graph if we have a capture or preview stream
			// that we no longer want. We can't derender the capture and 
			// preview streams seperately. 
			// Notice the second case will leave a capture stream intact
			// even if we no longer want it. This allows the user that is
			// not using the preview to Stop() and Start() without
			// rerendering the graph.
			if ( !wantPreviewRendered && isPreviewRendered )
				DerenderGraph();
			if ( !wantCaptureRendered && isCaptureRendered )
				if ( wantPreviewRendered )
					DerenderGraph();


			// Render capture stream (only if necessary)
			if ( wantCaptureRendered && !isCaptureRendered )
			{
				// Render the file writer portion of graph (mux -> file)
				// Record captured audio/video in Avi format
				Guid mediaSubType; // Media sub type
				bool captureAudio = true;
				bool captureVideo = true;
				IBaseFilter videoCompressorfilter = null;

				// Set media sub type and video compressor filter if needed
					mediaSubType = MediaSubType.Avi;
					// For Avi file saving a video compressor must be used
					// If one is selected, that one will be used.
					videoCompressorfilter = videoCompressorFilter;

				// Intialize the Avi or Asf file writer
                hr = captureGraphBuilder.SetOutputFileName(ref mediaSubType, Filename, out muxFilter, out fileWriterFilter);
				if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				// Render video (video -> mux) if needed or possible
				if((VideoDevice != null)&&(captureVideo))
                {
					// Try interleaved first, because if the device supports it,
					// it's the only way to get audio as well as video
					cat = PinCategory.Capture;
					med = MediaType.Interleaved;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, videoCompressorfilter, muxFilter);
					if( hr < 0 ) 
					{
						med = MediaType.Video;
                        hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, videoCompressorfilter, muxFilter);
						if ( hr == -2147220969 ) throw new DeviceInUseException( "Video device", hr );
						if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
					}
				}

				// Render audio (audio -> mux) if possible
				if((audioDeviceFilter != null)&&(captureAudio))
				{
					// If this Asf file format than please keep in mind that
					// certain Wmv formats do not have an audio stream, so
					// when using this code, please ensure you use a format
					// which supports audio!
					cat = PinCategory.Capture;
					med = MediaType.Audio;
					hr = captureGraphBuilder.RenderStream( ref cat, ref med, audioDeviceFilter, audioCompressorFilter, muxFilter ); 
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				}
				else
					if( (_audioViaPci)&&(captureAudio)&&
					(audioDeviceFilter == null)&&(videoDeviceFilter != null) )
				{
					cat = PinCategory.Capture;
					med = MediaType.Audio;
					hr = captureGraphBuilder.RenderStream( ref cat, ref med, videoDeviceFilter, audioCompressorFilter, muxFilter );
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				}

				isCaptureRendered = true;
				didSomething = true;
			}

			// Render preview stream (only if necessary)
			if ( wantPreviewRendered && !isPreviewRendered )
			{
				// Render preview (video -> renderer)
				InitVideoRenderer();
				//this.AddDeInterlaceFilter();

				// When capture pin is used, preview works immediately,
				// however this conflicts with file saving.
				// An alternative is to use VMR9
                cat = PinCategory.Preview;
				med = MediaType.Video;
				if(InitSampleGrabber())
				{
					Debug.WriteLine("SampleGrabber added to graph.");
                    
					hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, _baseGrabFlt, _videoRendererFilter);
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				}
				else
				{
					hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, null, _videoRendererFilter);
					if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );
				}

				// Special option to enable rendering audio via PCI bus
				if((_audioViaPci)&&(audioDeviceFilter != null))
				{
					cat = PinCategory.Preview;
					med = MediaType.Audio;
					hr = captureGraphBuilder.RenderStream( ref cat, ref med, audioDeviceFilter, null, null ); 
					if( hr < 0 )
					{
						Marshal.ThrowExceptionForHR( hr );
					}
				}
				else
					if( (_audioViaPci)&&
					(audioDeviceFilter == null)&&(videoDeviceFilter != null) )
				{
					cat = PinCategory.Preview;
					med = MediaType.Audio;
					hr = captureGraphBuilder.RenderStream( ref cat, ref med, videoDeviceFilter, null, null ); 
					if( hr < 0 )
					{
						Marshal.ThrowExceptionForHR( hr );
					}
				}

				// Get the IVideoWindow interface
				videoWindow = graphBuilder as IVideoWindow;

				// Set the video window to be a child of the main window
				hr = videoWindow.put_Owner( previewWindow.Handle );
				if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

				// Set video window style
				hr = videoWindow.put_WindowStyle( WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS);
				if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

				// Position video window in client rect of owner window
				previewWindow.Resize += OnPreviewWindowResize;
				OnPreviewWindowResize( this, null );

				// Make the video window visible, now that it is properly positioned
				hr = videoWindow.put_Visible( DsHlp.OATRUE );
				if( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

				isPreviewRendered = true;
				didSomething = true;
				SetMediaSampleGrabber();
			}

			if ( didSomething )
				graphState = GraphState.Rendered;
		}

		/// <summary>
		///  Setup and start the preview window if the user has
		///  requested it (by setting PreviewWindow).
		/// </summary>
		protected void StartPreviewIfNeeded()
		{
			// Render preview 
			if ( wantPreviewRendered && isPreviewRendered && !isCaptureRendered )
			{
				// Run the graph (ignore errors)
				// We can run the entire graph becuase the capture
				// stream should not be rendered (and that is enforced
				// in the if statement above)
#if DEBUG
				try
				{
					int hr = mediaControl.Run();
					if(hr != 0)
					{
						Debug.WriteLine("MediaControl.Run() returns: " + hr);
					}
				}
				catch
				{
					Debug.WriteLine("MediaControl.Run()in StartPreviewIfNeeded cause fatal exception ...");
				}
#else
                mediaControl.Run();
#endif
            }
		}

		/// <summary>
		///  Disconnect and remove all filters except the device
		///  and compressor filters. This is the opposite of
		///  renderGraph(). Soem properties such as FrameRate
		///  can only be set when the device output pins are not
		///  connected. 
		/// </summary>
		protected void DerenderGraph()
		{
			// Stop the graph if it is running (ignore errors)
			if ( mediaControl != null )
				mediaControl.Stop();

			// Free the preview window (ignore errors)
			if ( videoWindow != null )
			{
                videoWindow.put_Visible(DsHlp.OAFALSE);
				videoWindow.put_Owner( IntPtr.Zero );
				videoWindow = null;
			}

			// Remove the Resize event handler
			if ( PreviewWindow != null )
				previewWindow.Resize -= OnPreviewWindowResize;

			if ( (int)graphState >= (int)GraphState.Rendered )
			{
				// Update the state
				graphState = GraphState.Created;
				isCaptureRendered = false;
				isPreviewRendered = false;

				// Disconnect all filters downstream of the 
				// video and audio devices. If we have a compressor
				// then disconnect it, but don't remove it
				if (videoDeviceFilter != null)
				{
					try
					{
						RemoveDownstream(videoDeviceFilter, (videoCompressor == null));
					}
					catch
					{
						Debug.WriteLine("Error removeDownstream videoDeviceFilter");
					}
				}
				if (audioDeviceFilter != null)
				{
					try
					{
						RemoveDownstream(audioDeviceFilter, (audioCompressor == null));
					}
					catch
					{
						Debug.WriteLine("Error removeDownstream audioDeviceFilter");
					}
				}

				// These filters should have been removed by the
				// calls above. (Is there anyway to check?)
				muxFilter = null;
				fileWriterFilter = null;
				_videoRendererFilter = null;
            }
		}

		/// <summary>
		///  Removes all filters downstream from a filter from the graph.
		///  This is called only by derenderGraph() to remove everything
		///  from the graph except the devices and compressors. The parameter
		///  "removeFirstFilter" is used to keep a compressor (that should
		///  be immediately downstream of the device) if one is begin used.
		/// </summary>
		protected void RemoveDownstream( IBaseFilter filter, bool removeFirstFilter )
		{
			// Get a pin enumerator off the filter
			IEnumPins pinEnum;
			int hr = filter.EnumPins( out pinEnum );
			pinEnum.Reset();
			if( (hr == 0) && (pinEnum != null) )
			{
				// Loop through each pin
				IPin[] pins = new IPin[1];
				int f;
				do
				{
					// Get the next pin
					hr = pinEnum.Next( 1, pins, out f );
					if( (hr == 0) && (pins[0] != null) )
					{
						// Get the pin it is connected to
						IPin pinTo = null;
						pins[0].ConnectedTo( out pinTo );
						if ( pinTo != null )
						{
							// Is this an input pin?
							PinInfo info = new PinInfo();
							hr = pinTo.QueryPinInfo( out info );
							if( (hr == 0) && (info.dir == (PinDirection.Input)) )
							{
								// Recurse down this branch
								RemoveDownstream( info.filter, true );

								// Disconnect 
								graphBuilder.Disconnect( pinTo );
								graphBuilder.Disconnect( pins[0] );

								// Remove this filter
								// but don't remove the video or audio compressors
								if ( ( info.filter != videoCompressorFilter ) &&
									 ( info.filter != audioCompressorFilter ) )
									graphBuilder.RemoveFilter( info.filter );
							}
							Marshal.ReleaseComObject( info.filter );
							Marshal.ReleaseComObject( pinTo );
						}
						Marshal.ReleaseComObject( pins[0] );
					}
				}
				while( hr == 0 );

				Marshal.ReleaseComObject( pinEnum ); pinEnum = null;
			}
		}

		/// <summary>
		///  Completely tear down a filter graph and 
		///  release all associated resources.
		/// </summary>
		protected void DestroyGraph()
		{
			// Derender the graph (This will stop the graph
			// and release preview window. It also destroys
			// half of the graph which is unnecessary but
			// harmless here.) (ignore errors)
		    try
		    {
		        DerenderGraph();
		    }
		    catch
		    {
		        // ignored
		    }

		    // Update the state after derender because it
			// depends on correct status. But we also want to
			// update the state as early as possible in case
			// of error.
			graphState = GraphState.Null;
			isCaptureRendered = false;
			isPreviewRendered = false;

            // Remove graph from the ROT
			if ( rotCookie != 0 )
			{
				DsROT.RemoveGraphFromRot( ref rotCookie );
				rotCookie = 0;
			}

            // Remove filters from the graph
			// This should be unnecessary but the Nvidia WDM
			// video driver cannot be used by this application 
			// again unless we remove it. Ideally, we should
			// simply enumerate all the filters in the graph
			// and remove them. (ignore errors)
			if ( muxFilter != null )
				graphBuilder.RemoveFilter( muxFilter );
			if ( videoCompressorFilter != null )
				graphBuilder.RemoveFilter( videoCompressorFilter  );
			if ( audioCompressorFilter != null )
				graphBuilder.RemoveFilter( audioCompressorFilter  );
			if ( videoDeviceFilter != null )
				graphBuilder.RemoveFilter( videoDeviceFilter );
			if ( audioDeviceFilter != null )
				graphBuilder.RemoveFilter( audioDeviceFilter );

			if(_videoRendererFilter != null)
			{
				graphBuilder.RemoveFilter(_videoRendererFilter);
			}

            // Clean up properties
		    videoSources?.Dispose();
		    videoSources = null;
		    audioSources?.Dispose();
		    audioSources = null;
            PropertyPages = null; // Disposal done within PropertyPages

			// Cleanup
			if ( graphBuilder != null )
				Marshal.ReleaseComObject( graphBuilder );  graphBuilder = null;
			if ( captureGraphBuilder != null )
				Marshal.ReleaseComObject( captureGraphBuilder ); captureGraphBuilder = null;
			if ( muxFilter != null )
				Marshal.ReleaseComObject( muxFilter ); muxFilter = null;
			if ( fileWriterFilter != null )
				Marshal.ReleaseComObject( fileWriterFilter ); fileWriterFilter = null;
			if ( videoDeviceFilter != null )
				Marshal.ReleaseComObject( videoDeviceFilter ); videoDeviceFilter = null;
			if ( audioDeviceFilter != null )
				Marshal.ReleaseComObject( audioDeviceFilter ); audioDeviceFilter = null;
			if ( videoCompressorFilter != null )
				Marshal.ReleaseComObject( videoCompressorFilter ); videoCompressorFilter = null;
			if ( audioCompressorFilter != null )
				Marshal.ReleaseComObject( audioCompressorFilter ); audioCompressorFilter = null;
			DisposeSampleGrabber();

			if(_videoRendererFilter != null)
			{
				Marshal.ReleaseComObject(_videoRendererFilter); _videoRendererFilter = null;
			}

            // These are copies of graphBuilder
			mediaControl = null;
			videoWindow = null;
        }

		/// <summary> Resize the preview when the PreviewWindow is resized </summary>
		protected void OnPreviewWindowResize(object sender, EventArgs e)
		{
		    if (videoWindow == null) return;
		    // Position video window in client rect of owner window
		    Rectangle rc = previewWindow.ClientRectangle;
		    videoWindow.SetWindowPosition( 0, 0, rc.Right, rc.Bottom );
		}
	
		/// <summary> 
		///  Get a valid temporary filename (with path). We aren't using 
		///  Path.GetTempFileName() because it creates a 0-byte file 
		/// </summary>
		protected string GetTempFilename()
		{
			string s;
		    try
		    {
		        int count = 0;
		        int i;
		        Random r = new Random();
		        string tempPath = Path.GetTempPath();
		        do
		        {
		            i = r.Next();
		            s = Path.Combine(tempPath, i.ToString("X") + ".avi");
		            count++;
		            if (count > 100)
                        throw new InvalidOperationException("Unable to find temporary file.");
		        }
                while (File.Exists(s));
		    }
		    catch
		    {
		        s = "c:\temp.avi";
		    }
			return( s );
		}

		/// <summary>
		///  Retrieves the value of one member of the IAMStreamConfig format block.
		///  Helper function for several properties that expose
		///  video/audio settings from IAMStreamConfig.GetFormat().
		///  IAMStreamConfig.GetFormat() returns a AMMediaType struct.
		///  AMMediaType.formatPtr points to a format block structure.
		///  This format block structure may be one of several 
		///  types, the type being determined by AMMediaType.formatType.
		/// </summary>
		protected object GetStreamConfigSetting( IAMStreamConfig streamConfig, string fieldName)
		{
			if ( streamConfig == null )
				throw new NotSupportedException();
			assertStopped();

			DerenderGraph();

			object returnValue = null;
			IntPtr pmt = IntPtr.Zero;
			AMMediaType mediaType = new AMMediaType();

			try 
			{
				// Get the current format info
                int hr = streamConfig.GetFormat(out pmt);
				if ( hr != 0 )
					Marshal.ThrowExceptionForHR( hr );
				Marshal.PtrToStructure( pmt, mediaType );

				// The formatPtr member points to different structures
				// dependingon the formatType
				object formatStruct;
				if ( mediaType.formatType == FormatType.WaveEx )
					formatStruct = new WaveFormatEx();
				else if ( mediaType.formatType == FormatType.VideoInfo )
					formatStruct = new VideoInfoHeader();
				else if ( mediaType.formatType == FormatType.VideoInfo2 )
					formatStruct = new VideoInfoHeader2();
				else
					throw new NotSupportedException( "This device does not support a recognized format block." );

				// Retrieve the nested structure
				Marshal.PtrToStructure( mediaType.formatPtr, formatStruct );

				// Find the required field
				Type structType = formatStruct.GetType();
				FieldInfo fieldInfo = structType.GetField( fieldName );
				if ( fieldInfo == null )
					throw new NotSupportedException( "Unable to find the member '" + fieldName + "' in the format block." );

				// Extract the field's current value
				returnValue = fieldInfo.GetValue( formatStruct ); 
						
			}
			finally
			{
				DsUtils.FreeAMMediaType( mediaType );
				Marshal.FreeCoTaskMem( pmt );
			}
			RenderGraph();
			StartPreviewIfNeeded();

			return( returnValue );
		}

		/// <summary>
		///  Set the value of one member of the IAMStreamConfig format block.
		///  Helper function for several properties that expose
		///  video/audio settings from IAMStreamConfig.GetFormat().
		///  IAMStreamConfig.GetFormat() returns a AMMediaType struct.
		///  AMMediaType.formatPtr points to a format block structure.
		///  This format block structure may be one of several 
		///  types, the type being determined by AMMediaType.formatType.
		/// </summary>
		protected object SetStreamConfigSetting( IAMStreamConfig streamConfig, string fieldName, object newValue)
		{
			if ( streamConfig == null )
				throw new NotSupportedException();
			assertStopped();
			DerenderGraph();

			object returnValue = null;
            IntPtr pmt = IntPtr.Zero;
            AMMediaType mediaType = new AMMediaType();

			try 
			{
				// Get the current format info
                int hr = streamConfig.GetFormat(out pmt);
				if ( hr != 0 )
					Marshal.ThrowExceptionForHR( hr );
                Marshal.PtrToStructure(pmt, mediaType);

				// The formatPtr member points to different structures
				// dependingon the formatType
				object formatStruct;
				if ( mediaType.formatType == FormatType.WaveEx )
					formatStruct = new WaveFormatEx();
				else if ( mediaType.formatType == FormatType.VideoInfo )
					formatStruct = new VideoInfoHeader();
				else if ( mediaType.formatType == FormatType.VideoInfo2 )
					formatStruct = new VideoInfoHeader2();
				else
					throw new NotSupportedException( "This device does not support a recognized format block." );

				// Retrieve the nested structure
				Marshal.PtrToStructure( mediaType.formatPtr, formatStruct );

				// Find the required field
				Type structType = formatStruct.GetType();
				FieldInfo fieldInfo = structType.GetField( fieldName );
				if ( fieldInfo == null )
					throw new NotSupportedException( "Unable to find the member '" + fieldName + "' in the format block." );

				// Update the value of the field
				fieldInfo.SetValue( formatStruct, newValue );

				// Update fields that may depend on specific values of other attributes
				if (mediaType.formatType == FormatType.WaveEx)
				{
					WaveFormatEx waveFmt = formatStruct as WaveFormatEx;
					waveFmt.nBlockAlign = (short)(waveFmt.nChannels * waveFmt.wBitsPerSample / 8);
					waveFmt.nAvgBytesPerSec = waveFmt.nBlockAlign * waveFmt.nSamplesPerSec;
				}

                // PtrToStructure copies the data so we need to copy it back
				Marshal.StructureToPtr( formatStruct, mediaType.formatPtr, false ); 

				// Save the changes
				hr = streamConfig.SetFormat( mediaType );
				if ( hr != 0 )
					Marshal.ThrowExceptionForHR( hr );
			}
			finally
			{
				DsUtils.FreeAMMediaType( mediaType );
                Marshal.FreeCoTaskMem(pmt);
            }
			RenderGraph();
			StartPreviewIfNeeded();

			return( returnValue );
		}

		/// <summary>
		///  Assert that the class is in a Stopped state.
		/// </summary>
		protected void assertStopped()
		{
			if ( !Stopped )
            {
                throw new InvalidOperationException( "This operation not allowed while Capturing. Please Stop the current capture." );
            }
        }
        
		/// <summary>
		/// CLSID_VideoRenderer
		/// </summary>
		[ComImport, Guid("70e102b0-5556-11ce-97c0-00aa0055595a")]
		public class VideoRenderer
		{

		}

		/// <summary>
		/// Use VMR9 flag, if false use the video renderer instead
		/// </summary>
		private bool _useVmr9;

		private IBaseFilter _videoRendererFilter;

		/// <summary>
		/// Check if VMR9 should be used
		/// </summary>
		public bool UseVMR9
		{
			get => _useVmr9;
		    set => _useVmr9 = value;
		}

		private bool InitVideoRenderer()
		{
			if(_videoRendererFilter != null)
			{
				graphBuilder.RemoveFilter(_videoRendererFilter);
				Marshal.ReleaseComObject(_videoRendererFilter);
				_videoRendererFilter = null;
			}

			if(_useVmr9)
			{
				_videoRendererFilter = new VideoMixingRenderer9() as IBaseFilter;
			}
			else
			{
				_videoRendererFilter = new VideoRenderer() as IBaseFilter;
			}

			if(_videoRendererFilter != null)
			{
				graphBuilder.AddFilter(_videoRendererFilter, "Video Renderer");
			}
			return false;
		}
        
		/// <summary>
		/// CLSID_SampleGrabber
		/// </summary>
		[ComImport, Guid("C1F400A0-3F08-11d3-9F0B-006008039E37")]
			public class SampleGrabber
		{
		}
		/// <summary>
		/// SampleGrabber flag, if false do not insert SampleGrabber in graph
		/// </summary>
		private bool _allowSampleGrabber;

		/// <summary> grabber filter interface. </summary>
		private IBaseFilter _baseGrabFlt;

		/// <summary>
		/// Sample Grabber interface
		/// </summary>
		protected ISampleGrabber SampGrabber;

		/// <summary>
		/// Check if usage SampleGrabber is allowed
		/// </summary>
		public bool AllowSampleGrabber
		{
			get => _allowSampleGrabber;
		    set => _allowSampleGrabber = value;
		}

		int ISampleGrabberCB.SampleCB( double sampleTime, IMediaSample pSample )
		{
			Trace.Write ("Sample");
			return 0;
		}

		/// <summary>
		/// Disable grabbing next frame
		/// </summary>
		public void DisableEvent()
		{
		    SampGrabber?.SetCallback(null, 0);
		}

		/// <summary> Interface frame event </summary>
		public delegate void HeFrame(Bitmap BM);
		private delegate void CaptureDone();
		/// <summary> Frame event </summary>
		public event HeFrame FrameEvent2;
		private	byte[] _savedArray;
		private	int	_bufferedSize;
		private bool _frameCaptured = true;

		int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen )
		{
			if(_frameCaptured)
			{
				return 0;
			}
			_frameCaptured = true;
			_bufferedSize = BufferLen;
			
			int stride = SnapShotWidth * 3;

			Marshal.Copy( pBuffer, _savedArray, 0, BufferLen );

			GCHandle handle = GCHandle.Alloc( _savedArray, GCHandleType.Pinned );
			int scan0 = (int) handle.AddrOfPinnedObject();
			scan0 += (SnapShotHeight - 1) * stride;
			Bitmap b = new Bitmap(SnapShotWidth, SnapShotHeight, -stride, PixelFormat.Format24bppRgb, (IntPtr) scan0 );
			handle.Free();

			// Transfer bitmap upon firing event
			FrameEvent2(b);
			return 0;
		}

		/// <summary> Allocate memory space and set SetCallBack </summary>
		public void GrapImg()
		{
			Trace.Write ("IMG");

			if( _savedArray == null )
			{
				int size = snapShotImageSize;
				if( (size < 1000) || (size > 16000000) )
					return;
				_savedArray = new byte[ size + 64000 ];
			}
			SampGrabber.SetCallback( this, 1 );
			_frameCaptured = false;
		}

		private bool InitSampleGrabber()
		{
			if (VideoDevice == null)
			{
				// nothing to do
				return false;
			}

			if (!_allowSampleGrabber)
			{
				return false;
			}

			DisposeSampleGrabber();

			int hr  = 0;

			// Get SampleGrabber if needed
			if(SampGrabber == null)
			{
				SampGrabber = new SampleGrabber() as ISampleGrabber;
			}

			if(SampGrabber == null)
			{
				return false;
			}
            
			_baseGrabFlt	= (IBaseFilter) SampGrabber;

			if(_baseGrabFlt == null)
			{
				Marshal.ReleaseComObject(SampGrabber);
				SampGrabber = null;
			}

		    AMMediaType media = new AMMediaType
		    {
		        majorType = MediaType.Video,
		        subType = MediaSubType.RGB24,
		        formatPtr = IntPtr.Zero
		    };

		    hr = SampGrabber.SetMediaType(media);
			if(hr < 0)
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			hr = graphBuilder.AddFilter(_baseGrabFlt, "SampleGrabber");
			if(hr < 0)
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			hr = SampGrabber.SetBufferSamples(false);
			if( hr == 0 )
			{
				hr = SampGrabber.SetOneShot(false);
			}
			if( hr == 0 )
			{
				hr = SampGrabber.SetCallback(null, 0);
			}
			if( hr < 0 )
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			return true;
		}

		private void SetMediaSampleGrabber()
		{
			snapShotValid = false;
			if((_baseGrabFlt != null)&&(AllowSampleGrabber))
			{
				AMMediaType media = new AMMediaType();
				VideoInfoHeader videoInfoHeader;
				int hr;

				hr = SampGrabber.GetConnectedMediaType(media);
				if (hr < 0)
				{
					Marshal.ThrowExceptionForHR(hr);
				}
				if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
				{
					throw new NotSupportedException("Unknown Grabber Media Format");
				}

				videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
				snapShotWidth = videoInfoHeader.BmiHeader.Width;
				snapShotHeight = videoInfoHeader.BmiHeader.Height;
				snapShotImageSize = videoInfoHeader.BmiHeader.ImageSize;
				Marshal.FreeCoTaskMem(media.formatPtr);
				media.formatPtr = IntPtr.Zero;
				snapShotValid = true;
			}

			if (!snapShotValid)
			{
				snapShotWidth = 0;
				snapShotHeight = 0;
				snapShotImageSize = 0;
			}
		}

		private int snapShotWidth;
		private int snapShotHeight;
		private int snapShotImageSize;
		private bool snapShotValid;

		/// <summary>
		/// Dispose Sample Grabber specific data
		/// </summary>
		public void DisposeSampleGrabber()
		{
			if(_baseGrabFlt != null)
			{
				try
				{
					graphBuilder.RemoveFilter(_baseGrabFlt);
				}
				catch
				{
				}
				Marshal.ReleaseComObject(_baseGrabFlt);
				_baseGrabFlt = null;
			}

			if(SampGrabber != null)
			{
				Marshal.ReleaseComObject(SampGrabber);
				SampGrabber = null;
			}
			_savedArray =  null;
		}

		/// <summary>
		/// SampleGrabber video width
		/// </summary>
		public int SnapShotWidth => snapShotWidth;

        /// <summary>
		/// SampleGrabber video height
		/// </summary>
		public int SnapShotHeight => snapShotHeight;

        /// <summary>
		/// Show property page of object 
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool ShowPropertyPage(int filter, Control o)
		{
			ISpecifyPropertyPages specifyPropertyPages =  null;
			String propertyPageName = "";
		    if (filter == 1)
		    {
		        if (_videoRendererFilter != null)
		        {
		            specifyPropertyPages = _videoRendererFilter as ISpecifyPropertyPages;
		            propertyPageName = "Video Renderer";
		        }
		    }
		    DirectShowPropertyPage propertyPage = new DirectShowPropertyPage(propertyPageName, specifyPropertyPages);
		    if (propertyPage == null) return false;
		    propertyPage.Show(o);
		    propertyPage.Dispose();
		    return true;
		}

		/// <summary>
		/// IAMStreamConfig interface of preview pin. It is not really
		/// common that the preview has such interface, but if it has
		/// such interface it can be used "independent" from the capture
		/// pin interface.
		/// </summary>
		protected IAMStreamConfig	previewStreamConfig;	

		/// <summary>
		/// Property Backer: preview capabilities of video device
		/// </summary>
		protected VideoCapabilities	previewCaps;
	}
}

