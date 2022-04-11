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
using System.Collections.Generic;
using System;
using DOR.Core.IO;

namespace DOR.Core.Test
{

	/// <summary>
	///This is a test class for FixedRecordDefTest and is intended
	///to contain all FixedRecordDefTest Unit Tests
	///</summary>
	[TestClass]
	public class FixedRecordDefTest
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

		//private const int VALID_FIELD_COUNT = 10;
		#endregion Constants

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
		///A test for ReleaseCached (puts a record in the cache) and
		/// CreateCached.
		/// These tests are combined because they are a paired process:
		///		ReleaseCached puts records in cache
		///		CreateCached pulls records out of cache
		/// 
		/// This cache is a stack - last in, first out
		///</summary>
		[TestMethod]
		public void ReleaseCachedAndCreateCacheTests()
		{
			var target = BuildValidFixedRecordDefForTest();

			#region ReleaseCached tests
			FixedRecord rcd = new FixedRecord(target, BuildValidFixedFieldDataArrayForTestWithNonBlankDefaults(FIXED_RECORD_FOR_TEST_100BYTE));
			Assert.AreEqual(F0, rcd[0].Value);
			Assert.AreEqual(F1, rcd[1].Value);
			Assert.AreEqual(F2, rcd[2].Value);
			Assert.AreEqual(F3, rcd[3].Value);
			Assert.AreEqual(F4, rcd[4].Value);
			Assert.AreEqual(F5, rcd[5].Value);
			Assert.AreEqual(F6, rcd[6].Value);
			Assert.AreEqual(F7, rcd[7].Value);
			Assert.AreEqual(F8, rcd[8].Value);
			Assert.AreEqual(F9, rcd[9].Value);

			// put the record in the cache (wierdly, this does a 'reset' 
			// on the record before caching - why????
			target.ReleaseCached(rcd);
			Assert.AreEqual("0000000000", rcd[0].Value);
			Assert.AreEqual("1111111111", rcd[1].Value);
			Assert.AreEqual("2222222222", rcd[2].Value);
			Assert.AreEqual("3333333333", rcd[3].Value);
			Assert.AreEqual("4444444444", rcd[4].Value);
			Assert.AreEqual("5555555555", rcd[5].Value);
			Assert.AreEqual("6666666666", rcd[6].Value);
			Assert.AreEqual("7777777777", rcd[7].Value);
			Assert.AreEqual("8888888888", rcd[8].Value);
			Assert.AreEqual("9999999999", rcd[9].Value);


			// put another rec in cache (with reset...)
			FixedRecord rcd2 = new FixedRecord(target, BuildValidFixedFieldDataArrayForTest(FIXED_RECORD_FOR_TEST_100BYTE));

			target.ReleaseCached(rcd2);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[0].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[1].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[2].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[3].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[4].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[5].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[6].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[7].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[8].Value);
			Assert.AreEqual(string.Empty.PadRight(10), rcd2[9].Value);

			#endregion ReleaseCached tests

			#region CreateCached tests

			// now that we have records in cache, we can pull them out (last in, first out)

