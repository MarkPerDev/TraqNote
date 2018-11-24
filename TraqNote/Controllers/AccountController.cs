using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TraqNote.Data.Views;
using TraqNote.Security;
using System.Net;
using System.Net.Mail;
using TraqNote.Data;
using System.Data.Entity;

namespace TraqNote.Controllers
{
	public class AccountController : Controller
	{
		[AllowAnonymous]
		[HttpPost]
		public ActionResult Login(Login model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					if (Url.IsLocalUrl(returnUrl) && User.Identity.IsAuthenticated)
					{
						//save last login
						var userinfo = new UserInfo();
						var user = userinfo.GetUserSession();
						using (var db = new TraqnoteEntities())
						{
							db.Entry(user).State = EntityState.Detached;
							db.Entry(user).State = EntityState.Modified;
							db.SaveChanges();
						}
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			HttpContext.Response.Cookies.Clear();
			// clear authentication cookie
			HttpCookie currentCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
			currentCookie.Expires = DateTime.Now.AddYears(-1);
			Response.Cookies.Add(currentCookie);
				
			// clear session cookie
			HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
			cookie2.Expires = DateTime.Now.AddYears(-1);
			Response.Cookies.Add(cookie2);

			FormsAuthentication.RedirectToLoginPage();
			return RedirectToAction("Index", "Home");
		}


		// GET: Account
		public ActionResult Login()
	{
			return View();
	}

		// GET: Account/Details/5
		public ActionResult Details(int id)
		{
				return View();
		}

		// GET: Account/Create
		public ActionResult Create()
		{
				return View();
		}

		// POST: Account/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
				try
				{
						// TODO: Add insert logic here
						return RedirectToAction("Index");
				}
				catch
				{
						return View();
				}
		}

		// GET: Account/Edit/5
		public ActionResult Edit(int id)
		{
				return View();
		}

		// POST: Account/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
				try
				{
						// TODO: Add update logic here

						return RedirectToAction("Index");
				}
				catch
				{
						return View();
				}
		}

		// GET: Account/Delete/5
		public ActionResult Delete(int id)
		{
				return View();
		}

		// POST: Account/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
				try
				{
						// TODO: Add delete logic here

						return RedirectToAction("Index");
				}
				catch
				{
						return View();
				}
		}

		[HttpGet]
		public ActionResult Registration()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Registration(Registration reg)
		{
			VerificationEmail(reg.Email, reg.ActivationCode.ToString());
			return View(reg);

			bool statusRegistration = false;
			string messageRegistration = string.Empty;
			MembershipCreateStatus status;

			if (ModelState.IsValid)
			{
				// Email Verification
				string userName = Membership.GetUserNameByEmail(reg.Email);
				if (!string.IsNullOrEmpty(userName))
				{
					ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
					return View(reg);
				}

				//Save User Data 
				using (var context = new TNMembershipProvider(reg.FirstName, reg.LastName))
				{
					context.CreateUser(reg.Username, reg.Password, reg.Email, "", "", true, null, out status);
				}

				//Verification Email
				VerificationEmail(reg.Email, reg.ActivationCode.ToString());
				messageRegistration = "Your account has been created successfully. ^_^";
				statusRegistration = true;
			}
			else
			{
				messageRegistration = "Something Wrong!";
			}
			ViewBag.Message = messageRegistration;
			ViewBag.Status = statusRegistration;

			return View(reg);
		}

		[NonAction]
		public void VerificationEmail(string email, string activationCode)
		{
			var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
			var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

			var fromEmail = new MailAddress("@gmail.com", "Activation Account - TraqNote");
			var toEmail = new MailAddress(email);

			var fromEmailPassword = "";
			string subject = "Activation Account !";

			string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
			};

			using (var message = new MailMessage(fromEmail, toEmail)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true

			})

				smtp.Send(message);

		}
	}
}
