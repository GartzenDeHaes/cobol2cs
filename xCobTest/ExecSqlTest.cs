using CobolParser.Verbs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CobolParser;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for ExecSqlTest and is intended
    ///to contain all ExecSqlTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ExecSqlTest
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

		readonly string _execsql =
			"     EXEC SQL\r\n" +
			"         SELECT  HADJRATE_TAB_RATE\r\n" +
			"\r\n" +
			"         INTO   :SQL-HADJRATE-TAB-RATE\r\n" +
			"\r\n" +
			"         FROM   =HARVLOC L, =HADJRATE  R\r\n" +
			"\r\n" +
			"         WHERE  \r\n" +
			"             :TRTRAN-YEAR OF RTNSMRY-REC, :TRTRAN-QTR OF RTNSMRY-REC\r\n" +
			"                                         BETWEEN\r\n" +
			"                    (HADJRATE_BEGIN_YEAR, HADJRATE_BEGIN_QTR) AND\r\n" +
			"                    (HADJRATE_END_YEAR, HADJRATE_END_QTR)\r\n" +
			"\r\n" +
			"         FOR BROWSE ACCESS\r\n" +
			"     END-EXEC.\r\n";
		
		[TestMethod()]
		public void ExecSqlConstructorTest()
		{
			ImportManager.BaseDirectory = "C:\\TEMP\\COBOL\\";

			Terminalize terms = new Terminalize(new GuardianPath("$D08.SOURCE.B005FE1Y"), _execsql);
			terms.BeginIteration();
			terms.Next();
			ExecSql target = new ExecSql(terms);

			Assert.AreEqual(typeof(CobolParser.SQL.Statements.Select), target.Sql.GetType());
		}
	}
}
