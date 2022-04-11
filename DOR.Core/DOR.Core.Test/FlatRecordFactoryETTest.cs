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
	[TestClass]
	public class FlatRecordFactoryETTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void ParseTest()
		{
			FixedRecord fr = FlatRecordFactoryET.TRecord.Parse("T 1016038055000001 6004012730903000100+0000103522494 +0000006728962ELF0000                                                                                                                              ");
			Assert.AreEqual("T ", fr[0].Value, "Record Type");
			Assert.AreEqual("101603", fr[1].Value, "Date Received");
			Assert.AreEqual("8055", fr[2].Value, "Batch Number");
			Assert.AreEqual("000001", fr[3].Value, "Serial");
			Assert.AreEqual(" ", fr[4].Value, "Filler");
			Assert.AreEqual("600401273", fr[5].Value, "TRA");
			Assert.AreEqual("09", fr[6].Value, "Period");
			Assert.AreEqual("03", fr[7].Value, "Year");
			Assert.AreEqual("0001", fr[8].Value, "Line Code");
			Assert.AreEqual("00", fr[9].Value, "Split");
			Assert.AreEqual("+0000103522494", fr[10].Value, "Gross");
			Assert.AreEqual(" ", fr[11].Value, "Filler");
			Assert.AreEqual("+0000006728962", fr[12].Value, "Tax Due");
			Assert.AreEqual("ELF", fr[13].Value, "Code");
			Assert.AreEqual("0000", fr[14].Value, "Location Code");
			Assert.AreEqual(" ", fr[15].Value, "DataType");
			Assert.AreEqual("".PadRight(125, ' '), fr[16].Value, "Filler");

			fr = FlatRecordFactoryET.MRecord.Parse("M 1016038057000055 601227545Q40100    +0000000000000   +00000000000ELF      000         102103         0000                                                                                             ");
			Assert.AreEqual(2001, fr["Year"].ParseYear(), "Year");
		}

	}
}
