using System;
using System.Collections.Generic;
using System.Linq;
using TraqNote.Data;
using TraqNote.Data.Views;

namespace TraqNote.Service
{
	public class AccountServices : BaseServices
	{
		/// <summary>
		/// Get all topics
		/// </summary>
		/// <returns></returns>
		public IList<Login> GetAllUsers()
		{
			return DbContext.users.Select(x =>
									new Login()
									{
										UserId = x.user_id,
										UserName = x.user_name,
										Password = "blah",
										UseEmail = true,
									}).OrderBy(z => z.UserId).ToList();
		}
	}
}
