using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using TraqNote.Data;
using TraqNote.Security;

namespace TraqNote.Security
{
	public class UserInfo
	{
				TraqnoteEntities _context;
        MembershipUser _AspUser;
        user _dbUser;

				public const string User = "User";
				public const string AuthError = "AuthError";
				public const string Time = "PreviousTime";
				public const string ZoneInfo = "ZoneInfo";

				public const string Sale = "TodaySale";
				public const string SaleID = "SaleID";
				//public const string Time = "TodayTime";
				public const string InventoryEmpty = "InventoryEmpty";
				//public const string ZoneInfo = "ZoneInfo";
				public const string UserID = "UserID";
				public const string MessageSetterMsg = "MessageSetter";
				public const string MessageSetterType = "MessageSetterType";

		public UserInfo() {
            _context = new TraqnoteEntities();
        }

        //public bool IsUserExist(string log) {
        //    if (_context.users.Any(n => n.LogName == log)) {
        //        return true;
        //    }
        //    return false;
        //}

        //public bool IsUserExist(int id) {
        //    if (_context.users.Any(n => n.UserID == id)) {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        ///     Returns custom database user.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public user GetUser(int userid) {
            if (_dbUser != null && _dbUser.user_id == userid) {
                return _dbUser;
            }
            return _dbUser = _context.users.Find(userid);
        }

        /// <summary>
        ///     Get current logged user record from db.
        /// </summary>
        /// <returns></returns>
        public user GetUser() {
            if (IsAuthenticated()) {
                return GetUser(GetAspUserCurrentUser().Identity.Name);
            }
            return null;
        }

        /// <summary>
        ///     Returns the custom database user.
        /// </summary>
        /// <param name="log">By log name.</param>
        /// <returns></returns>
        public user GetUser(string log) {
            if (_dbUser != null && _dbUser.user_name == log) {
                return _dbUser;
            }
            _dbUser = _context.users.FirstOrDefault(c => c.user_name == log);
            return _dbUser;
        }
        /// <summary>
        /// Returns all admin users from the database.
        /// </summary>
        /// <returns></returns>
       // public IQueryable<user> GetAdmins() { return _context.users.Where(c => c.IsAccessToAdmin); }

        //public DateTime? LastActive(string log) {
        //    if (_dbUser != null && _dbUser.LogName == log) {
        //        return _dbUser.LastLogIn;
        //    }

        //    _dbUser = GetUser(log);
        //    if (_dbUser != null) {
        //        return _dbUser.LastLogIn;
        //    }
        //    return null;
        //}

        public bool IsAuthenticated() {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

       
        public bool IsCurrentUserSessionExist() {
            if (IsAuthenticated() && HttpContext.Current.Session != null && HttpContext.Current.Session[User] != null) {
                var user = (user)HttpContext.Current.Session[User];
                if (user == null) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        ///     If db user exist on the session return from session
        ///     or else get it and then save it to the session.
        /// </summary>
        /// <returns></returns>
        public user GetUserSession() {
            user user;
            if (IsCurrentUserSessionExist()) {
                user = (user) HttpContext.Current.Session[User];
                if (user != null) {
                    return user;
                }
            }
            user = GetUser(GetAspUserCurrentUser()
                               .Identity.Name);
            if (user == null) {
                return null;
            }
            HttpContext.Current.Session[User] = user;
            return user;
        }

        /// <summary>
        ///     Get user id from session or asp.net->db->session(keep);
        /// </summary>
        /// <returns>Return -1 when not found.</returns>
        public int GetUserID() {
            var useridCookie = HttpContext.Current.Request.Cookies[UserID];

            if (useridCookie != null) {
                int userid;
                if (int.TryParse(useridCookie.Value, out userid)) {
                    return userid;
                }
            }
            user user = GetUserSession();
            if (user != null) {
                var cookieUser = new HttpCookie(UserID);
                cookieUser.Value = user.user_id.ToString();
                cookieUser.Expires = DateTime.Now.AddDays(60);
                HttpContext.Current.Response.Cookies.Set(cookieUser);
                return user.user_id;
            }
            return -1;
        }


        public string AuthenticatedUserName() {
            if (IsAuthenticated()) {
                return GetAspUserCurrentUser()
                    .Identity.Name;
            }
            return "";
        }

        public bool IsAuthenticated(string log) {
            if (_AspUser != null && log == _AspUser.UserName) {
                return _AspUser.IsOnline;
            }
            _AspUser = Membership.GetUser(log);
            return _AspUser != null && _AspUser.IsOnline;
        }

        public MembershipUser GetAspUser(string log) {
            if (_AspUser != null && log == _AspUser.UserName) {
                return _AspUser;
            }
            _AspUser = Membership.GetUser(log);
            return _AspUser;
        }

        public IPrincipal GetAspUserCurrentUser() {
            if (HttpContext.Current.User.Identity.IsAuthenticated) {
                return HttpContext.Current.User;
            }

            return null;
        }
    }
}