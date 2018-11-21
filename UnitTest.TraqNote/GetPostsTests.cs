using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraqNote.Service;
using UnitTest.TraqNote.Constants;

namespace UnitTest.TraqNote
{
	[TestClass]
	public class GetPostsTests
	{
		[TestMethod, TestCategory(TestCategoryNames.GET_ALL_POSTS_TESTS)]
		public void GetAllPosts_Test()
		{
			using (var svc = new NoteServices())
			{
				var result = svc.GetAllPosts();
				Assert.IsNotNull(result, @"Expected a valid result from GetAllPosts");
			}
		}
	}
}
