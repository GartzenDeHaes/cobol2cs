using CobolParser.Verbs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CobolParser;
using System.IO;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for PerformTest and is intended
    ///to contain all PerformTest Unit Tests
    ///</summary>
	[TestClass()]
	public class PerformTest
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
		///A test for Perform Constructor
		///</summary>
		[TestMethod()]
		public void PerformConstructorTest()
		{
			StringReader reader = new StringReader("PERFORM 2500-LOCATIONS UNTIL END-OF-GROSS-INCOME OR (WS-COL-S * 2) > WS-MAX-COLS-PER-GROUP.\n");
			Terminalize terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			Perform target = new Perform(terms);
		}
	}
}
