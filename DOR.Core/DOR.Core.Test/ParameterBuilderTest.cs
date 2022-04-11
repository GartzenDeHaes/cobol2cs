using System;
using System.Data;
using System.Data.SqlClient;
using DOR.Core.Data;
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

using DOR.Core;

namespace DOR.Core.Test
{
	/// <summary>
	///This is a test class for ParameterBuilderTest and is intended
	///to contain all ParameterBuilderTest Unit Tests
	///</summary>
	[TestClass]
	public class ParameterBuilderTest
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
		///A test for BuildParameter from Int
		///</summary>
		[TestMethod]
		public void BuildParameterTestInt()
		{
			var name = "@test";
			var value = 100;
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Int, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter from Decimal
		///</summary>
		[TestMethod]
		public void BuildParameterTestDecimal()
		{
			var name = "@test"; 
			var value = 100m; 
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Money, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter
		///</summary>
		[TestMethod]
		public void BuildParameterTestDateTime()
		{
			var name = "@test";
			var value = DateTime.Now;
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.DateTime, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter
		///</summary>
		[TestMethod]
		public void BuildParameterTestString()
		{
			var name = "@test";
			var value = "this is a test";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.VarChar, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			// null test
			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.VarChar, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter from Decimal?
		///</summary>
		[TestMethod]
		public void BuildParameterTestNullableDecimal()
		{
			var name = "@NullableDecimal"; 
			decimal? value = 100m; 
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Money, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Money, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter from bool
		///</summary>
		[TestMethod]
		public void BuildParameterTestBool()
		{
			var name = "@bool"; 
			SqlParameter actual = ParameterBuilder.BuildParameter(name, false);
			Assert.AreEqual(SqlDbType.Bit, actual.SqlDbType);
			Assert.AreEqual(0, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			actual = ParameterBuilder.BuildParameter(name, true);
			Assert.AreEqual(SqlDbType.Bit, actual.SqlDbType);
			Assert.AreEqual(1, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter for DateTime?
		///</summary>
		[TestMethod]
		public void BuildParameterTestNullableDateTime()
		{
			var name = "@NullableDateTime";
			DateTime? value = DateTime.Now;
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.DateTime, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.DateTime, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter from int?
		///</summary>
		[TestMethod]
		public void BuildParameterTestNullableInt()
		{
			var name = "@nullableInt";
			int? value = 100;
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Int, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Int, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildParameter from int?
		///</summary>
		[TestMethod]
		public void BuildParameterTestGuid()
		{
			var name = "@guid";
			Guid value = new Guid();
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.UniqueIdentifier, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for BuildNullParameter
		///</summary>
		[TestMethod]
		public void BuildNullParameterTest()
		{
			var name = "@NullParameter";
			SqlParameter actual = ParameterBuilder.BuildNullParameter(name, SqlDbType.NVarChar);
			Assert.AreEqual(SqlDbType.NVarChar, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for Build Parameter from  DOR Utility Date
		///</summary>
		[TestMethod]
		public void BuildDORDateParameterTest()
		{

			var value = new Date(DateTime.Now);
			var name = "@dorDate";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.DateTime, actual.SqlDbType);
			Assert.AreEqual(value.ToDateTime(), actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.DateTime, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for Build Parameter from  DOR Utility Money
		///</summary>
		[TestMethod]
		public void BuildDORMoneyParameterTest()
		{

			var value = new Money(100m);
			var name = "@dorMoney";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Money, actual.SqlDbType);
			Assert.AreEqual(value.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);

			value = null;
			actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.Money, actual.SqlDbType);
			Assert.AreEqual(DBNull.Value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for Build Parameter from  short
		///</summary>
		[TestMethod]
		public void BuildInt16ParameterTest()
		{

			var value = (short)100;
			var name = "@int16Test";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.SmallInt, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}

		/// <summary>
		///A test for Build Parameter from  long
		///</summary>
		[TestMethod]
		public void BuildInt64ParameterTest()
		{

			var value = 123456789123;
			var name = "@int64Test";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(SqlDbType.BigInt, actual.SqlDbType);
			Assert.AreEqual(value, actual.Value);
			Assert.AreEqual(name, actual.ParameterName);
		}
		/// <summary>
		///A test for Build Parameter from  long
		///</summary>
		[TestMethod]
		public void SetOutputParameterTest()
		{

			var value = 123456789123;
			var name = "@int64Test";
			SqlParameter actual = ParameterBuilder.BuildParameter(name, value);
			Assert.AreEqual(ParameterDirection.Input, actual.Direction);

			ParameterBuilder.SetOutputParameter(actual);
			Assert.AreEqual(ParameterDirection.Output, actual.Direction);
		}

	}
}