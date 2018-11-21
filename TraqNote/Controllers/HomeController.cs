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
				return View(GetAllPosts());
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

		#region Private methods
		private IList<Posts> GetAllPosts()
		{
			using (var context = new PostServices())
			{
				return context.GetAllPosts();
			}
		}

		#endregion Private methods
	}
}