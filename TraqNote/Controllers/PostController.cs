using System.Web.Mvc;
using TraqNote.Data.Views;
using TraqNote.Service;

namespace TraqNote.Controllers
{
	public class PostController : Controller
	{
		// GET: Post
		public ActionResult Index()
		{
		return RedirectToAction("Index", "Home");
		}

		// GET: Post/Create
		public ActionResult Create()
		{
		return View();
		}

		public ActionResult Delete(int id)
		{
			if (ModelState.IsValid)
			{
				using (var context = new PostServices())
				{
					context.DeletePost(id);
				}
			}
			// Go to home page after a post has been created
			return RedirectToAction("Index");
		}

		public ActionResult CreatePost()
		{
			ViewBag.Message = "Create new post";

			return View();
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult CreatePost(Posts posts)
		{
			if (ModelState.IsValid)
			{
				using (var context = new PostServices())
				{
					context.SavePost(posts);
				}
			}
			// Go to home page after a post has been created
			return RedirectToAction("Index");
		}


		// POST: Post/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		[HttpGet]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}

			Posts post = null;
			if (ModelState.IsValid)
			{
				using (var context = new PostServices())
				{
				post = context.GetPost(id.Value);
				}
			}
			return View(post);
		}

		// POST: Post/Edit/5
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(Posts posts)
		{
			if (ModelState.IsValid)
			{
				using (var context = new PostServices())
				{
					context.SaveEdit(posts);
				}
			}
			// Go to home page after a post has been created
			return RedirectToAction("Index", "Home");
		}

		// POST: Post/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
