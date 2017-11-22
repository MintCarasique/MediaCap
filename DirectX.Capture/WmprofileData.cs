// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
// 2006-March-1 HV - created
//
// 2008-Apr-8   HV - modified
// - profile filename attribute added
//
// Copyright (C) 2006, 2007, 2008 Hans Vosman
// ------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace MediaCap.Capture
{
	/// <summary>
	/// Summary description for WMProfileData
	/// </summary>
	public class WMProfileData
	{
		/// <summary>
		/// Name of the profile
		/// Must be valid and an unique name
		/// </summary>
		protected string name;

		/// <summary>
		/// Guid of the profile
		/// Must be valid and an unique guid
		/// </summary>
		protected Guid guid;

		/// <summary>
		/// Description of the profile
		/// Might be interesting to know when selecting a format
		/// </summary>
		protected string description;

		/// <summary>
		/// Audio bit rate
		/// Might be interesting to know when selecting a format
		/// </summary>
		protected int audioBitrate;

		/// <summary>
		/// Video bit rate
		/// Might be interesting to know when selecting a format
		/// </summary>
		protected int videoBitrate;

		/// <summary>
		/// Profile filename, a profile must have a guid or a filename
		/// </summary>
		protected string filename;

		/// <summary>
		/// Indicates whether this profile supports an audio stream
		/// </summary>
		protected bool audio;

		/// <summary>
		/// Indicates whether this profile supports an video stream
		/// </summary>
		protected bool video;

		/// <summary>
		/// Indicates whether this profile is the one currently in use
		/// </summary>
		protected bool enabled;

		/// <summary>
		/// Name of profile
		/// </summary>
		public string Name
		{
			get => name;
		    set
			{
				if(!string.IsNullOrEmpty(value))
				{
					// In future check whether the name is unique
					name = value;
				}
			}
		}

		/// <summary>
		/// Guid of profile (might be a null value)
		/// </summary>
		public Guid Guid => guid;

	    /// <summary>
		/// Description of profile
		/// </summary>
		public string Description => description;

	    /// <summary>
		/// Video bit rate value
		/// </summary>
		public int VideoBitrate => videoBitrate;

	    /// <summary>
		/// Audio bit rate value
		/// </summary>
		public int AudioBitrate => audioBitrate;

	    /// <summary>
		/// Indicates whether profile supports audio
		/// </summary>
		public bool Audio => audio;

	    /// <summary>
		/// Indicates whether profile supports video
		/// </summary>
		public bool Video => video;

	    /// <summary>
		/// Enabled flag
		/// </summary>
		public bool Enabled
		{
			get => enabled;
	        set => enabled = value;
	    }

		/// <summary>
		/// Filename profile
		/// </summary>
		public string Filename
		{
			get => filename;
		    set => filename = value;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		internal WMProfileData()
		{
			enabled = false;
		}

		/// <summary>
		/// Initialize profile data
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="audioBitrate"></param>
		/// <param name="videoBitrate"></param>
		/// <param name="audio"></param>
		/// <param name="video"></param>
		internal WMProfileData(Guid guid, string name, string description, int audioBitrate, int videoBitrate, bool audio, bool video)
		{
			this.guid = guid;
			this.name = name;
			this.description = description;
			this.audioBitrate = audioBitrate;
			this.videoBitrate = videoBitrate;
			this.audio = audio;
			this.video = video;
			enabled = false;
#if DEBUG
			Debug.WriteLine(
				"\"" + guid + "\", \"" + name + "\", \"" + description + "\", " +
				audioBitrate + ", " + videoBitrate + ", " +
				audio + ", " + video + ", enabled=" + enabled);
#endif
		}

		/// <summary> Release resources. </summary>
		public virtual void Dispose()
		{
			name = null;
			description = null;
			filename = null;
		}
	}
}
