using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraqNote.Service;

namespace TraqNote.UnitTest
{
	[TestClass]
	public class PostServicesTests : BaseUnitTest
	{
		[TestInitialize]
		public void MyTestInitialize()
		{
			// Not implemented yet
		}

		[TestCleanup]
		public void MyTestCleanup()
		{
			// Not implemented yet
		}
		[TestMethod]
		public void GetAllPosts_Test()
		{
			using (var svc = new PostServices())
			{
				var result = svc.GetAllPosts(string.Empty);

				Assert.IsNotNull(result, @"Was expecting a non-null result set");
			}
		}
	}
}
