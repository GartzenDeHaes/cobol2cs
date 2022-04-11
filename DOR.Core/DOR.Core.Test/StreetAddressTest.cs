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
	///This is a test class for StreetAddressTest and is intended
	///to contain all StreetAddressTest Unit Tests
	///</summary>
	[TestClass]
	public class StreetAddressTest
	{
		#region Members

		private readonly CultureInfo m_cultureInfo = new CultureInfo("es-ES");

		#endregion Members

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
		///A test for StreetAddress Constructor when invalid state is provided
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void StreetAddressConstructorForZipAsStrTest()
		{
			var addressLine1 = "220 Linderson Way";
			var city = "Tumwater";
			var state = "Washington";
			var zip = "98501";
			new StreetAddress(addressLine1, city, state, zip);
		}

		/// <summary>
		///A test for StreetAddress Constructor when Zip extension is invalid
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void StreetAddressConstructorForNonNumericZipTest()
		{
			var addressLine1 = "220 Linderson Way";
			var city = "Tumwater";
			var state = "WA";
			var zip5 = "98501";
			var zipdash4 = "568A";
			new StreetAddress(addressLine1, city, state, zip5, zipdash4);
		}

		/// <summary>
		///A test for StreetAddress Constructor when Zip extension is invalid
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void StreetAddressConstructorForInvalidZipExtensionLengthTest()
		{
			var addressLine1 = "220 Linderson Way";
			var city = "Tumwater";
			var state = "WA";
			var zip5 = "98501";
			var zipdash4 = "56811";
			new StreetAddress(addressLine1, city, state, zip5, zipdash4);
		}

		/// <summary>
		///A test for StreetAddress Constructor when Zip is non-numeric
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void StreetAddressConstructorForInvalidZipTest()
		{
			var addressLine1 = "220 Linderson Way";
			var state = "WA";
			var zip5 = "9850A";
			var zipdash4 = "5681";
			new StreetAddress(addressLine1, null, state, zip5, zipdash4);
		}

		/// <summary>
		///A test for StreetAddress Constructor when Zip code length is invalid
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (FormatException))]
		public void StreetAddressConstructorForNonInvalidZipLengthTest()
		{
			var addressLine1 = "220 Linderson Way";
			var state = "WA";
			var zip5 = "";
			var zipdash4 = "";
			new StreetAddress(addressLine1, null, state, zip5, zipdash4);
		}

		///// <summary>
		/////A test for Standarized test
		/////</summary>
		//[TestMethod]
		//public void StandardizeTest()
		//{
		//    var addressLine1 = "220 Israel Road SE";
		//    var city = "Tumwater";
		//    var zip5 = "98501";
		//    var zipdash4 = "6458";
		//    var expected = new StreetAddress("220 ISRAEL RD SE", "TUMWATER", "WA", new ZIP(98501));
		//    StreetAddress actual = StreetAddress.Standardize(addressLine1, city, zip5, zipdash4);
		//    Assert.AreEqual(expected.Line1, actual.Line1);
		//    Assert.AreEqual(expected.City, actual.City);
		//    Assert.AreEqual(expected.ZIP.ToString(), actual.ZIP.ToString());
		//}

		/// <summary>
		///A test for StreetAddress Constructor
		///</summary>
		[TestMethod]
		public void StreetAddressConstructorForZipTest()
		{
			var addressLine1 = "220 Israel Road SE";
			var state = "WA";
			var zip5 = "98501";
			var zipdash4 = "6458";
			var target = new StreetAddress(addressLine1, null, state, zip5, zipdash4);
			Assert.IsTrue(target.GetType() == typeof (StreetAddress));
			Assert.AreEqual(addressLine1, target.Line1);
			Assert.AreEqual(null, target.City);
			Assert.AreEqual(state, target.State);
			Assert.AreEqual("98501", target.ZIP.Base.ToString());
			Assert.AreEqual("6458", target.ZIP.Extension.ToString());
		}

		/// <summary>
		///A test for IsAddressXorZipPlus4Valid
		///</summary>
		[TestMethod]
		public void IsAddressXorZipPlus4ValidTest()
		{
			var addressLine1 = "220 Israel Road SE";
			var city = "Tumwater";
			var state = "WA";
			var zip5 = "98501";
			var zipdash4 = "6458";
			string[] errorMessage;
			bool actual = StreetAddress.IsAddressXorZipPlus4Valid(addressLine1, city, state, zip5, zipdash4, out errorMessage);
			Assert.AreEqual(true, actual);
		}

		/// <summary>
		///A test for IsAddressXorZipPlus4Valid when Zip fields are empty
		///</summary>
		[TestMethod]
		public void IsAddressXorZipPlus4ValidForEmptyZipFieldsTest()
		{
			var addressLine1 = "220 Israel Road SE";
			var city = "Tumwater";
			var state = "WA";
			string zip5 = string.Empty;
			string zipdash4 = string.Empty;
			string[] errorMessage;
			var errorMessageExpected = "ZIP is required";
			bool actual = StreetAddress.IsAddressXorZipPlus4Valid(addressLine1, city, state, zip5, zipdash4, out errorMessage);
			Assert.AreEqual(errorMessageExpected, errorMessage[0]);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for IsAddressValid when no city is provided
		///</summary>
		[TestMethod]
		public void IsAddressValidWithNoStreetTest()
		{
			Thread.CurrentThread.CurrentCulture = m_cultureInfo;

			var addressLine1 = "220 Israel Road SE";
			var city = String.Empty;
			var state = "WA";
			var zip5 = String.Empty;
			string[] errorMessage;
			var errorMessageExpected = "City is required";
			bool actual = StreetAddress.IsAddressValid(addressLine1, city, state, zip5, out errorMessage);
			Assert.AreEqual(errorMessageExpected, errorMessage[0]);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for IsAddressValid when no city is provided
		///</summary>
		[TestMethod]
		public void IsAddressValidWithNoCityTest()
		{
			var addressLine1 = String.Empty;
			var city = "Tumwater";
			var state = "WA";
			var zip5 = "98501";
			string[] errorMessage;
			var errorMessageExpected = "Street address is required";
			bool actual = StreetAddress.IsAddressValid(addressLine1, city, state, zip5, out errorMessage);
			Assert.AreEqual(errorMessageExpected, errorMessage[0]);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for IsAddressValid when no state abbreviation is incorrect
		///</summary>
		[TestMethod]
		public void IsAddressValidWithNoStateTest()
		{
			var addressLine1 = "220 Israel Road SE";
			var city = "Tumwater";
			var state = "WAC";
			var zip5 = "98501";
			string[] errorMessage;
			var errorMessageExpected = "Invalid state abbreviation";
			bool actual = StreetAddress.IsAddressValid(addressLine1, city, state, zip5, out errorMessage);
			Assert.AreEqual(errorMessageExpected, errorMessage[0]);
			Assert.AreEqual(false, actual);
		}

		/// <summary>
		///A test for StreetAddress Constructor
		///</summary>
		[TestMethod]
		public void StreetAddressConstructorTest()
		{
			var addressLine1 = "220 Israel Road SE";
			var city = "Tumwater";
			var state = "WA";
			var zip = "98501";
			var target = new StreetAddress(addressLine1, city, state, zip);
			Assert.IsTrue(target.GetType() == typeof (StreetAddress));
			Assert.AreEqual(addressLine1, target.Line1);
			Assert.AreEqual(city, target.City);
			Assert.AreEqual(state, target.State);
			Assert.AreEqual(zip, target.ZIP.ToString());
		}
	}
}