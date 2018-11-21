using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraqNote.Data;
using TraqNote.Data.Views;

namespace TraqNote.Service
{
	public class PostServices : BaseServices
	{
		#region Constructors

		/// <summary>
		/// An empty, default constructor that creates an instance of <see cref="PostServices"/>.
		/// </summary>
		public PostServices()
			: base()
		{
			// Intentionally left empty
		}

		/// <summary>
		/// Constructor builds an instance of <see cref="PostServices"/> and
		/// populates the <see cref="BaseServices.DbContext"/>.
		/// </summary>
		/// <remarks>
		/// Enables constructor based injection of the <see cref="TraqnoteEntities"/>
		/// which can enable isolated unit testing.
		/// </remarks>
		/// <param name="pContext">The <see cref="RolloutManagementContext"/> to assign as the
		/// <see cref="BaseServices.RmsDbContext"/>.</param>
		public PostServices(TraqnoteEntities pContext)
			: base(pContext)
		{
			// Intentionally left empty
		}

		#endregion Constructors

		/// <summary>
		/// Get all posts
		/// </summary>
		/// <returns></returns>
		public IList<Posts> GetAllPosts()
		{
			return DbContext.posts.Select(x =>
									new Posts()
									{
										PostId = x.id,
										Title = x.title.Substring(0, 20),
										Content = x.content.Substring(0, 20),
										Topic_Id = x.topic_id,
										TopicName = x.topic.topic_name
									}).OrderBy(z => z.TopicName).ToList();
		}

		public void DeletePost(int id)
		{
			var p = DbContext.posts.FirstOrDefault(x => x.id == id);
			DbContext.posts.Remove(p);
			DbContext.SaveChanges();
		}

		/// <summary>
		/// Gets a post based on <paramref name="id"/>
		/// </summary>
		public Posts GetPost(int id)
		{
			var post = DbContext.posts.FirstOrDefault(x => x.id == id);

			// TODO - Check for null. Load ero
			return new Posts()
			{
				PostId = post.id,
				Title = post.title,
				Content = post.content,
				Topic_Id = post.topic_id,
				TopicName = post.topic.topic_name,
				Created_On = post.created_on
			};
		}

		/// <summary>
		/// Gets the detail summary based on <paramref name="id"/>
		/// </summary>
		public Summary GetDetailSummary(int id)
		{
			var sb = new StringBuilder();
			var post = DbContext.posts.FirstOrDefault(x => x.id == id);

			sb.AppendLine(string.Format("Topic: {0}", post.topic.topic_name));
			sb.AppendLine(string.Format("Title: {0}", post.title));
			sb.AppendLine(post.content);

			return new Summary()
			{
				SummaryDetail = sb.ToString()
			};
		}
	

		/// <summary>
		/// Saves a post
		/// </summary>
		/// <param name="post"></param>
	public void SavePost(Posts post)
	{
		var p = new post();
    p.topic_id = post.Topic_Id.Value;
    p.title = post.Title;
    p.content = post.Content;
		p.created_on = DateTime.Now;

		DbContext.posts.Add(p);
		DbContext.SaveChanges();
	}

		/// <summary>
		/// Saves a post after an edit
		/// </summary>
		/// <param name="post"></param>
		public void SaveEdit(Posts post)
		{
			var p = DbContext.posts.FirstOrDefault(x => x.id == post.PostId);
			p.topic_id = post.Topic_Id.Value;
			p.title = post.Title;
			p.content = post.Content;
			p.modified_on = DateTime.Now;

			DbContext.SaveChanges();
		}
	}
}
