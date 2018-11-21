using System.Web.Mvc;
using TraqNote.Data.Views;
using TraqNote.Service;

namespace TraqNote.Controllers
{
	public class SummaryController : Controller
  {
		// GET: Summary
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Home");
		}

		// GET: Post/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Index", "Home");
			}

			Summary sd = null;
			if (ModelState.IsValid)
			{
				using (var context = new PostServices())
				{
					sd = context.GetDetailSummary(id.Value);
				}
			}
			return View(sd);
		}
  }
}
