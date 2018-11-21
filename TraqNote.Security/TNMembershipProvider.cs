using System;

using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Security;
using TraqNote.Data;
using TraqNote.Service;


namespace TraqNote.Security
{
	public class TNMembershipProvider : MembershipProvider, IDisposable
	{

		private readonly int newPasswordLength = 8;
		private string connectionString;
		private string applicationName;
		private bool enablePasswordReset;
		private bool enablePasswordRetrieval;
		private bool requiresQuestionAndAnswer;
		private bool requiresUniqueEmail;
		private int maxInvalidPasswordAttempts;
		private int passwordAttemptWindow;
		private MembershipPasswordFormat passwordFormat;
		private int minRequiredNonAlphanumericCharacters;
		private int minRequiredPasswordLength;
		private string passwordStrengthRegularExpression = string.Empty;
		private MachineKeySection machineKey; //Used when determining encryption key values.

		public const string CACHE_KEY_SESSION_DATA = @"keySessionData:";
		public const string CONFIG_KEY_CONNECTION_STRING_NAME = @"connectionStringName";

		private static object _lockSessionUpdate = new object();

		private enum FailureType
		{
			Password = 1,
			PasswordAnswer = 2
		}
		public override string ApplicationName
		{
			get
			{
				return applicationName;
			}
			set
			{
				applicationName = value;
			}
		}

		public override bool EnablePasswordReset
		{
			get
			{
				return enablePasswordReset;
			}
		}

		public override bool EnablePasswordRetrieval
		{
			get
			{
				return enablePasswordRetrieval;
			}
		}

		public override bool RequiresQuestionAndAnswer
		{
			get
			{
				return requiresQuestionAndAnswer;
			}
		}

		public override bool RequiresUniqueEmail
		{
			get
			{
				return requiresUniqueEmail;
			}
		}

		public override int MaxInvalidPasswordAttempts
		{
			get
			{
				return maxInvalidPasswordAttempts;
			}
		}

		public override int PasswordAttemptWindow
		{
			get
			{
				return passwordAttemptWindow;
			}
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get
			{
				return passwordFormat;
			}
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get
			{
				return minRequiredNonAlphanumericCharacters;
			}
		}

		public override int MinRequiredPasswordLength
		{
			get
			{
				return minRequiredPasswordLength;
			}
		}

		public override string PasswordStrengthRegularExpression
		{
			get
			{
				return passwordStrengthRegularExpression;
			}
		}

		//public LoginAttempt CurrentLoginAttempt
		//{
		//	get;
		//	set;
		//}

		public string ChangePasswordError
		{
			get;
			set;
		}

		public string ResetPasswordError
		{
			get;
			set;
		}

		public string ResetPasswordValue
		{
			get;
			set;
		}

		private string ENCODING_NAME = @"ISO-8859-1";
		private Encoding ENCODING
		{
			get
			{
				return Encoding.GetEncoding(ENCODING_NAME);
			}
		}

		private string GetConfigValue(string configValue, string defaultValue)
		{
			if (String.IsNullOrEmpty(configValue))
			{
				return defaultValue;
			}

			return configValue;
		}



		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}

			if (name == null || name.Length == 0)
			{
				name = "TNMembershipProvider";
			}

			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "TN Membership provider");
			}

			//Initialize the abstract base class.
			base.Initialize(name, config);

			applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
			minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "8"));
			passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], String.Empty));
			enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
			requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
			requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

			string temp_format = config["passwordFormat"];
			if (temp_format == null)
			{
				temp_format = "Hashed";
			}

			switch (temp_format)
			{
				case "Hashed":
					passwordFormat = MembershipPasswordFormat.Hashed;
					break;
				case "Encrypted":
					passwordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Clear":
					passwordFormat = MembershipPasswordFormat.Clear;
					break;
				default:
					throw new ProviderException("Password format not supported.");
			}

			ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config[CONFIG_KEY_CONNECTION_STRING_NAME]];

			if ((ConnectionStringSettings == null) || (ConnectionStringSettings.ConnectionString.Trim() == String.Empty))
			{
				throw new ProviderException("Connection string cannot be blank.");
			}

			connectionString = ConnectionStringSettings.ConnectionString;

			//Get encryption and decryption key information from the configuration.
			System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			machineKey = cfg.GetSection("system.web/machineKey") as MachineKeySection;

