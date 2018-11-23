using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.Data
{
	public static class Functions
	{
		internal const string SQL_GET_LAST_USEAGES_SHA510 = @"WITH recent_passwords AS (SELECT password FROM password_history WHERE user_id = {0} ORDER BY created_on DESC LIMIT {1}) SELECT EXISTS (SELECT 1 FROM recent_passwords WHERE password = digest({2},'sha512'))";
		internal const string SQL_GET_DIGEST_SHA512 = @"select digest({0},'sha512')";
		internal const string SQL_GET_USER_BY_USER_ID_AND_PWD_SHA512 = @"select * from users where user_id = {0} and password = digest({1},'sha512')";



	}
}
