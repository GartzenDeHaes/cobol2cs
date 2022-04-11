using CobolParser.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CobolParser;
using System.IO;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for ExprTest and is intended
    ///to contain all ExprTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ExprTest
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
		///A test for Expr Constructor
		///</summary>
		[TestMethod()]
		public void ExprConstructorTest()
		{
			StringReader reader = new StringReader("IF HND-SALMON-CRDT-APPR-CODE OF PRMTDESC-REC = \"X\", OR \"N\", OR \"Y\" CONTINUE.\n");
			Terminalize terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			terms.Match("IF");
			IExpr target = IExpr.Parse(terms);
			terms.Match("CONTINUE");
		}
	}
}
