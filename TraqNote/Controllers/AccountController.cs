using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TraqNote.Data.Views;
using TraqNote.Security;

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
						//var user = new user();
						//User user = userinfo.GetUserSession();
						//user.LastLogIn = DateTime.UtcNow;
						//using (var db = new JustFoodDBEntities())
						//{
						//	db.Entry(user).State = EntityState.Detached;
						//	db.Entry(user).State = EntityState.Modified;
						//	db.SaveChanges();
						//}
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
			HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
			cookie1.Expires = DateTime.Now.AddYears(-1);
			Response.Cookies.Add(cookie1);
				
			// clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
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
	}
}
