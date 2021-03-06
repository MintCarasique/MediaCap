using System;

namespace MediaCap.Capture
{
	/// <summary>
	///  Represents a physical connector or source on an audio/video device.
	/// </summary>
	public class Source : IDisposable
	{

		// --------------------- Private/Internal properties -------------------------

        /// <summary>
        /// Name of the source
        /// </summary>
		protected string name;


		// ----------------------- Public properties -------------------------

		/// <summary> The name of the source. Read-only. </summary>
		public string Name => name;

	    /// <summary> Obtains the String representation of this instance. </summary>
		public override string ToString() { return( Name ); }

		/// <summary> Is this source enabled. </summary>
		public virtual bool Enabled 
		{
			get => throw new NotSupportedException( "This method should be overriden in derrived classes." );
		    set => throw new NotSupportedException( "This method should be overriden in derrived classes." );
		}

		
		// -------------------- Constructors/Destructors ----------------------

		/// <summary> Release unmanaged resources. </summary>
		~Source()
		{
			Dispose();
		}


		
		// -------------------- IDisposable -----------------------

		/// <summary> Release unmanaged resources. </summary>
		public virtual void Dispose()
		{
			name = null;
		}

	}
}
