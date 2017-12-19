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

	
	public partial class Capture : ISampleGrabberCB
    {
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

		/// <summary> Is the class currently capturing. Read-only. </summary>
		public bool Capturing => graphState==GraphState.Capturing;

        /// <summary> Has the class been cued to begin capturing. Read-only. </summary>
		public bool Cued => isCaptureRendered && graphState==GraphState.Rendered;

        /// <summary>
        /// Is the class currently stopped. Read-only.
        /// </summary>
		public bool Stopped => graphState != GraphState.Capturing;

        public string Filename 
		{ 
			get => filename;
            set 
			{ 
				assertStopped();
				if (Cued)
					throw new InvalidOperationException( "The Filename cannot be changed once cued. Use Stop() before changing the filename." );
				filename = value; 
				if ( fileWriterFilter != null )
				{
					string s;
					AMMediaType mt = new AMMediaType(); 
					int hr = fileWriterFilter.GetCurFile( out s, mt );
					if( hr < 0 )
                        Marshal.ThrowExceptionForHR( hr );
					if ( mt.formatSize > 0 )
						Marshal.FreeCoTaskMem( mt.formatPtr ); 
					hr = fileWriterFilter.SetFileName( filename, mt );
					if( hr < 0 )
                        Marshal.ThrowExceptionForHR( hr );
				}
			} 
		}
        
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
								audioSources.AddFromGraph( captureGraphBuilder, videoDeviceFilter, false );
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
								audioSources.AddFromGraph( captureGraphBuilder, videoDeviceFilter, false );
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
			    var bmiHeader = (BitmapInfoHeader) GetStreamConfigSetting( videoStreamConfig, "BmiHeader" );
				var size = new Size( bmiHeader.Width, bmiHeader.Height );
				return size;
			}
			set
			{
			    var bmiHeader = (BitmapInfoHeader) GetStreamConfigSetting( videoStreamConfig, "BmiHeader" );
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
				short audioChannels = (short) GetStreamConfigSetting(audioStreamConfig, "nChannels");
				return audioChannels;
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
		//protected PropertyPageCollection propertyPages;			// Property Backer: list of property pages exposed by filters
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
            //PropertyPages = null; // Disposal done within PropertyPages

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
				// depending on the formatType
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