#if RELEASE
			if (machineKey.ValidationKey.Contains("AutoGenerate"))
      {
        if (PasswordFormat != MembershipPasswordFormat.Clear)
        {
          throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
        }
      }
#endif
		}


		//public override bool EnablePasswordRetrieval => throw new NotImplementedException();

		//public override bool EnablePasswordReset => throw new NotImplementedException();

		//public override bool RequiresQuestionAndAnswer => throw new NotImplementedException();

		//public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		//public override int MaxInvalidPasswordAttempts => throw new NotImplementedException();

		//public override int PasswordAttemptWindow => throw new NotImplementedException();

		//public override bool RequiresUniqueEmail => throw new NotImplementedException();

		//public override MembershipPasswordFormat PasswordFormat => throw new NotImplementedException();

		////public override int MinRequiredPasswordLength => throw new NotImplementedException();

		//public override int MinRequiredNonAlphanumericCharacters => throw new NotImplementedException();

		//public override string PasswordStrengthRegularExpression => throw new NotImplementedException();

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			throw new NotImplementedException();
		}

		//public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		//{
		//	throw new NotImplementedException();
		//}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override int GetNumberOfUsersOnline()
		{
			throw new NotImplementedException();
		}

		public override string GetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		//public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		//{
		//	throw new NotImplementedException();
		//}

		//public override MembershipUser GetUser(string username, bool userIsOnline)
		//{
		//	throw new NotImplementedException();
		//}

		//public override string GetUserNameByEmail(string email)
		//{
		//	throw new NotImplementedException();
		//}

		public override string ResetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override bool UnlockUser(string userName)
		{
			throw new NotImplementedException();
		}

		public override void UpdateUser(MembershipUser user)
		{
			throw new NotImplementedException();
		}

		//public override bool ValidateUser(string username, string password)
		//{
		//	throw new NotImplementedException();
		//}

		public override MembershipUser CreateUser(string username, string password,
				 string email, string passwordQuestion, string passwordAnswer,
				 bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			ValidatePasswordEventArgs args =
				 new ValidatePasswordEventArgs(username, password, true);
			OnValidatingPassword(args);

			if (args.Cancel)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			MembershipUser user = GetUser(username, true);

			if (user == null)
			{
				var currentUser= new user();
				currentUser.user_name = username;
				//currentUser.password = GetMD5Hash(password);
				//currentUser.password = password;
				currentUser.email_address = email;

				status = MembershipCreateStatus.Success;

				return GetUser(username, true);
			}
			else
			{
				status = MembershipCreateStatus.DuplicateUserName;
			}

			return null;
		}
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			var currentUser = new user();
			using (var svc = new AccountServices())
			{
				var user = svc.GetAllUsers().SingleOrDefault(u => u.UserName == username);
				if (user != null)
				{
					MembershipUser memUser = new MembershipUser("TNMembershipProvider",
																					 username, user.UserId, "markpero45@gmail.com",
																					 string.Empty, string.Empty,
																					 true, false, DateTime.MinValue,
																					 DateTime.MinValue,
																					 DateTime.MinValue,
																					 DateTime.Now, DateTime.Now);
					return memUser;
				}
			}
			return null;
		}

		public override bool ValidateUser(string username, string password)
		{
			//string sha1Pswd = GetMD5Hash(password);
			//var currentUser = new user();
			//UserObj userObj = user.GetUserObjByUserName(username, sha1Pswd);
			//if (userObj != null)
			//	return true;
			return true;
		}

		//public override int MinRequiredPasswordLength
		//{
		//	get { return 6; }
		//}

		//public override bool RequiresUniqueEmail
		//{
		//	// In a real application, you will essentially have to return true
		//	// and implement the GetUserNameByEmail method to identify duplicates
		//	get { return false; }
		//}

		public static string GetMD5Hash(string value)
		{
			MD5 md5Hasher = MD5.Create();
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			throw new NotImplementedException();
		}

		public override string GetUserNameByEmail(string email)
		{
			throw new NotImplementedException();
		}
	}
}
