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

using DOR.Core;

namespace DOR.Core.Test
{
	/// <summary>
	///This is a test class for MoneyTest and is intended
	///to contain all MoneyTest Unit Tests
	///</summary>
	[TestClass]
	public class MoneyTest
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
		///A test for the default value (s/b zero)
		///</summary>
		[TestMethod]
		public void DefaultValueTest()
		{
			var target = new Money();
			Assert.AreEqual(0, target.Value, "Default Money value is not zero.");
		}

		/// <summary>
		/// Test to verify that the money value is the same as the decimal constructor value
		/// </summary>
		[TestMethod]
		public void DecimalValueTest()
		{
			const decimal testVal = 100.01m;
			var target = new Money(testVal);
			Assert.AreEqual(testVal, target.Value);
		}

		/// <summary>
		/// Test to verify that the Money value is the same as the Money constructor value
		/// </summary>
		[TestMethod]
		public void MoneyValueTest()
		{
			const decimal testValDecimal = 200.02m;
			var moneySource = new Money(testValDecimal);
			var target = new Money(moneySource);
			Assert.AreEqual(testValDecimal, target.Value);
		}

		/// <summary>
		/// Test to verify that the Money value is the same as the string constructor value
		/// </summary>
		[TestMethod]
		public void StringValueTest()
		{
			const decimal testValDecimal = 200.02m;
			string testValString = testValDecimal.ToString();
			var moneySource = Money.Parse(testValString);
			var target = new Money(moneySource);
			Assert.AreEqual(testValDecimal, target.Value);
		}

		/// <summary>
		///A test for ToString with the returnZeros bool set to false or true
		///</summary>
		[TestMethod]
		public void ToStringTest2()
		{
			var target = new Money();
			string actual = target.ToString(false);
			Assert.AreEqual(string.Empty, actual);

			actual = target.ToString(true);
			Assert.AreEqual("0.00", actual);
		}

