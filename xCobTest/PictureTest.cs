using CobolParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using CobolParser.Records;

namespace xCobTest
{
    
    
    /// <summary>
    ///This is a test class for PictureTest and is intended
    ///to contain all PictureTest Unit Tests
    ///</summary>
	[TestClass()]
	public class PictureTest
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
		///A test for Picture Constructor
		///</summary>
		[TestMethod()]
		public void PictureConstructorTest()
		{
			StringReader reader = new StringReader("PIC S9(11)V99 COMP.\n");
			Terminalize terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			Picture target = new Picture(terms);
			Assert.AreEqual(14, target.Length);
			Assert.AreEqual(2, target.PicFormat.Decimals);

			reader = new StringReader("PIC 9(3)v9(5) COMP.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Picture(terms);
			Assert.AreEqual(8, target.Length);
			Assert.AreEqual(5, target.PicFormat.Decimals);

			reader = new StringReader("PIC -(11)--.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Picture(terms);
			//Assert.IsTrue(target.PicFormat.IsSigned);
			Assert.AreEqual(13, target.Length);
			Assert.AreEqual(0, target.PicFormat.Decimals);

			reader = new StringReader("PIC 9(9)BBBBBB.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Picture(terms);
			Assert.AreEqual(9, target.Length);
			Assert.AreEqual(0, target.PicFormat.Decimals);

			reader = new StringReader("PIC ZZ,ZZZ,ZZZ,ZZZ.99-.\n");
			terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			target = new Picture(terms);
			Assert.IsTrue(target.PicFormat.IsSigned);
			Assert.AreEqual(15, target.Length);
			Assert.AreEqual(2, target.PicFormat.Decimals);
		}

		[TestMethod()]
		public void EolRegressionTest()
		{
			StringReader reader = new StringReader("PIC X(10).\r\n     03 TNDMUSER-ID                 PIC S9(09)     COMP.\r\n");
			Terminalize terms = new Terminalize(null, reader);
			terms.BeginIteration();
			terms.Next();
			Picture target = new Picture(terms);
			Assert.AreEqual(10, target.Length);
			Assert.AreEqual(0, target.PicFormat.Decimals);
		}
	}
}