			FixedRecord recFromCache = target.CreateCached();
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[0].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[1].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[2].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[3].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[4].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[5].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[6].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[7].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[8].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache[9].Value);

			FixedRecord recFromCache2 = target.CreateCached();
			Assert.AreEqual("0000000000", recFromCache2[0].Value);
			Assert.AreEqual("1111111111", recFromCache2[1].Value);
			Assert.AreEqual("2222222222", recFromCache2[2].Value);
			Assert.AreEqual("3333333333", recFromCache2[3].Value);
			Assert.AreEqual("4444444444", recFromCache2[4].Value);
			Assert.AreEqual("5555555555", recFromCache2[5].Value);
			Assert.AreEqual("6666666666", recFromCache2[6].Value);
			Assert.AreEqual("7777777777", recFromCache2[7].Value);
			Assert.AreEqual("8888888888", recFromCache2[8].Value);
			Assert.AreEqual("9999999999", recFromCache2[9].Value);

			// at this point, the cache s/b empty.  Pulling from cache 'should'
			// return a default record
			FixedRecord recFromCache3 = target.CreateCached();
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[0].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[1].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[2].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[3].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[4].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[5].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[6].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[7].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[8].Value);
			Assert.AreEqual(string.Empty.PadRight(10), recFromCache3[9].Value);

			#endregion CreateCached tests

		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod]
		public void ParseTest()
		{
			var target = BuildValidFixedRecordDefForTest();
			FixedRecord expected = new FixedRecord(target, BuildValidFixedFieldDataArrayForTest(FIXED_RECORD_FOR_TEST_100BYTE));
			FixedRecord actual = target.Parse(FIXED_RECORD_FOR_TEST_100BYTE);
			Assert.AreEqual(expected[0].Value, actual[0].Value);
			Assert.AreEqual(expected[1].Value, actual[1].Value);
			Assert.AreEqual(expected[2].Value, actual[2].Value);
			Assert.AreEqual(expected[3].Value, actual[3].Value);
			Assert.AreEqual(expected[4].Value, actual[4].Value);
			Assert.AreEqual(expected[5].Value, actual[5].Value);
			Assert.AreEqual(expected[6].Value, actual[6].Value);
			Assert.AreEqual(expected[7].Value, actual[7].Value);
			Assert.AreEqual(expected[8].Value, actual[8].Value);
			Assert.AreEqual(expected[9].Value, actual[9].Value);
		}
		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseTestThrowsIfInputIsWrongLength()
		{
			var target = BuildValidFixedRecordDefForTest();
			// make the input 1 byte too long
			target.Parse(FIXED_RECORD_FOR_TEST_100BYTE + " ");
		}

		/// <summary>
		///A test for GetFieldOrdinal
		///</summary>
		[TestMethod]
		public void GetFieldOrdinalTest()
		{
			var target = BuildValidFixedRecordDefForTest();
			var name = F6_NAME;
			var expected = 6; 
			int actual = target.GetFieldOrdinal(name);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Create
		///</summary>
		[TestMethod]
		public void CreateTest()
		{
			FixedRecordDef target = BuildValidFixedRecordDefForTestWithNonBlankDefault();
			FixedRecord actual = target.Create();
			// assure that the record that was created, has the default values 
			// from the def (in target).
			Assert.AreEqual("0000000000", actual[0].Value);
			Assert.AreEqual("1111111111", actual[1].Value);
			Assert.AreEqual("2222222222", actual[2].Value);
			Assert.AreEqual("3333333333", actual[3].Value);
			Assert.AreEqual("4444444444", actual[4].Value);
			Assert.AreEqual("5555555555", actual[5].Value);
			Assert.AreEqual("6666666666", actual[6].Value);
			Assert.AreEqual("7777777777", actual[7].Value);
			Assert.AreEqual("8888888888", actual[8].Value);
			Assert.AreEqual("9999999999", actual[9].Value);
		}

		/// <summary>
		///A test for AddField
		///</summary>
		[TestMethod]
		public void AddFieldTestWithStringAndCharDefaults()
		{
			var target = new FixedRecordDef(25);

			// there are three (3) overloads of AddField
			target.AddField("myTestField", 10, "myDefault ");
			target.AddField("myTestField2", 15, 'x');
			
			FixedRecord rec = new FixedRecord(target, 
					new FixedFieldData[]
						{
							new FixedFieldData(new FixedFieldDef("x1", 10, 0, 0, '1')),
							new FixedFieldData(new FixedFieldDef("x2", 15, 10, 1, '2')) 
						});

			Assert.AreEqual("1111111111", rec["myTestField"].Value);
			Assert.AreEqual("222222222222222", rec["myTestField2"].Value);
		}


		/// <summary>
		///A test for FixedRecordDef Constructor
		///</summary>
		[TestMethod]
		public void FixedRecordDefConstructorTest()
		{
			var linelen = 10; 
			var target = new FixedRecordDef(linelen);
			Assert.AreEqual(typeof(FixedRecordDef), target.GetType());
		}

		#region Private Helper Methods		/// <summary>
		/// Build a valid record definition for testing 
		/// </summary>
		/// <returns></returns>
		private static FixedRecordDef BuildValidFixedRecordDefForTest()
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
		/// Build a valid record definition for testing 
		/// </summary>
		/// <returns></returns>
		private static FixedRecordDef BuildValidFixedRecordDefForTestWithNonBlankDefault()
		{
			var def = new FixedRecordDef(100);
			def.AddField(F0_NAME, F0.Length, '0');
			def.AddField(F1_NAME, F1.Length, '1');
			def.AddField(F2_NAME, F2.Length, '2');
			def.AddField(F3_NAME, F3.Length, '3');
			def.AddField(F4_NAME, F4.Length, '4');
			def.AddField(F5_NAME, F5.Length, '5');
			def.AddField(F6_NAME, F6.Length, '6');
			def.AddField(F7_NAME, F7.Length, '7');
			def.AddField(F8_NAME, F8.Length, '8');
			def.AddField(F9_NAME, F9.Length, '9');
			return def;
		}

		private static FixedFieldData[] BuildValidFixedFieldDataArrayForTest(string record)
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

		private static FixedFieldData[] BuildValidFixedFieldDataArrayForTestWithNonBlankDefaults(string record)
		{
			var fieldDataList = new List<FixedFieldData>(10)
			                    	{
			                    		new FixedFieldData(new FixedFieldDef(F0_NAME, 10, 0, 0, '0'), record),
			                    		new FixedFieldData(new FixedFieldDef(F1_NAME, 10, 10, 1, '1'), record),
			                    		new FixedFieldData(new FixedFieldDef(F2_NAME, 10, 20, 2, '2'), record),
			                    		new FixedFieldData(new FixedFieldDef(F3_NAME, 10, 30, 3, '3'), record),
			                    		new FixedFieldData(new FixedFieldDef(F4_NAME, 10, 40, 4, '4'), record),
			                    		new FixedFieldData(new FixedFieldDef(F5_NAME, 10, 50, 5, '5'), record),
			                    		new FixedFieldData(new FixedFieldDef(F6_NAME, 10, 60, 6, '6'), record),
			                    		new FixedFieldData(new FixedFieldDef(F7_NAME, 10, 70, 7, '7'), record),
			                    		new FixedFieldData(new FixedFieldDef(F8_NAME, 10, 80, 7, '8'), record),
			                    		new FixedFieldData(new FixedFieldDef(F9_NAME, 10, 90, 9, '9'), record)
			                    	};

			return fieldDataList.ToArray();
		}
		#endregion Private Helper Methods
	}
}