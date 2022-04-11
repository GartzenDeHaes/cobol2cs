using System;
using System.Globalization;
using System.Threading;
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
	///This is a test class for PhoneNumberTest and is intended
	///to contain all PhoneNumberTest Unit Tests
	///</summary>
	[TestClass]
	public class PhoneNumberTest
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
		///A test for Suffix
		///</summary>
		[TestMethod]
		public void StaticValidatorTest()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			string[] errors;

			PhoneNumber.IsValidPhoneNumber("rew", "123", "1234", out errors);
			Assert.AreEqual("Phone Number must be numeric.", errors[0]);
		}

		/// <summary>
		///A test for Suffix
		///</summary>
		[TestMethod]
		public void SuffixTest()
		{
			var expected = 1234;
			var target = new PhoneNumber(360, 321, expected);
			var actual = target.Suffix;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Prefix
		///</summary>
		[TestMethod]
		public void PrefixTest()
		{
			var expected = 321;
			var target = new PhoneNumber(360, expected, 3456);
			int actual = target.Prefix;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Extension
		///</summary>
		[TestMethod]
		public void ExtensionTest()
		{
			var expected = 1234;
			var target = new PhoneNumber(360, 432, 3456, expected);
			int actual = target.Extension;
			Assert.AreEqual(1234, actual);
		}

		/// <summary>
		///A test for AreaCode
		///</summary>
		[TestMethod]
		public void AreaCodeTest()
		{
			var expected = 459;
			var target = new PhoneNumber(expected, 432, 3456);
			int actual = target.AreaCode;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod]
		public void ToStringTest()
		{
			// first without extension
			var target = new PhoneNumber(360, 432, 3456);
			var expected = "(360) 432-3456";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);

			// now with extension
			target = new PhoneNumber(360, 432, 3456, 1234);
			expected = "(360) 432-3456 EXT: 1234";
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToInt
		///</summary>
		[TestMethod]
		public void ToIntTest()
		{
			var target = new PhoneNumber(360, 432, 3456);
			var expected = 3604323456;
			long actual = target.ToInt();
			Assert.AreEqual(expected, actual);
			// with an extension
			target = new PhoneNumber(360, 432, 3456, 1234);
			expected = 3604323456;
			actual = target.ToInt();
			Assert.AreEqual(expected, actual);
		}


		/// <summary>
		///A test for PhoneNumber Constructor
		///</summary>
		[TestMethod]
		public void PhoneNumberConstructorTest4()
		{
			var area = 321; 
			var prefix = 543; 
			var suffix = 7654; 
			var target = new PhoneNumber(area, prefix, suffix);
			Assert.AreEqual(area, target.AreaCode);
			Assert.AreEqual(prefix, target.Prefix);
			Assert.AreEqual(suffix, target.Suffix);
		}

		///// <summary>
		/////A test for PhoneNumber default Constructor 
		///// (the default constructor has been removed). 
		/////</summary>
		//[TestMethod]
		//public void PhoneNumberConstructorTest3()
		//{
		//    var target = new PhoneNumber();
		//    Assert.AreEqual(0, target.AreaCode);
		//    Assert.AreEqual(0, target.Prefix);
		//    Assert.AreEqual(0, target.Suffix);
		//}

		/// <summary>
		///A test for PhoneNumber Constructor
		///</summary>
		[TestMethod]
		public void PhoneNumberConstructorTest2()
		{
			var area = 543;
			var prefix = 765;
			var suffix = 8765;
			var ext = 1234; 
			var target = new PhoneNumber(area, prefix, suffix, ext);
			Assert.AreEqual(area, target.AreaCode);
			Assert.AreEqual(prefix, target.Prefix);
			Assert.AreEqual(suffix, target.Suffix);
			Assert.AreEqual(1234, target.Extension);
		}

		/// <summary>
		///A test for PhoneNumber Constructor from string
		///</summary>
		[TestMethod]
		public void PhoneNumberConstructorTest1()
		{
			// with punctuation
			var phoneNum = "(360) 432-1234";
			var target = PhoneNumber.Parse(phoneNum);
			Assert.AreEqual(360, target.AreaCode);
			Assert.AreEqual(432, target.Prefix);
			Assert.AreEqual(1234, target.Suffix);

			// without punctuation
			phoneNum = "4594321234";
			target = PhoneNumber.Parse(phoneNum);
			Assert.AreEqual(459, target.AreaCode);
			Assert.AreEqual(432, target.Prefix);
			Assert.AreEqual(1234, target.Suffix);

			//not a valid way to test default constructor,
			//TryParse() throws exception "not a valid phone number"
			//so constructor is never called

			// no number - essentially a default constructor (no value at all)
			//phoneNum = "() - ";
			//target = PhoneNumber.Parse(phoneNum);
			//Assert.AreEqual(0, target.AreaCode);
			//Assert.AreEqual(0, target.Prefix);
			//Assert.AreEqual(0, target.Suffix);

			//default constructor only - passes test
			target = new PhoneNumber();
			Assert.AreEqual(0, target.AreaCode);
			Assert.AreEqual(0, target.Prefix);
			Assert.AreEqual(0, target.Suffix);
		}

		/// <summary>
		///Format error test - non-numeric
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorNonNumeric()
		{
			// 11 characters
			PhoneNumber.Parse("xxx2342340");
		}

		/// <summary>
		///Format error test - wrong length string
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorWrongLength1()
		{
			// 11 characters
			PhoneNumber.Parse("234234234560");
		}
		/// <summary>
		///Format error test - wrong length string
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorWrongLength2()
		{
			// 9 characters
				PhoneNumber.Parse("234234234");
		}

		/// <summary>
		///Format error test - Area Code
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorAreaCodeGreater999()
		{
			//existing test code:
			//new PhoneNumber(1234, 234, 2345);

			//cannot simply call constructor to test because the constructor does 
			//not attempt to parse the number for validity.
			//the way the parse works if there is only one extra digit, it sees it as a 
			//country code and it still passes. adding 2 extra digits does make the test pass,
			//however there may be a better way to validate this problem in PhoneNumber.cs
			PhoneNumber.Parse("(22234) 234-2345");
		}

		/// <summary>
		///Format error test - Prefix > 999
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorPrefixGreater999()
		{
			//new PhoneNumber(123, 2344, 2345);
			PhoneNumber.Parse("(224) 25634-2345");
		}

		/// <summary>
		///Format error test - Suffix > 9999
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorSuffixGreater9999()
		{
			//new PhoneNumber(123, 234, 23451);
			PhoneNumber.Parse("(234) 234-239845");
		}

		/// <summary>
		///Format error test - Prefix &lt; 200
		///</summary>
		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void PhoneNumberConstructorPrefixLessThan100()
		{
			//new PhoneNumber(123, 99, 2345);
			PhoneNumber.Parse("(234) 24-2345");
		}

		/// <summary>
		///A test for PhoneNumber Constructor from long int
		///</summary>
		[TestMethod]
		public void PhoneNumberConstructorTest()
		{
			var phoneNum = 2345678901; 
			var target = new PhoneNumber(phoneNum);
			Assert.AreEqual(234, target.AreaCode);
			Assert.AreEqual(567, target.Prefix);
			Assert.AreEqual(8901, target.Suffix);
		}

		/// <summary>
		/// IsValidPhoneNumber test
		///</summary>
		[TestMethod]
		public void PhoneNumberIsValidTest()
		{
			Assert.IsTrue(PhoneNumber.IsValidPhoneNumber("3601231234"));
			Assert.IsTrue(PhoneNumber.IsValidPhoneNumber("(360) 123-1234"));
			Assert.IsTrue(PhoneNumber.IsValidPhoneNumber("(360)123-1234"));
			Assert.IsTrue(PhoneNumber.IsValidPhoneNumber("360-123-1234"));

			Assert.IsFalse(PhoneNumber.IsValidPhoneNumber("(360 123-1234"));
			Assert.IsFalse(PhoneNumber.IsValidPhoneNumber("360) 123-1234"));
			Assert.IsFalse(PhoneNumber.IsValidPhoneNumber("360-123-123"));
			Assert.IsFalse(PhoneNumber.IsValidPhoneNumber("60 123-1234"));
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest()
		{
			string phoneNum = "3603421111";
			PhoneNumber expected = new PhoneNumber(360,342, 1111); 
			PhoneNumber actual;
			actual = PhoneNumber.Parse(phoneNum);
			Assert.AreEqual(expected.ToString(), actual.ToString());	
			Assert.AreEqual(expected.Prefix, actual.Prefix);
			Assert.AreEqual(expected.Suffix, actual.Suffix);
			Assert.AreEqual(expected.AreaCode, actual.AreaCode);
			actual = PhoneNumber.Parse("2344322343");
			Assert.AreNotEqual(expected.ToString(), actual.ToString());

		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest2()
		{
			string phoneNum = "4561237891";
			PhoneNumber expected = new PhoneNumber(456, 123, 7891);
			PhoneNumber actual;
			actual = PhoneNumber.Parse(phoneNum);
			Assert.AreEqual(expected.ToString(), actual.ToString());
			Assert.AreEqual(expected.Prefix, actual.Prefix);
			Assert.AreEqual(expected.Suffix, actual.Suffix);
			Assert.AreEqual(expected.AreaCode, actual.AreaCode);
			actual = PhoneNumber.Parse("986-895-8877");
			Assert.AreNotEqual(expected.ToString(), actual.ToString());

		}

		/// <summary>
		///A test for Parse using an extention
		///</summary>
		[TestMethod()]
		public void ParseTest3()
		{
			string phoneNum = "4561237891 ext 4";
			PhoneNumber expected = new PhoneNumber(456, 123, 7891);
			PhoneNumber actual;
			actual = PhoneNumber.Parse(phoneNum);
			Assert.AreEqual(expected.Prefix, actual.Prefix);
			Assert.AreEqual(expected.Suffix, actual.Suffix);
			Assert.AreEqual(expected.AreaCode, actual.AreaCode);
		}
	}
}