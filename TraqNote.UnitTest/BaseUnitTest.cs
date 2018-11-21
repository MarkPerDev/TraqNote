using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.UnitTest
{
	public class BaseUnitTest
	{
		#region Abstract Methods

		/// <summary>
		/// Virtual method for common unit test clean up
		/// </summary>
		public virtual void CleanUpTestData()
		{
			// Not implemented locally yet
		}

		#endregion
	}
}
