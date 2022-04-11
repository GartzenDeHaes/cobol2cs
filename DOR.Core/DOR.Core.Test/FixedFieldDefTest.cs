using System;

#if INTEGRATION
using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;
using Assert = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using DOR.Core.IO;

namespace DOR.Core.Test
{
	/// <summary>
	///This is a test class for FixedFieldDefTest and is intended
	///to contain all FixedFieldDefTest Unit Tests
	///</summary>
	[TestClass]
	public class FixedFieldDefTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

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
		///A test for OrdinalPosition
		///</summary>
		[TestMethod]
		public void OrdinalPositionTest()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, ' ');
			Assert.AreEqual(1, target.OrdinalPosition);
		}

		/// <summary>
		///A test for Name
		///</summary>
		[TestMethod]
		public void NameTest()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, ' ');
			Assert.AreEqual("testField", target.Name);
		}

		/// <summary>
		///A test for Length
		///</summary>
		[TestMethod]
		public void LengthTest()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, ' ');
			Assert.AreEqual(10, target.Length);
		}

		/// <summary>
		///A test for DefaultData
		///</summary>
		[TestMethod]
		public void DefaultDataTest()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, 'x');
			Assert.AreEqual("xxxxxxxxxx", target.DefaultData);

			target = new FixedFieldDef("testField", 10, 5, 1, "xyzzy12345");
			Assert.AreEqual("xyzzy12345", target.DefaultData);
		}

		/// <summary>
		///A test for Parse (i.e. get value for the field from an input record)
		///</summary>
		[TestMethod]
		public void ParseTest()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, 'x');
			Assert.AreEqual("xxxxxxxxxx", target.DefaultData);
			var record = "12345this_Is_The_Record";
			string expected = record.Substring(5, 10);
			string actual = target.Parse(record);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for FixedFieldDef Constructor
		///</summary>
		[TestMethod]
		public void FixedFieldDefConstructorTestCharDefault()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, 'x');
			Assert.AreEqual("xxxxxxxxxx", target.DefaultData);
			Assert.AreEqual("testField", target.Name);
			Assert.AreEqual(10, target.Length);
			Assert.AreEqual(1, target.OrdinalPosition);
		}

		/// <summary>
		///A test for FixedFieldDef Constructor
		///</summary>
		[TestMethod]
		public void FixedFieldDefConstructorTestStringDefault()
		{
			var target = new FixedFieldDef("testField", 10, 5, 1, "xyzzy12345");
			Assert.AreEqual("xyzzy12345", target.DefaultData);
			Assert.AreEqual("testField", target.Name);
			Assert.AreEqual(10, target.Length);
			Assert.AreEqual(1, target.OrdinalPosition);
		}

		/// <summary>
		///A test for FixedFieldDef Constructor
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void FixedFieldDefConstructorThrowsIfDefaultWrongLength()
		{
			// one char short
			new FixedFieldDef("testField", 10, 5, 1, "123456789");
		}
	}
}