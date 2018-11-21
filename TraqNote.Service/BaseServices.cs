using System;
using TraqNote.Data;
using TraqNote.Service.Interfaces;

namespace TraqNote.Service
{
	public class BaseServices : IBaseServices, IDisposable
	{
		private TraqnoteEntities _context = null;
		public TraqnoteEntities DbContext
		{
			get
			{
				if (_context == null)
					_context = new TraqnoteEntities();

				return _context;
			}

			set
			{
				this._context = value;
			}
		}

		#region Constructors

		/// <summary>
		/// An empty, default constructor that creates an instance of <see cref="BaseServices"/>.
		/// </summary>
		public BaseServices()
		{
			// Intentionally left empty.
		}

		/// <summary>
		/// Constructor builds an instance of <see cref="BaseServices"/> and
		/// populates the <see cref="_Context"/>.
		/// </summary>
		/// <remarks>
		/// Enables constructor based injection of the <see cref="TraqnoteEntities"/>
		/// which can enable isolated unit testing.
		/// </remarks>
		/// <param name="pDbContext">The <see cref="TraqnoteEntities"/> to assign as the
		/// service's _Context.</param>
		public BaseServices(TraqnoteEntities pDbContext)
		{
			this._context = pDbContext;
		}

		#endregion Constructors


		#region IDisposable Implementation

		/// <summary>
		/// Releases the managed resources directly held by the current instance. i.e. The
		/// <<see cref="TraqnoteEntities"/> used by this service is released, if it was
		/// instantiated internally by the current instance.
		/// </summary>
		/// <remarks>
		/// Child classes must not implement <see cref="Dispose"/>. That is covered by this base
		/// class. They should implement <see cref="Dispose(bool)"/> to proactively release any
		/// resources. If they do, they must call base.Dispose(bool) to allow for parent classes
		/// to release their resources.
		/// <para>
		/// The Entity Frameworks objects are likely to refer to some unmanaged resources. It is
		/// their responsibility to maintain those safely.
		/// </para><para>
		/// The Entity Framework follows the "Unit of Work" pattern. The TraqnoteEntities
		/// lifespan should be as short as possible. The Dispose() method allows the consumer to
		/// have some control over the lifespan of the DbContext.
		/// </para>
		/// </remarks>
		/// <see cref="_Context"/>
		public void Dispose()
		{
			Dispose(true);
			// take this object off the finalization queue 
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Conditionally releases the managed resources directly held by the current instance.
		/// i.e. The <see cref="TraqnoteEntities"/> used by this service is released, if
		/// it was instantiated internally by the current instance.
		/// </summary>
		/// <remarks>
		/// The Entity Frameworks objects are likely to refer to some unmanaged resources. It is
		/// their responsibility to maintain those safely.
		/// <para>
		/// The Entity Framework follows the "Unit of Work" pattern. The TraqnoteEntities
		/// lifespan should be as short as possible. The Dispose() methods allow the consumer to
		/// have some control over the lifespan of the DbContext.
		/// </para>
		/// </remarks>
		/// <seealso cref="_Context"/>
		protected virtual void Dispose(bool disposing)
		{
			if (this._context != null)
			{
				this._context.Dispose();
				this._context = null;
			}
		}

		#endregion IDisposable Implementation
	}
}
