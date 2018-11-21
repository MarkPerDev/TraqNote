using System;
using System.Collections.Generic;
using System.Linq;
using TraqNote.Data;
using TraqNote.Data.Views;

namespace TraqNote.Service
{
	public class TopicServices : BaseServices
  {
		/// <summary>
		/// Get all topics
		/// </summary>
		/// <returns></returns>
		public IList<Topics> GetAllTopics()
		{
				return DbContext.topics.Select(x =>
										new Topics()
										{
											Id = x.id,
											TopicName = x.topic_name
										}).OrderBy(z => z.Id).ToList();
		}

		/// <summary>
		/// Saves a post
		/// </summary>
		/// <param name="post"></param>
		public void SaveTopic(Topics topic)
		{
			if (topic.TopicName == null)
			{
				return;
			}

			var timeNow = DateTime.Now;
			var t = new topic();

			t.topic_name = topic.TopicName;
			t.created_on = timeNow;

			DbContext.topics.Add(t);
			DbContext.SaveChanges();
		}
	}
}