		/// <summary>
		///A test for ToString.  S/B the same as the constructor decimal toString().
		///</summary>
		[TestMethod]
		public void ToStringTest1()
		{
			const decimal testVal = 200.02m;
			var target = new Money(testVal);
			string expected = testVal.ToString();
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString formatted (removeZeros, numberOfPlaces)
		///</summary>
		[TestMethod]
		public void ToStringTest()
		{
			var target = new Money(200.00m);
			Assert.AreEqual("200", target.ToString(false, 0),
			                "tostring should trim off trailing zeroes and decimal");

			Assert.AreEqual("200.00", target.ToString(false, 2),
			                "tostring should show two trailing zeros");

			Assert.AreEqual("200.00", target.ToString(true, 2),
			                "tostring should show two trailing zeros");

			// zero tests - returning zero 
			target = new Money();
			Assert.AreEqual("0", target.ToString(true, 0),
			                "tostring should trim off trailing zeroes and decimal");

			Assert.AreEqual("0.00", target.ToString(true, 2),
			                "tostring should show two trailing zeros");

			Assert.AreEqual("0.0000", target.ToString(true, 4),
			                "tostring should show four trailing zeros");
		}

		/// <summary>
		///A test for Round
		///</summary>
		[TestMethod]
		public void RoundTest()
		{
			var target = new Money(200.1234m);
			Assert.AreEqual(200.12m, target.Value);
		}

		/// <summary>
		///A test for op_UnaryNegation
		///</summary>
		[TestMethod]
		public void op_UnaryNegationTest()
		{
			const decimal value = 100.01m;
			var m = new Money(value);
			var expected = new Money(value*-1.00m);
			Money actual = -(m);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Subtraction subtracting a Money obj
		///</summary>
		[TestMethod]
		public void op_SubtractionTestSubtractMoney()
		{
			const decimal initValue = 100.00m;
			const decimal valueToSubtract = 47.30m;
			var m1 = new Money(initValue);
			var m2 = new Money(valueToSubtract);
			var expected = new Money(initValue - valueToSubtract);
			Money actual = (m1 - m2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Subtraction subtracting a Decimal obj
		///</summary>
		[TestMethod]
		public void op_SubtractionTestSubtractDecimal()
		{
			const decimal INIT_VALUE = 100.00m;
			const decimal VALUE_TO_SUBTRACT = 47.30m;
			var m1 = new Money(INIT_VALUE);
			var expected = new Money(INIT_VALUE - VALUE_TO_SUBTRACT);
			Money actual = m1 - VALUE_TO_SUBTRACT;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Multiply by a Decimal num
		///</summary>
		[TestMethod]
		public void op_MultiplyByDecimalTest()
		{
			const decimal INITIAL_VALUE = 200.01m;
			const decimal MULTIPLIER = 11;

			var m = new Money(INITIAL_VALUE);
			var expected = new Money(INITIAL_VALUE*MULTIPLIER);
			Money actual = (m*MULTIPLIER);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Multiply by a Double num
		///</summary>
		[TestMethod]
		public void op_MultiplyByDoubleTest()
		{
			const decimal INITIAL_VALUE = 200.01m;
			const double MULTIPLIER = 11;
			var m = new Money(INITIAL_VALUE);
			var expected = new Money(INITIAL_VALUE*(decimal) MULTIPLIER);
			Money actual = (m*MULTIPLIER);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Multiply by a Money obj
		///</summary>
		[TestMethod]
		public void op_MultiplyByMoneyTest()
		{
			const decimal INITIAL_VALUE = 200.01m;
			const decimal MULTIPLIER = 11;
			var m1 = new Money(INITIAL_VALUE);
			var m2 = new Money(MULTIPLIER);
			var expected = new Money(INITIAL_VALUE*MULTIPLIER);
			Money actual = (m1*m2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_Multiply by an int num
		///</summary>
		[TestMethod]
		public void op_MultiplyByInt()
		{
			const decimal INITIAL_VALUE = 200.01m;
			const int MULTIPLIER = 11;
			var m = new Money(INITIAL_VALUE);
			var expected = new Money(INITIAL_VALUE*MULTIPLIER);
			Money actual = (m*MULTIPLIER);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for op_LessThanOrEqual with a Money obj
		///</summary>
		[TestMethod]
		public void op_LessThanOrEqualMoneyTest()
		{
			var m1 = new Money(100m);
			var m2 = new Money(200m);
			Assert.AreEqual(true, (m1 <= m2));
			Assert.AreEqual(false, (m2 <= m1));
			var m3 = new Money(100m);
			Assert.AreEqual(true, (m1 <= m3));
		}

		/// <summary>
		///A test for op_LessThanOrEqual  with an int
		///</summary>
		[TestMethod]
		public void op_LessThanOrEqualIntTest()
		{
			var m1 = new Money(100m);
			int m2 = 200;
			Assert.AreEqual(true, (m1 <= m2));
			m2 = 100;
			Assert.AreEqual(true, (m1 <= m2));
			m2 = 55;
			Assert.AreEqual(false, (m1 <= m2));
		}

		/// <summary>
		///A test for op_LessThanOrEqual against Decimal
		///</summary>
		[TestMethod]
		public void op_LessThanOrEqualDecimalTest()
		{
			var m1 = new Money(100m);
			decimal m2 = 200m;
			Assert.AreEqual(true, (m1 <= m2));
			m2 = 100m;
			Assert.AreEqual(true, (m1 <= m2));
			m2 = 55m;
			Assert.AreEqual(false, (m1 <= m2));
		}

		/// <summary>
		///A test for op_LessThan with a Money obj
		///</summary>
		[TestMethod]
		public void op_LessThanMoneyTest1()
		{
			var m1 = new Money(100m);
			var m2 = new Money(200m);
			Assert.AreEqual(true, (m1 < m2), "100 should be less than 200");
			Assert.AreEqual(false, (m2 < m1), "200 should not be less than 100");

			var m3 = new Money(100m);
			Assert.AreEqual(false, (m1 < m3), "100 should not be less than 100");
		}

		/// <summary>
		///A test for op_LessThan with a Decimal
		///</summary>
		[TestMethod]
		public void op_LessThanDecimalTest()
		{
			var m1 = new Money(100m);
			decimal m2 = 200m;
			Assert.AreEqual(true, (m1 < m2));
			m2 = 100m;
			Assert.AreEqual(false, (m1 < m2));
			m2 = 55m;
			Assert.AreEqual(false, (m1 < m2));
		}

		/// <summary>
		///A test for op_Inequality with a Money obj
		///</summary>
		[TestMethod]
		public void op_InequalityAgainstMoneyTest()
		{
			var m1 = new Money(100m);
			var m2 = new Money(200m);
			Assert.AreEqual(true, (m1 != m2));

			var m3 = new Money(100m);
			Assert.AreEqual(false, (m1 != m3));
		}

		/// <summary>
		///A test for op_Inequality  with an int
		///</summary>
		[TestMethod]
		public void op_InequalityAgainstIntTest()
		{
			var m1 = new Money(100m);
			const int m2 = 200;
			Assert.AreEqual(true, (m1 != m2));

			const int m3 = 100;
			Assert.AreEqual(false, (m1 != m3));
		}

		/// <summary>
		///A test for op_GreaterThanOrEqual against Money
		///</summary>
		[TestMethod]
		public void op_GreaterThanOrEqualMoneyTest()
		{
			var m1 = new Money(100m);
			var m2 = new Money(200m);
			Assert.AreEqual(false, (m1 >= m2));
			Assert.AreEqual(true, (m2 >= m1));
			var m3 = new Money(100m);
			Assert.AreEqual(true, (m1 >= m3));
		}

		/// <summary>
		///A test for op_GreaterThanOrEqual Integer
		///</summary>
		[TestMethod]
		public void op_GreaterThanOrEqualIntegerTest()
		{
			var m1 = new Money(100m);
			int m2 = 200;
			Assert.AreEqual(false, (m1 >= m2));
			m2 = 99;
			Assert.AreEqual(true, (m1 >= m2));
			m2 = 100;
			Assert.AreEqual(true, (m1 >= m2));
		}

		/// <summary>
		///A test for op_GreaterThanOrEqual decimal
		///</summary>
		[TestMethod]
		public void op_GreaterThanOrEqualDecimalTest()
		{
			var m1 = new Money(100m);
			decimal m2 = 200;
			Assert.AreEqual(false, (m1 >= m2));
			m2 = 99;
			Assert.AreEqual(true, (m1 >= m2));
			m2 = 100;
			Assert.AreEqual(true, (m1 >= m2));
		}

		/// <summary>
		///A test for op_GreaterThan Money
		///</summary>
		[TestMethod]
		public void op_GreaterThanMoney()
		{
			var m1 = new Money(100m);
			var m2 = new Money(200m);
			Assert.AreEqual(false, (m1 > m2));
			Assert.AreEqual(true, (m2 > m1));
			var m3 = new Money(100m);
			Assert.AreEqual(false, (m1 > m3));
		}

		/// <summary>
		///A test for op_GreaterThan Decimal
		///</summary>
		[TestMethod]
		public void op_GreaterThanDecimalTest()
		{
			var m1 = new Money(100m);
			decimal m2 = 200;
			Assert.AreEqual(false, (m1 > m2));
			m2 = 99;
			Assert.AreEqual(true, (m1 > m2));
			m2 = 100;
			Assert.AreEqual(false, (m1 > m2));
		}

		/// <summary>
		///A test for op_Explicit cast to double
		///</summary>
		[TestMethod]
		public void op_ExplicitCastToDoubleTest()
		{
			const double EXPECTED = 220.25F;
			var m = new Money((decimal)EXPECTED);
			Assert.AreEqual((double)m, EXPECTED);
		}

		/// <summary>
		///A test for op_Explicit cast to decimal
		///</summary>
		[TestMethod]
		public void op_ExplicitCastToDecimalTest()
		{
			const decimal EXPECTED = 220.25m;
			var m = new Money(EXPECTED);
			Assert.AreEqual((decimal)m, EXPECTED);
		}

		/// <summary>
		///A test for op_Explicit cast to Int
		///</summary>
		[TestMethod]
		public void op_ExplicitCastToIntegerTest()
		{
			decimal expected = 220;
			var m = new Money(expected);
			Assert.AreEqual((int) m, expected);

			var m2 = new Money(220.49m);
			Assert.AreEqual((int) m2, expected);

			var m3 = new Money(219.50m);
			Assert.AreEqual((int) m3, expected);

			var m4 = new Money(220.51m);
			Assert.AreNotEqual((int) m4, expected);

			var m5 = new Money(220.50m);
			Assert.AreEqual((int) m5, expected);
		}

		/// <summary>
		///A test for op_Equality with another Money
		///</summary>
		[TestMethod]
		public void op_EqualityWithMoneyTest()
		{
			var m1 = new Money(100m);
			var m2 = new Money(100m);
			var m3 = new Money(200m);
			Assert.AreEqual(true, (m1 == m2));
			Assert.AreEqual(false, (m1 == m3));
		}

		/// <summary>
		///A test for op_Equality with an int
		///</summary>
		[TestMethod]
		public void op_EqualityWithIntTest()
		{
			var m1 = new Money(100m);
			const int M2 = 100;
			const int M3 = 200;
			Assert.AreEqual(true, (m1 == M2));
			Assert.AreEqual(false, (m1 == M3));
		}

		/// <summary>
		///A test for op_Equality with an int
		///</summary>
		[TestMethod]
		public void op_EqualityWithDecimalTest()
		{
			var m1 = new Money(100m);
			const decimal M2 = 100m;
			const decimal M3 = 200m;
			Assert.AreEqual(true, (m1 == M2));
			Assert.AreEqual(false, (m1 == M3));
		}

		/// <summary>
		///A test for op_Equality with a decimal
		///</summary>
		[TestMethod]
		public void op_InEqualityWithDecimalTest()
		{
			var m1 = new Money(100m);
			const decimal M2 = 100m;
			const decimal M3 = 200m;
			Assert.AreEqual(false, (m1 != M2));
			Assert.AreEqual(true, (m1 != M3));
		}

		/// <summary>
		///A test for op_Addition with Decimal
		///</summary>
		[TestMethod]
		public void op_AdditionWithDecimalTest()
		{
			const decimal INITIAL_VALUE = 100m;
			var m1 = new Money(INITIAL_VALUE);
			const decimal ADDED_DECIMAL_VALUE = 155m;
			Money actual = (m1 + ADDED_DECIMAL_VALUE);
			Assert.AreEqual(INITIAL_VALUE + ADDED_DECIMAL_VALUE, actual.Value);
		}

		/// <summary>
		///A test for op_Addition with a Money obj
		///</summary>
		[TestMethod]
		public void op_AdditionWithMoneyTest()
		{
			const decimal INITIAL_VALUE = 100m;
			const decimal ADDED_DECIMAL_VALUE = 155m;
			var m1 = new Money(INITIAL_VALUE);
			var m2 = new Money(ADDED_DECIMAL_VALUE);
			Money actual = (m1 + m2);
			Assert.AreEqual(INITIAL_VALUE + ADDED_DECIMAL_VALUE, actual.Value);
		}

		/// <summary>
		///A test for IsMoney
		///</summary>
		[TestMethod]
		public void IsMoneyTest()
		{
			Assert.IsFalse(Money.IsMoney(string.Empty));
			Assert.IsTrue(Money.IsMoney("0"));
			Assert.IsTrue(Money.IsMoney("100.33"));
			Assert.IsFalse(Money.IsMoney("xyzzy"));
		}

		/// <summary>
		///A test for GetHashCode
		///</summary>
		[TestMethod]
		public void GetHashCodeTest()
		{
			const decimal VALUE = 100.25m;
			var target = new Money(VALUE);
			Assert.AreEqual(VALUE.GetHashCode(), target.GetHashCode());
		}

		/// <summary>
		///A test for Format
		///</summary>
		[TestMethod]
		public void FormatTestUsingReturnZeros()
		{
			var m = new Money(125.50m);
			Assert.AreEqual("125.50", Money.Format(m, false));
			Assert.AreEqual("125.50", Money.Format(m, true));

			var m2 = new Money();
			Assert.AreEqual(string.Empty, Money.Format(m2, false));
			Assert.AreEqual("0.00", Money.Format(m2, true));
		}

		/// <summary>
		///A test for Format using returnZeros and number of decimals
		///</summary>
		[TestMethod]
		public void FormatADecimalTestWithReturnZerosAndNumberOfDecimals()
		{
			Decimal m = 125.50m;
			Assert.AreEqual("125.50", Money.Format(m, false, 2));
			Assert.AreEqual("125.50", Money.Format(m, true, 2));
			Assert.AreEqual("125.5", Money.Format(m, true, 1));
			m = 125.51m;
			Assert.AreEqual("125.5", Money.Format(m, true, 1));
			m = 125.59m;
			Assert.AreEqual("125.6", Money.Format(m, true, 1));
			m = 0;
			Assert.AreEqual(string.Empty, Money.Format(m, false, 2));
			Assert.AreEqual("0.00", Money.Format(m, true, 2));
		}

		/// <summary>
		///A test for Format
		///</summary>
		[TestMethod]
		public void FormatADecimalAsMoneyTestWithReturnZeros()
		{
			Decimal m = 125.50m;
			Assert.AreEqual("125.50", Money.Format(m, false));
			Assert.AreEqual("125.50", Money.Format(m, true));
			m = 0;
			Assert.AreEqual(string.Empty, Money.Format(m, false));
			Assert.AreEqual("0.00", Money.Format(m, true));
		}

		/// <summary>
		///A test for Equals
		///</summary>
		[TestMethod]
		public void EqualsTest()
		{
			var target = new Money();
			object o = null;
			Assert.AreEqual(target.Equals(o), false);
			o = new Money();
			Assert.AreEqual(target.Equals(o), true);
			target = target + 100m;
			Assert.AreEqual(target.Equals(o), false);
			o = (Money) o + 100m;
			Assert.AreEqual(target.Equals(o), true);
		}

		/// <summary>
		///A test for Money Constructor from a money obj
		///</summary>
		[TestMethod]
		public void MoneyConstructorFromMoneyTest()
		{
			var m = new Money(100m);
			var target = new Money(m);
			Assert.IsTrue(m.Equals(target));
		}

		/// <summary>
		///A test for Money Constructor
		///</summary>
		[TestMethod]
		public void MoneyDefaultConstructorTest()
		{
			var target = new Money();
			Assert.AreEqual(target.Value, 0.00m);
		}

		/// <summary>
		///A test for Money Constructor
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void MoneyConstructorFromStringTest()
		{
			var target = Money.Parse(string.Empty);
			Assert.AreEqual(target, null);
			target = Money.Parse("125.44");
			Assert.AreEqual(target.Value, 125.44m);

			// should hit a format exception
			Money.Parse("xxx");
		}

		/// <summary>
		///A test for Money Constructor
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NullReferenceException))]
		public void MoneyConstructorFromDecimalTest()
		{
			var target = new Money(125.31m);
			Assert.AreEqual(target.Value, 125.31m);

			// pass in a null and should get a null ref on construction
			object o = null;
			new Money((decimal) o);
		}
	}
}