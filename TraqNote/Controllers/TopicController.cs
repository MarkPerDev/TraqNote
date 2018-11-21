using System.Linq;
using System.Web.Mvc;
using TraqNote.Data;
using TraqNote.Data.Views;
using TraqNote.Service;

namespace TraqNote.Controllers
{
	public class TopicController : Controller
	{
		public ActionResult Index()
		{
			using (var context = new TopicServices())
			{
				var topics = context.GetAllTopics();
				return Json(topics, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult CreateTopic()
		{
			ViewBag.Message = "Create new topic";

			return View();
		}

		[HttpPost]
		public ActionResult CreateTopic(Topics topics)
		{
			if (ModelState.IsValid)
			{
				using (var DBContext = new TraqnoteEntities())
				{
					if (DBContext.topics.Any(x => x.topic_name.ToLower() == topics.TopicName.ToLower()))
					{
						ModelState.AddModelError("", $"Topic already exists: {topics.TopicName}");

						return View();
					}

					using (var context = new TopicServices())
					{
						context.SaveTopic(topics);
					}
				}
			}

			// Go to home page after a post has been created
			return RedirectToAction("CreatePost", "Post");
		}
	}
}