using CobolParser.SQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DOR.Core.Collections;
using CobolParser;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for SqlExprTest and is intended
    ///to contain all SqlExprTest Unit Tests
    ///</summary>
	[TestClass()]
	public class SqlExprTest
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

		private Vector<StringNode> BuildNodeList(params string[] toks)
		{
			Vector<StringNode> terms = new Vector<StringNode>();

			for (int x = 0; x < toks.Length; x++)
			{
				StringNode node = new StringNode(null, 0, 0, toks[x]);
				if (x > 0)
				{
					terms[x-1].Next = node;
				}
				terms.Add(node);
			}
			return terms;
		}

		/// <summary>
		///A test for SqlExpr Constructor
		///</summary>
		[TestMethod()]
		public void SqlExprConstructorTest()
		{
			Vector<StringNode> terms = BuildNodeList
			(
				"(",
				"VALIDRDS_REGION_NUM",
				",",
				"VALIDRDS_DIST_NUM",
				")",
				"=",
				"(",
				":VALIDRDS-REGION-NUM",
				",",
				":VALIDRDS-DIST-NUM",
				")"
			);

			SqlLex lex = new SqlLex(terms);
			lex.Next();
			Assert.IsFalse(lex.IsEOF);
			SqlExpr target = new SqlExpr(lex, false, true, false);
		}
	}
}
