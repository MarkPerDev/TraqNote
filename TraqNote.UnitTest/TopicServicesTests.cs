using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraqNote.Data;
using TraqNote.Service;
using System.Linq;
using TraqNote.Data.Views;

namespace TraqNote.UnitTest
{
	[TestClass]
	public class TopicServicesTests : BaseUnitTest
	{
		private string _topicTestName;

		[TestInitialize]
		public void MyTestInitialize()
		{
			// Not implemented yet
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			CleanUpTestData();
		}

		[TestMethod]
		public void SaveATopic()
		{
			_topicTestName = Guid.NewGuid().ToString().Substring(0, 20);

			var newTopic = new Topics()
			{
				Created_On = DateTime.Now,
				TopicName = _topicTestName
			};

			using (var svc = new TopicServices())
			{
				svc.SaveTopic(newTopic);
			}

			using (var db = new TraqnoteEntities())
			{
				Assert.AreEqual(_topicTestName,
					db.topics.FirstOrDefault(x => x.topic_name == _topicTestName).topic_name);
			}
		}


		#region Private Methods
		public override void CleanUpTestData()
		{
			using (var context = new TraqnoteEntities())
			{
				context.Database.ExecuteSqlCommand(@"delete from topic where topic_name = {0}", _topicTestName);
			}
		}

		#endregion
	}
}
