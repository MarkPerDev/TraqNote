using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraqNote.Data;

namespace TraqNote.Service.Interfaces
{
	/// <summary>
	/// Defines signatures of methods that all services provide.
	/// </summary>
	public interface IBaseServices
	{
		TraqnoteEntities DbContext  { get; set; }
	}
}
