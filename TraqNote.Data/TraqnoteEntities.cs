using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Data.Entity;
using TraqNote.Data.Views;

namespace TraqNote.Data
{
	public partial class TraqnoteEntities
	{
		public DbContextTransaction BeginTransaction()
		{

			if (Database == null)
				return null;

			return Database.BeginTransaction();
		}

		public bool IsInTransaction
		{
			get { return (Database.CurrentTransaction != null); }
		}

		public void EnsureEntityKeyIsDetached(object entity)
		{
			if (entity == null) return;

			var entry = Entry(entity);

			if ((entry != null) && (entry.State != EntityState.Detached) && (entry.State != EntityState.Added))
			{
				entry.State = EntityState.Detached;
			}
		}

		/// <summary>
		/// Attach an EntityObject that was modified when detached
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objectDetached">DetachedObject</param>
		public object AttachUpdated(object objectDetached)
		{
			var entry = Entry(objectDetached);

			if (entry != null && (entry.State == EntityState.Detached))
			{
				entry.State = EntityState.Modified;
			}
			return objectDetached;
		}

		/// <summary>
		/// Returns true if the object is not being tracked by the DbContext or it is detached.
		/// </summary>
		public bool IsDetached(object entityObject)
		{
			if (entityObject == null) throw new ArgumentNullException(@"entityObject");

			var entry = this.Entry(entityObject);

			return (entry == null) || (entry.State == EntityState.Detached);
		}

		/// <summary>
		/// Returns true if the object is not being tracked by the DbContext or it is detached, or added.
		/// </summary>
		public bool IsDetachedOrAdded(object entityObject)
		{
			if (entityObject == null) throw new ArgumentNullException(@"entityObject");

			var entry = this.Entry(entityObject);

			return (entry == null) || (entry.State == EntityState.Detached) || (entry.State == EntityState.Added);
		}

		/// <summary>
		/// Returns true if the object is being tracked by the DbContext and its state is "deleted".
		/// </summary>
		public bool IsDeleted(object entityObject)
		{
			if (entityObject == null) throw new ArgumentNullException(@"entityObject");

			var entry = this.Entry(entityObject);

			return (entry != null) && (entry.State == EntityState.Deleted);
		}
	}
}
