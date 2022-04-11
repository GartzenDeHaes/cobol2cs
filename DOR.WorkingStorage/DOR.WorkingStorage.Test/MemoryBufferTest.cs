using DOR.WorkingStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for MemoryBufferTest and is intended
    ///to contain all MemoryBufferTest Unit Tests
    ///</summary>
	[TestClass()]
	public class MemoryBufferTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for MemoryBuffer Constructor
		///</summary>
		[TestMethod()]
		public void MemoryBufferConstructorTest()
		{
			StringBuilder buf = new StringBuilder();
			MemoryBuffer target = new MemoryBuffer(4);
			Assert.AreEqual(4, target.Length);

			target.ClearTo('x');
			target.Get(buf, 0, 4);
			Assert.AreEqual("xxxx", buf.ToString());

			target.Set("1234", 0, 4);
			target.Get(buf, 0, 4);
			Assert.AreEqual("1234", buf.ToString());
			int i;
			target.Get(out i, 0, 4, false);
			Assert.AreEqual(1234, i);

			target.Set((decimal)12.34, 0, 4, false, 2);
			target.Get(buf, 0, 4);
			Assert.AreEqual("1234", buf.ToString());
			decimal d;
			target.Get(out d, 0, 4, false, 2);
			Assert.AreEqual((decimal)12.34, d);

			target = new MemoryBuffer(1);
			target.Set(12, 0, 1, false);
			target.Get(buf, 0, 1);
			Assert.AreEqual("1", buf.ToString());
		}
	}
}
