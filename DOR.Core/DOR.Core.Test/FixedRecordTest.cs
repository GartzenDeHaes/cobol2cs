using System.Collections.Generic;

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
	///This is a test class for FixedRecordTest and is intended
	///to contain all FixedRecordTest Unit Tests
	///</summary>
	[TestClass]
	public class FixedRecordTest
	{
		#region Constants
		private const string F0 = "F0        ";
		private const string F0_NAME = "field0";
		private const string F1 = "F1        ";
		private const string F1_NAME = "field1";
		private const string F2 = "F2        ";
		private const string F2_NAME = "field2";
		private const string F3 = "F3        ";
		private const string F3_NAME = "field3";
		private const string F4 = "F4        ";
		private const string F4_NAME = "field4";
		private const string F5 = "F5        ";
		private const string F5_NAME = "field5";
		private const string F6 = "F6        ";
		private const string F6_NAME = "field6";
		private const string F7 = "F7        ";
		private const string F7_NAME = "field7";
		private const string F8 = "F8        ";
		private const string F8_NAME = "field8";
		private const string F9 = "F9        ";
		private const string F9_NAME = "field9";

		private const string FIXED_RECORD_FOR_TEST_100BYTE =
			F0 + F1 + F2 + F3 + F4 + F5 + F6 + F7 + F8 + F9;

		private const int VALID_FIELD_COUNT = 10;
		#endregion Constants

		#region Properties
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#endregion Properties

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

		#region Tests
		/// <summary>
		///A test for Item using ordinals
		///</summary>
		[TestMethod]
		public void ItemTestGetValueByOrdinal()
		{
			FixedRecord target = BuildValidFixedRecordForTest();
			Assert.AreEqual(target[0].Value, F0);
			Assert.AreEqual(target[1].Value, F1);
			Assert.AreEqual(target[2].Value, F2);
			Assert.AreEqual(target[3].Value, F3);
			Assert.AreEqual(target[4].Value, F4);
			Assert.AreEqual(target[5].Value, F5);
			Assert.AreEqual(target[6].Value, F6);
			Assert.AreEqual(target[7].Value, F7);
			Assert.AreEqual(target[8].Value, F8);
			Assert.AreEqual(target[9].Value, F9);
		}

		/// <summary>
		///A test for Item using field names
		///</summary>
		[TestMethod]
		public void ItemTestGetValueByFieldName()
		{
			FixedRecord target = BuildValidFixedRecordForTest();
			Assert.AreEqual(target[F0_NAME].Value, F0);
			Assert.AreEqual(target[F1_NAME].Value, F1);
			Assert.AreEqual(target[F2_NAME].Value, F2);
			Assert.AreEqual(target[F3_NAME].Value, F3);
			Assert.AreEqual(target[F4_NAME].Value, F4);
			Assert.AreEqual(target[F5_NAME].Value, F5);
			Assert.AreEqual(target[F6_NAME].Value, F6);
			Assert.AreEqual(target[F7_NAME].Value, F7);
			Assert.AreEqual(target[F8_NAME].Value, F8);
			Assert.AreEqual(target[F9_NAME].Value, F9);
		}

		/// <summary>
		///A test for Count
		///  - make sure that it returns the number of fields in the rec
		///  - s/b 10
		///</summary>
		[TestMethod]
		public void CountTest()
		{
			FixedRecord target = BuildValidFixedRecordForTest();
			Assert.AreEqual(target.Count, VALID_FIELD_COUNT);
		}

		/// <summary>
		///A test for ToString
		///Record should match our input string plus a new-line marker 
		/// TODO: ask why a new line?
		///</summary>
		[TestMethod]
		public void ToStringTest()
		{
			FixedRecord target = BuildValidFixedRecordForTest();
			Assert.AreEqual(FIXED_RECORD_FOR_TEST_100BYTE + "\r\n", target.ToString());
		}

		/// <summary>
		///A test for Reset - in our case, should reset to spaces (I think...)
		///</summary>
		[TestMethod]
		public void ResetTest()
		{
			FixedRecord target = BuildValidFixedRecordForTest();
			target.Reset();
			Assert.AreEqual("".PadRight(100) + "\r\n", target.ToString());
		}

		/// <summary>
		///A test for FixedRecord Constructor
		///</summary>
		[TestMethod]
		public void FixedRecordConstructorTest()
		{
			var def = new FixedRecordDef(100);
			var fields = new FixedFieldData[]
			             	{
			             		new FixedFieldData(
			             			new FixedFieldDef("WholeRecord", 100, 0, 0, ' '),
			             			"".PadRight(100))
			             	};
			var target = new FixedRecord(def, fields);

			Assert.IsTrue(target.GetType() == typeof (FixedRecord));
		}
		#endregion Tests

		#region Private Helper Methods
		/// <summary>
		/// Build a valid record definition for testing 
		/// </summary>
		/// <returns></returns>
		private FixedRecordDef BuildValidFixedRecordDefForTest()
		{
			var def = new FixedRecordDef(100);
			def.AddField(F0_NAME, F0.Length, ' ');
			def.AddField(F1_NAME, F1.Length, ' ');
			def.AddField(F2_NAME, F2.Length, ' ');
			def.AddField(F3_NAME, F3.Length, ' ');
			def.AddField(F4_NAME, F4.Length, ' ');
			def.AddField(F5_NAME, F5.Length, ' ');
			def.AddField(F6_NAME, F6.Length, ' ');
			def.AddField(F7_NAME, F7.Length, ' ');
			def.AddField(F8_NAME, F8.Length, ' ');
			def.AddField(F9_NAME, F9.Length, ' ');
			return def;
		}

		private FixedFieldData[] BuildValidFixedFieldDataArrayForTest(string record)
		{
			var fieldDataList = new List<FixedFieldData>(10)
			                    	{
			                    		new FixedFieldData(new FixedFieldDef(F0_NAME, 10, 0, 0, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F1_NAME, 10, 10, 1, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F2_NAME, 10, 20, 2, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F3_NAME, 10, 30, 3, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F4_NAME, 10, 40, 4, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F5_NAME, 10, 50, 5, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F6_NAME, 10, 60, 6, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F7_NAME, 10, 70, 7, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F8_NAME, 10, 80, 7, ' '), record),
			                    		new FixedFieldData(new FixedFieldDef(F9_NAME, 10, 90, 9, ' '), record)
			                    	};

			return fieldDataList.ToArray();
		}

		private FixedRecord BuildValidFixedRecordForTest()
		{
			FixedRecordDef def = BuildValidFixedRecordDefForTest();
			FixedFieldData[] fields = BuildValidFixedFieldDataArrayForTest(FIXED_RECORD_FOR_TEST_100BYTE);
			return new FixedRecord(def, fields);
		}
		#endregion Private Helper Methods
	}
}