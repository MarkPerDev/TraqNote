using System.Collections.Generic;
using System.Web.Mvc;
using TraqNote.Data.Views;
using TraqNote.Service;

namespace TraqNote.Controllers
{
	public class HomeController : Controller
	{

		[Authorize]
		public ActionResult Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				return View(GetAllPosts(string.Empty));
			}
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";
			return View();
		}

		public ActionResult Contact()
		{
				ViewBag.Message = "Your contact page.";

				return View();
		}

		public ActionResult Details(int id)
		{
			// load details view based on post id
			return View();
		}

		[HttpPost]
		public ActionResult Index(string searchString)
		{
			// load details view based on post id
			return View(GetAllPosts(searchString));
		}

		#region Private methods

		private IList<Posts> GetAllPosts(string searchString)
		{
			using (var context = new PostServices())
			{
				return context.GetAllPosts(searchString);
			}
		}

		#endregion Private methods
	}
}