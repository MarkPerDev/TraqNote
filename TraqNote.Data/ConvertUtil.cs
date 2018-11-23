using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.Data
{
	public static class ConvertUtil
	{
		public static char ToChar(this int i)
		{
			if (i < 10)
			{
				return (char)(0x30 + i); //48='0'
			}
			return (char)(0x37 + i); //(i - 10) + 'A'
		}
	}
}
