using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraqNote.Data;
using TraqNote.Data.Views;

namespace TraqNote.Service
{
	public class AccountServices : BaseServices
	{
		// Default user
		// U: markp p: HendriX24

		/// <summary>
		/// Get all users
		/// </summary>
		/// <returns></returns>
		public IList<Login> GetAllUsers()
		{
			return DbContext.users.Select(x =>
									new Login()
									{
										UserId = x.user_id,
										UserName = x.user_name,
										EmailAddress = x.email_address,
										Password = "blah",
										UseEmail = true,
									}).OrderBy(z => z.UserId).ToList();
		}

		/// <summary>
		/// Create new user
		/// </summary>
		/// <returns></returns>
		public void CreateNewUser(Registration registration)
		{
			var user = new user()
			{
				user_name = registration.Username,
				first_name = registration.FirstName,
				last_name = registration.LastName,
				email_address = registration.Email,
				password = registration.PasswordByte,
				activation_code = Guid.NewGuid().ToString(),
				active = true
			};

				DbContext.users.Add(user);
				DbContext.SaveChanges();
		}

		/// <summary>
		/// Retrieves the user name based on the email address
		/// </summary>
		/// <returns></returns>
		public string GetUserNameByEmail(string emailAddress)
		{
			var result = DbContext.users.FirstOrDefault(x => x.email_address == emailAddress);
			return (result != null) ? result.user_name : null;
		}
	}
}
