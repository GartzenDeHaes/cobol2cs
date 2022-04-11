using CobolParser.Verbs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CobolParser;
using System.IO;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for MoveTest and is intended
    ///to contain all MoveTest Unit Tests
    ///</summary>
	[TestClass()]
	public class MoveTest
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
		///A test for Move Constructor
		///</summary>
		[TestMethod()]
		public void MoveConstructorTest()
		{
			StringReader reader = new StringReader("MOVE ALBL-SEQ-NUM OF CL-SYSTEM-MENU-REPLY (1) TO ALBL-SEQ-NUM OF CL-SYSTEM-MENU-MSG.\n");
			Terminalize terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			Move target = new Move(terms);

			reader = new StringReader("MOVE FUNCTION UPPER-CASE (RTNSMRY-TRANS-CODE OF RTNSMRY-REC) TO RTNSMRY-TRANS-CODE OF RTNSMRY-REC.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Move(terms);

			reader = new StringReader("MOVE WS-STUMPAGE-VALUE(WS-RPTG-YEAR-SUB, 5) TO W10-TOT-STUMPAGE-VALUE.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Move(terms);

			reader = new StringReader("MOVE \"        TOTAL  \" TO WS-RPT-DEDUCT-LIT-SEGMENT (1 WS-LIT-COL-S).\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Move(terms);

			reader = new StringReader("MOVE \"TOTAL \"\"X\"\" IN\" TO WS-RPT.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Move(terms);
		}
	}
}
