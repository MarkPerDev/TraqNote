using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using TraqNote.Data;
using TraqNote.Service;
using TraqNote.Data.Views;


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

		//private static object _lockSessionUpdate = new object();

		private string _newUserFirstName;
		private string _newUserLastName;


		public TNMembershipProvider()
		{
			// Intentionally left empty 
		}

		public TNMembershipProvider(string firstName, string lastName)
		{
			_newUserFirstName = firstName;
			_newUserLastName = lastName;
		}


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
				return MembershipPasswordFormat.Hashed;
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

		public string ChangePasswordError	{get;	set;}

		public string ResetPasswordError { get; set; }

		public string ResetPasswordValue { get; set; }

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

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			throw new NotImplementedException();
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
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
				using (var context = new AccountServices())
				{
					var newUserReg = new Registration()
					{
						ActivationCode = Guid.NewGuid(),
						Username = username,
						PasswordByte = EncodePasswordBytes(password),
						FirstName = _newUserFirstName,
						LastName = _newUserLastName,
						Email = email
					};

					context.CreateNewUser(newUserReg);
				}

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
			using (var svc = new AccountServices())
			{
				var user = svc.GetAllUsers().SingleOrDefault(u => u.UserName == username);
				if (user != null)
				{
					MembershipUser memUser = new MembershipUser("TNMembershipProvider",
																					 username, user.UserId, user.EmailAddress,
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
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				return false;
			}

			using (var dbContext = new TraqnoteEntities())
			{

				var passwordEncoded = EncodePasswordBytes(password);


				var user = (from us in dbContext.users
										where string.Compare(username, us.user_name, StringComparison.OrdinalIgnoreCase) == 0
										&& us.password == passwordEncoded
										&& us.active == true
										select us).FirstOrDefault();

				return (user != null);
			}
		}

		/// <summary>
		/// UnEncode password.
		/// </summary>
		/// <param name="encodedPassword">Password.</param>
		/// <returns>Unencoded password.</returns>
		private string UnEncodePassword(string encodedPassword)
		{
			string password = encodedPassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					password = ENCODING.GetString(DecryptPassword(HexToByte(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					throw new ProviderException("Cannot unencode a hashed password.");
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return password;
		}

		/// <summary>
		/// Encode password.
		/// </summary>
		/// <param name="password">Password.</param>
		/// <returns>Encoded password.</returns>
		public byte[] EncodePasswordBytes(string password)
		{
			byte[] encodedPassword = ENCODING.GetBytes(password);

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					encodedPassword =
						EncryptPassword(ENCODING.GetBytes(password));
					break;
				case MembershipPasswordFormat.Hashed:
					using (var context = new TraqnoteEntities())
					{
						encodedPassword = context.GetDigestSHA512(password).FirstOrDefault();
					}
					break;
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return encodedPassword;
		}

		/// <summary>
		/// UnEncode password.
		/// </summary>
		/// <param name="encodedPassword">Password.</param>
		/// <returns>Unencoded password.</returns>
		public byte[] UnEncodePasswordBytes(byte[] encodedPassword)
		{
			byte[] password = encodedPassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					password = DecryptPassword(password);
					break;
				case MembershipPasswordFormat.Hashed:
					throw new ProviderException("Cannot unencode a hashed password.");
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return password;
		}

		//public string GetMD5HashStringFromByte(byte[] bytes, bool upperCase)
		//{
		//	StringBuilder result = new StringBuilder(bytes.Length * 2);

		//	for (int i = 0; i < bytes.Length; i++)
		//		result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

		//	return result.ToString();
		//}

		//public string GetMD5HashByteFromString(, bool upperCase)
		//{
		//	StringBuilder result = new StringBuilder(bytes.Length * 2);

		//	for (int i = 0; i < bytes.Length; i++)
		//		result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

		//	return result.ToString();
		//}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			throw new NotImplementedException();
		}

		public override string GetUserNameByEmail(string email)
		{
			using (var context = new AccountServices())
			{
				return context.GetUserNameByEmail(email);
			}
		}

		/// <summary>
		/// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private static byte[] HexToByte(string hexString)
		{
			byte[] returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			return returnBytes;
		}

		/// <summary>
		/// Converts a byte array to a hexadecimal string.
		/// </summary>
		/// <param name="ba"></param>
		/// <returns></returns>
		public static string ByteToHex(byte[] array)
		{
			//string hex = BitConverter.ToString(ba); 
			//return hex.Replace("-", ""); 
			int length = array.Length; // length = array.Length*2
			length += length;

			var chars = new char[length];
			int bIdx = 0;
			for (int index = 0; index < length; index += 2)
			{
				byte b = array[bIdx++];
				chars[index] = (b / 16).ToChar();
				chars[index + 1] = (b % 16).ToChar();
			}
			return new string(chars, 0, length).ToLower();
		}

		#region IDisposable Implementation
		public void Dispose()
		{
			//	Dispose(true);
			//	// take this object off the finalization queue 
			//	// and prevent finalization code for this object
			//	// from executing a second time.
			//	GC.SuppressFinalize(this);
			//throw new NotImplementedException();
		}

		#endregion IDisposable Implementation
	}
}
