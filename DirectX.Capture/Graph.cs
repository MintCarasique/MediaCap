using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DShowNET;
using DShowNET.Device;
using System.Threading.Tasks;

namespace MediaCap.Capture
{
    public partial class Capture
    {
        protected void CreateGraph()
        {
            Guid cat;
            Guid med;
            int hr;

            // Ensure required properties are set
            if (videoDevice == null && audioDevice == null)
                throw new ArgumentException("The video and/or audio device have not been set. Please set one or both to valid capture devices.\n");

            // Skip if we are already created
            if ((int)graphState >= (int)GraphState.Created) return;

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
            if (VideoDevice != null)
            {
                videoDeviceFilter = (IBaseFilter)Marshal.BindToMoniker(VideoDevice.MonikerString);
                hr = graphBuilder.AddFilter(videoDeviceFilter, "Video Capture Device");
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Get the audio device and add it to the filter graph
            if (AudioDevice != null)
            {
                audioDeviceFilter = (IBaseFilter)Marshal.BindToMoniker(AudioDevice.MonikerString);
                hr = graphBuilder.AddFilter(audioDeviceFilter, "Audio Capture Device");
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Get the video compressor and add it to the filter graph
            if (VideoCompressor != null)
            {
                videoCompressorFilter = (IBaseFilter)Marshal.BindToMoniker(VideoCompressor.MonikerString);
                hr = graphBuilder.AddFilter(videoCompressorFilter, "Video Compressor");
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            }

            // Get the audio compressor and add it to the filter graph
            if (AudioCompressor != null)
            {
                audioCompressorFilter = (IBaseFilter)Marshal.BindToMoniker(AudioCompressor.MonikerString);
                hr = graphBuilder.AddFilter(audioCompressorFilter, "Audio Compressor");
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
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
            if (hr != 0)
            {
                // If not found, try looking for a video media type
                med = MediaType.Video;
                hr = captureGraphBuilder.FindInterface(
                    ref cat, ref med, videoDeviceFilter, ref iid, out o);
                if (hr != 0)
                    o = null;
            }
            videoStreamConfig = o as IAMStreamConfig;

            o = null;
            cat = PinCategory.Preview;
            med = MediaType.Interleaved;
            iid = typeof(IAMStreamConfig).GUID;
            hr = captureGraphBuilder.FindInterface(
                ref cat, ref med, videoDeviceFilter, ref iid, out o);

            if (hr != 0)
            {
                // If not found, try looking for a video media type
                med = MediaType.Video;
                hr = captureGraphBuilder.FindInterface(
                    ref cat, ref med, videoDeviceFilter, ref iid, out o);
                if (hr != 0)
                    o = null;
            }
            previewStreamConfig = o as IAMStreamConfig;
            o = null;
            cat = PinCategory.Capture;
            med = MediaType.Audio;
            iid = typeof(IAMStreamConfig).GUID;
            if ((_audioViaPci) &&
                (audioDeviceFilter == null) && (videoDeviceFilter != null))
            {
                hr = captureGraphBuilder.FindInterface(ref cat, ref med, videoDeviceFilter, ref iid, out o);
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
            //PropertyPages = null;

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
            Guid cat;
            Guid med;
            int hr;
            bool didSomething = false;
            const int WS_CHILD = 0x40000000;
            const int WS_CLIPCHILDREN = 0x02000000;
            const int WS_CLIPSIBLINGS = 0x04000000;

            assertStopped();

            // Ensure required properties set
            if (filename == null)
                throw new ArgumentException("The Filename property has not been set to a file.\n");

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
            if (!wantPreviewRendered && isPreviewRendered)
                DerenderGraph();
            if (!wantCaptureRendered && isCaptureRendered)
                if (wantPreviewRendered)
                    DerenderGraph();


            // Render capture stream (only if necessary)
            if (wantCaptureRendered && !isCaptureRendered)
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
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                // Render video (video -> mux) if needed or possible
                if ((VideoDevice != null) && (captureVideo))
                {
                    // Try interleaved first, because if the device supports it,
                    // it's the only way to get audio as well as video
                    cat = PinCategory.Capture;
                    med = MediaType.Interleaved;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, videoCompressorfilter, muxFilter);
                    if (hr < 0)
                    {
                        med = MediaType.Video;
                        hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, videoCompressorfilter, muxFilter);
                        if (hr == -2147220969) throw new DeviceInUseException("Video device", hr);
                        if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                    }
                }

                // Render audio (audio -> mux) if possible
                if ((audioDeviceFilter != null) && (captureAudio))
                {
                    // If this Asf file format than please keep in mind that
                    // certain Wmv formats do not have an audio stream, so
                    // when using this code, please ensure you use a format
                    // which supports audio!
                    cat = PinCategory.Capture;
                    med = MediaType.Audio;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, audioDeviceFilter, audioCompressorFilter, muxFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }
                else
                    if ((_audioViaPci) && (captureAudio) &&
                    (audioDeviceFilter == null) && (videoDeviceFilter != null))
                {
                    cat = PinCategory.Capture;
                    med = MediaType.Audio;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, audioCompressorFilter, muxFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                isCaptureRendered = true;
                didSomething = true;
            }

            // Render preview stream (only if necessary)
            if (wantPreviewRendered && !isPreviewRendered)
            {
                // Render preview (video -> renderer)
                InitVideoRenderer();
                //this.AddDeInterlaceFilter();

                // When capture pin is used, preview works immediately,
                // however this conflicts with file saving.
                // An alternative is to use VMR9
                cat = PinCategory.Preview;
                med = MediaType.Video;
                if (InitSampleGrabber())
                {
                    Debug.WriteLine("SampleGrabber added to graph.");

                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, _baseGrabFlt, _videoRendererFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }
                else
                {
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, null, _videoRendererFilter);
                    if (hr < 0) Marshal.ThrowExceptionForHR(hr);
                }

                // Special option to enable rendering audio via PCI bus
                if ((_audioViaPci) && (audioDeviceFilter != null))
                {
                    cat = PinCategory.Preview;
                    med = MediaType.Audio;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, audioDeviceFilter, null, null);
                    if (hr < 0)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }
                else
                    if ((_audioViaPci) &&
                    (audioDeviceFilter == null) && (videoDeviceFilter != null))
                {
                    cat = PinCategory.Preview;
                    med = MediaType.Audio;
                    hr = captureGraphBuilder.RenderStream(ref cat, ref med, videoDeviceFilter, null, null);
                    if (hr < 0)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }

                // Get the IVideoWindow interface
                videoWindow = graphBuilder as IVideoWindow;

                // Set the video window to be a child of the main window
                hr = videoWindow.put_Owner(previewWindow.Handle);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = videoWindow.put_WindowStyle(WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Position video window in client rect of owner window
                previewWindow.Resize += OnPreviewWindowResize;
                OnPreviewWindowResize(this, null);

                // Make the video window visible, now that it is properly positioned
                hr = videoWindow.put_Visible(DsHlp.OATRUE);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                isPreviewRendered = true;
                didSomething = true;
                SetMediaSampleGrabber();
            }

            if (didSomething)
                graphState = GraphState.Rendered;
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
            if (mediaControl != null)
                mediaControl.Stop();

            // Free the preview window (ignore errors)
            if (videoWindow != null)
            {
                videoWindow.put_Visible(DsHlp.OAFALSE);
                videoWindow.put_Owner(IntPtr.Zero);
                videoWindow = null;
            }

            // Remove the Resize event handler
            if (PreviewWindow != null)
                previewWindow.Resize -= OnPreviewWindowResize;

            if ((int)graphState >= (int)GraphState.Rendered)
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
    }
}
