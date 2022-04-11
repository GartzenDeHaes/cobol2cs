using DOR.WorkingStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DOR.WorkingStorage.Pic;

namespace DOR.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for WsRecordTest and is intended
    ///to contain all WsRecordTest Unit Tests
    ///</summary>
	[TestClass()]
	public class WsRecordTest
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
		///A test for Add
		///</summary>
		[TestMethod()]
		public void StorageTest()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			buf.Set("000000000-200907102460--700000000000000120090701", 0, 48);
			
			WsRecord rec = new WsRecord("TRA-ACTIVITY-REC", buf);
			rec.Add("TraActivityInvertTime", new WsRecord(buf, 22, 9, null, false, null, "TRA-ACTIVITY-INVERT-TIME", "", "S9(8)", true));
			rec["TraActivityInvertTime"].Set(-7000000);
			Assert.AreEqual("-07000000", rec["TraActivityInvertTime"].ToString());

			rec.Add("1", new WsRecord(buf, 0, 1, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "9", true));
			rec.Add("2", new WsRecord(buf, 1, 1, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "9", true));
			rec.Add("3", new WsRecord(buf, 2, 1, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "9", true));
			rec["1"].Set(1);
			rec["2"].Set(2);
			rec["3"].Set(3);

			Assert.AreEqual("1", rec["1"].ToString());
			Assert.AreEqual("2", rec["2"].ToString());
			Assert.AreEqual("3", rec["3"].ToString());

			rec.Add("-1", new WsRecord(buf, 0, 2, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "S9", true));
			rec.Add("-2", new WsRecord(buf, 2, 2, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "S9", true));
			rec.Add("-3", new WsRecord(buf, 4, 2, null, false, new string[] { }, "TRA-ACTIVITY-INVERT-TIME", "", "S9", true));
			rec["-1"].Set(-1);
			rec["-2"].Set(-2);
			rec["-3"].Set(-3);
			Assert.AreEqual("-1", rec["-1"].ToString());
			Assert.AreEqual("-2", rec["-2"].ToString());
			Assert.AreEqual("-3", rec["-3"].ToString());
			rec["-1"].Set(1);
			rec["-2"].Set(2);
			rec["-3"].Set(3);
			Assert.AreEqual("+1", rec["-1"].ToString());
			Assert.AreEqual("+2", rec["-2"].ToString());
			Assert.AreEqual("+3", rec["-3"].ToString());

			rec.Add("DDD", new WsRecord(buf, 0, 10, null, false, new string[] { }, "", "", "9(7)V9(3)", true));
			rec["DDD"].Set(.15);
			Assert.AreEqual("0000000150", rec["DDD"].ToRawString());
			Assert.AreEqual((decimal)0.15, rec["DDD"].ToDecimal());
			rec.Add("F", new WsRecord(buf, 0, 4, null, false, new string[] { }, "", "", "9.99", true));
			rec["F"].Set(.25);
			Assert.AreEqual((decimal)0.25, rec["F"].ToDecimal());
			rec["DDD"].Set(rec["F"]);
			Assert.AreEqual((decimal)0.25, rec["DDD"].ToDecimal());
		}

		[TestMethod()]
		public void AdditionTest()
		{
			MemoryBuffer buf = new MemoryBuffer(25);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1", 
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(3)", false)
			);

			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 4, 4, null, false, null, "NUM2", "", "S9(3)", false)
			);

			rec["NUM1"].Set(1);
			rec["NUM2"].Set(1);
			NumericTemp lhs = rec["NUM1"] + rec["NUM2"];
			Assert.AreEqual(2, lhs.ToInt());

			rec["NUM1"].Set(-1);
			rec["NUM2"].Set(-1);
			lhs = rec["NUM1"] + rec["NUM2"];
			Assert.AreEqual(-2, lhs.ToInt());

			lhs = rec["NUM1"] + lhs;
			Assert.AreEqual(-3, lhs.ToInt());

			rec["NUM1"].Set(1);
			rec["NUM2"].Set(1);
			lhs = rec["NUM1"] + 1;
			Assert.AreEqual(2, lhs.ToInt());
			lhs = rec["NUM1"] + 1.0;
			Assert.AreEqual(2, lhs.ToInt());

			rec.Add
			(
				"NUM3",
				new WsRecord(buf, 8, 4, null, false, null, "NUM3", "", "S9(1)V9(2)", false)
			);
			Assert.AreEqual(0, rec["NUM3"].ToDecimal());

			rec["NUM3"].Set(1.1);
			lhs = rec["NUM1"] + rec["NUM3"];
			Assert.AreEqual(2.1m, lhs.ToDecimal());
			lhs = rec["NUM3"] + rec["NUM1"];
			Assert.AreEqual(2.1m, lhs.ToDecimal());
			lhs = rec["NUM3"] + lhs;
			Assert.AreEqual(3.2m, lhs.ToDecimal());
			lhs = rec["NUM3"] + 1;
			Assert.AreEqual(2.1m, lhs.ToDecimal());

			rec.Add
			(
				"NUM4",
				new WsRecord(buf, 12, 4, null, false, null, "NUM4", "", "9(2)V9(2)", false)
			);

			rec["NUM4"].Set(2);
			lhs = rec["NUM4"] + rec["NUM4"];
			Assert.AreEqual(4, lhs.ToInt());

			lhs = rec["NUM4"] + rec["NUM1"];
			Assert.AreEqual(3, lhs.ToInt());

			lhs = rec["NUM4"] + rec["NUM3"];
			Assert.AreEqual(3.1m, lhs.ToDecimal());

			rec.Add
			(
				"NUM5",
				new WsRecord(buf, 16, 4, null, false, null, "NUM5", "", "9(1)V9(3)", false)
			);

			rec["NUM5"].Set(2);
			lhs = rec["NUM4"] + rec["NUM5"];
			Assert.AreEqual(4, lhs.ToInt());

			lhs = rec["NUM5"] + rec["NUM1"];
			Assert.AreEqual(3, lhs.ToInt());

			lhs = rec["NUM5"] + rec["NUM3"];
			Assert.AreEqual(3.1m, lhs.ToDecimal());

			rec.Add
			(
				"NUM6",
				new WsRecord(buf, 20, 4, null, false, null, "NUM6", "", "9(4)", false)
			);

			rec["NUM6"].Set(2);
			lhs = rec["NUM6"] + rec["NUM4"];
			Assert.AreEqual(4, lhs.ToInt());

			lhs = rec["NUM6"] + rec["NUM1"];
			Assert.AreEqual(3, lhs.ToInt());

			lhs = rec["NUM6"] + rec["NUM3"];
			Assert.AreEqual(3.1m, lhs.ToDecimal());
		}

		[TestMethod()]
		public void SubtractionTest()
		{
			MemoryBuffer buf = new MemoryBuffer(25);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(3)", false)
			);

			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 4, 4, null, false, null, "NUM2", "", "S9(3)", false)
			);

			rec["NUM1"].Set(2);
			rec["NUM2"].Set(1);
			NumericTemp lhs = rec["NUM1"] - rec["NUM2"];
			Assert.AreEqual(1, lhs.ToInt());

			rec["NUM1"].Set(-1);
			rec["NUM2"].Set(-2);
			lhs = rec["NUM1"] - rec["NUM2"];
			Assert.AreEqual(1, lhs.ToInt());

			lhs = rec["NUM1"] - lhs;
			Assert.AreEqual(-2, lhs.ToInt());

			rec["NUM1"].Set(3);
			rec["NUM2"].Set(1);
			lhs = rec["NUM1"] - 1;
			Assert.AreEqual(2, lhs.ToInt());
			lhs = rec["NUM1"] - 1.0;
			Assert.AreEqual(2, lhs.ToInt());

			rec.Add
			(
				"NUM3",
				new WsRecord(buf, 8, 4, null, false, null, "NUM3", "", "S9(1)V9(2)", false)
			);
			Assert.AreEqual(0, rec["NUM3"].ToDecimal());

			rec["NUM3"].Set(1.1);
			lhs = rec["NUM1"] - rec["NUM3"];
			Assert.AreEqual(1.9m, lhs.ToDecimal());
			lhs = rec["NUM3"] - rec["NUM1"];
			Assert.AreEqual(-1.9m, lhs.ToDecimal());
			lhs = rec["NUM3"] - lhs;
			Assert.AreEqual(3m, lhs.ToDecimal());
			lhs = rec["NUM3"] - 1;
			Assert.AreEqual(0.1m, lhs.ToDecimal());

			rec.Add
			(
				"NUM4",
				new WsRecord(buf, 12, 4, null, false, null, "NUM4", "", "9(2)V9(2)", false)
			);

			rec["NUM4"].Set(2);
			lhs = rec["NUM4"] - rec["NUM4"];
			Assert.AreEqual(0, lhs.ToInt());

			lhs = rec["NUM4"] - rec["NUM1"];
			Assert.AreEqual(-1, lhs.ToInt());

			lhs = rec["NUM4"] - rec["NUM3"];
			Assert.AreEqual(0.9m, lhs.ToDecimal());

			rec.Add
			(
				"NUM5",
				new WsRecord(buf, 16, 4, null, false, null, "NUM5", "", "9(1)V9(3)", false)
			);

			rec["NUM5"].Set(2);
			lhs = rec["NUM4"] - rec["NUM5"];
			Assert.AreEqual(0, lhs.ToInt());

			lhs = rec["NUM5"] - rec["NUM1"];
			Assert.AreEqual(-1, lhs.ToInt());

			lhs = rec["NUM5"] - rec["NUM3"];
			Assert.AreEqual(0.9m, lhs.ToDecimal());

			rec.Add
			(
				"NUM6",
				new WsRecord(buf, 20, 4, null, false, null, "NUM6", "", "9(4)", false)
			);

			rec["NUM6"].Set(2);
			lhs = rec["NUM6"] - rec["NUM4"];
			Assert.AreEqual(0, lhs.ToInt());

			lhs = rec["NUM6"] - rec["NUM1"];
			Assert.AreEqual(-1, lhs.ToInt());

			lhs = rec["NUM6"] - rec["NUM3"];
			Assert.AreEqual(0.9m, lhs.ToDecimal());
		}

		[TestMethod()]
		public void MultTest()
		{
			MemoryBuffer buf = new MemoryBuffer(25);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(3)", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 4, 3, null, false, null, "NUM2", "", "S9(2)", false)
			);
			rec.Add
			(
				"NUM2B",
				new WsRecord(buf, 4, 4, null, false, null, "NUM2B", "", "9(4)", false)
			);
			rec["NUM1"].Set(2);
			rec["NUM2"].Set(2);

			NumericTemp lhs = rec["NUM1"] * rec["NUM2"];
			Assert.AreEqual(4, lhs.ToInt());
			rec["NUM2B"].Set(2);
			lhs = rec["NUM1"] * rec["NUM2B"];
			Assert.AreEqual(4, lhs.ToInt());
			lhs = rec["NUM1"] * 3;
			Assert.AreEqual(6, lhs.ToInt());

			rec.Add
			(
				"NUM3",
				new WsRecord(buf, 8, 4, null, false, null, "NUM3", "", "S9(1)V9(2)", false)
			);
			rec["NUM3"].Set(3);

			lhs = rec["NUM1"] * rec["NUM3"];
			Assert.AreEqual(6.0m, lhs.ToDecimal());
			lhs = rec["NUM2B"] * rec["NUM3"];
			Assert.AreEqual(6.0m, lhs.ToDecimal());
			lhs = rec["NUM3"] * 2;
			Assert.AreEqual(6.0m, lhs.ToDecimal());

			rec.Add
			(
				"NUM4",
				new WsRecord(buf, 12, 4, null, false, null, "NUM4", "", "9(2)V9(2)", false)
			);
			rec["NUM4"].Set(4);

			rec.Add
			(
				"NUM5",
				new WsRecord(buf, 16, 4, null, false, null, "NUM5", "", "9(1)V9(3)", false)
			);
			rec["NUM5"].Set(5);

			rec.Add
			(
				"NUM6",
				new WsRecord(buf, 20, 4, null, false, null, "NUM6", "", "9(4)", false)
			);
			rec["NUM6"].Set(6);
		}

		[TestMethod()]
		public void DivTest()
		{
			MemoryBuffer buf = new MemoryBuffer(24);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(3)", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 4, 3, null, false, null, "NUM2", "", "S9(2)", false)
			);
			rec["NUM1"].Set(6);
			rec["NUM2"].Set(3);

			NumericTemp lhs = rec["NUM1"] / rec["NUM2"];
			Assert.AreEqual(2, lhs.ToInt());
		}

		[TestMethod()]
		public void RelopTest()
		{
			MemoryBuffer buf = new MemoryBuffer(24);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(3)", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 4, 3, null, false, null, "NUM2", "", "S9(2)", false)
			);
			rec.Add
			(
				"NUM3",
				new WsRecord(buf, 8, 4, null, false, null, "NUM3", "", "S9(1)V9(2)", false)
			);

			rec["NUM1"].Set(6);
			rec["NUM2"].Set(3);
			rec["NUM3"].Set(-6);

			Assert.IsFalse(rec["NUM1"] == rec["NUM2"]);
			Assert.IsTrue(rec["NUM1"] == 6);
			Assert.IsFalse(rec["NUM1"] != 6);
			Assert.IsTrue(rec["NUM1"] > 5);
			Assert.IsTrue(rec["NUM1"] < 7);
			Assert.IsTrue(rec["NUM1"] >= 6);
			Assert.IsTrue(rec["NUM1"] <= 6);
			Assert.IsFalse(rec["NUM1"] >= 7);
			Assert.IsFalse(rec["NUM1"] <= 5);
			Assert.IsFalse(rec["NUM1"] < 5);
			Assert.IsFalse(rec["NUM1"] > 7);
			Assert.IsTrue(rec["NUM1"] != rec["NUM2"]);
			Assert.IsFalse(rec["NUM1"] < rec["NUM2"]);
			Assert.IsFalse(rec["NUM1"] < rec["NUM3"]);
			Assert.IsTrue(rec["NUM1"] > rec["NUM2"]);
			Assert.IsFalse(rec["NUM1"] <= rec["NUM2"]);
			Assert.IsTrue(rec["NUM1"] >= rec["NUM2"]);

			Assert.IsTrue(rec["NUM1"] == new NumericTemp(6.0m));
			Assert.IsFalse(rec["NUM1"] != new NumericTemp(6.0m));
			Assert.IsTrue(rec["NUM1"] > new NumericTemp(5.0m));
			Assert.IsTrue(rec["NUM1"] < new NumericTemp(7.0m));
			Assert.IsTrue(rec["NUM1"] >= new NumericTemp(6.0m));
			Assert.IsTrue(rec["NUM1"] <= new NumericTemp(6.0m));
			Assert.IsFalse(rec["NUM1"] >= new NumericTemp(7.0m));
			Assert.IsFalse(rec["NUM1"] <= new NumericTemp(5.0m));
			Assert.IsFalse(rec["NUM1"] < new NumericTemp(5.0m));
			Assert.IsFalse(rec["NUM1"] > new NumericTemp(7.0m));
			
			rec["NUM3"].Set(3);
			Assert.IsFalse(rec["NUM3"] != rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] == rec["NUM2"]);
			Assert.IsFalse(rec["NUM3"] < rec["NUM2"]);
			Assert.IsFalse(rec["NUM3"] > rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] <= rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] >= rec["NUM2"]);

			rec["NUM3"].Set(-3);
			Assert.IsFalse(rec["NUM3"] == rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] != rec["NUM2"]);
			Assert.IsFalse(rec["NUM3"] > rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] < rec["NUM2"]);
			Assert.IsFalse(rec["NUM3"] >= rec["NUM2"]);
			Assert.IsTrue(rec["NUM3"] <= rec["NUM2"]);
		}

		[TestMethod()]
		public void SignedUnsignedAssignmentTest()
		{
			MemoryBuffer buf = new MemoryBuffer(13);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 6, null, false, null, "NUM1", "", "9(3)V9(3)", false)
			);
			rec.Add
			(
				"NUMS",
				new WsRecord(buf, 6, 7, null, false, null, "NUM2", "", "S9(3)V9(3)", false)
			);

			rec["NUMS"].Set("+123.456");
			Assert.AreEqual("+123.456", rec["NUMS"].ToString());

			rec["NUM1"].Set(rec["NUMS"]);
			Assert.AreEqual("+123.456", rec["NUMS"].ToString());
			Assert.AreEqual("123.456", rec["NUM1"].ToString());
			Assert.AreEqual("123.4", rec["NUM1"].ToString("---.-"));
		}
		
		[TestMethod()]
		public void SignedToMaskTest()
		{
			MemoryBuffer buf = new MemoryBuffer(13);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(2)V9", false)
			);
			rec["NUM1"].Set(34.2m);
			Assert.AreEqual("   34.2", rec["NUM1"].ToString("-,--9.9"));
		}

		[TestMethod()]
		public void SignedToXTest()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 8, null, false, null, "NUM1", "", "S9(4)V999", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 8, 6, null, false, null, "NUM2", "", "X(6)", false)
			);

			rec["NUM1"].Set(8);
			rec["NUM2"].Set(rec["NUM1"]);
			Assert.AreEqual("000800", rec["NUM2"].ToString());

			rec["NUM1"].Set(-8);
			rec["NUM2"].Set(rec["NUM1"]);
			Assert.AreEqual("000800", rec["NUM2"].ToString());

			rec.Add
			(
				"X8",
				new WsRecord(buf, 20, 8, null, false, null, "X8", "", "X(8)", false)
			);
			rec["NUM1"].Set(8);
			rec["X8"].Set(rec["NUM1"]);
			Assert.AreEqual("0008000 ", rec["X8"].ToString());
		}

		[TestMethod()]
		public void SignedToPlusMaskTest()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 8, null, false, null, "NUM1", "", "S9(4)V999", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 8, 6, null, false, null, "NUM2", "", "++++.9", false)
			);

			Assert.AreEqual(3, rec["NUM2"].Format.CharClassCount);

			rec["NUM1"].Set(8.0m);
			rec["NUM2"].Set(rec["NUM1"]);
			Assert.AreEqual("  +8.0", rec["NUM2"].ToString());

			rec["NUM1"].Set(-1m);
			rec["NUM2"].Set(rec["NUM1"]);
			Assert.AreEqual("  -1.0", rec["NUM2"].ToString());

			rec.Add
			(
				"X6",
				new WsRecord(buf, 20, 6, null, false, null, "X6", "", "X(6)", false)
			);

			rec["X6"].Set(rec["NUM2"]);
			Assert.AreEqual("  -1.0", rec["X6"].ToString());
		}

		[TestMethod()]
		public void SignedToPlusMaskTest2()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 4, null, false, null, "NUM1", "", "S9(2)V9", false)
			);
			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 8, 5, null, false, null, "NUM2", "", "--9.9", false)
			);

			rec["NUM1"].Set(8.0m);
			rec["NUM2"].Set(rec["NUM1"]);
			Assert.AreEqual("  8.0", rec["NUM2"].ToString());
			Assert.AreEqual("  8.0", rec["NUM1"].ToString("--9.9"));
		}

		[TestMethod()]
		public void SignedToMaskTest3()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 5, null, false, null, "NUM1", "", "S9(3)V9", false)
			);

			rec["NUM1"].Set(0m);
			Assert.AreEqual("    0.0", rec["NUM1"].ToString("-,--9.9"));
			Assert.AreEqual(6, rec["NUM1"].Format.DisplayLength);
		}

		[TestMethod()]
		public void MaskRegressionTest4()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 5, null, false, null, "NUM1", "", "S9(3)V9", false)
			);

			rec["NUM1"].Set(1.0m);
			Assert.AreEqual("  1.0", rec["NUM1"].ToString("---.-"));
		}

		[TestMethod()]
		public void MaskRegressionTest5()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 7, null, false, null, "NUM1", "", "S9(3)V999", false)
			);

			rec.Add
			(
				"NUM2",
				new WsRecord(buf, 8, 6, null, false, null, "NUM2", "", "ZZ9.9-", false)
			);

			rec["NUM1"].Set(123.4m);
			rec["NUM2"].Set(rec["NUM1"]);

			Assert.AreEqual("123.4 ", rec["NUM2"].ToString());
		}

		[TestMethod()]
		public void RoundingTest()
		{
			MemoryBuffer buf = new MemoryBuffer(48);
			WsRecord rec = new WsRecord("REC", buf);

			rec.Add
			(
				"NUM1",
				new WsRecord(buf, 0, 5, null, false, null, "NUM1", "", "S9(3)V9", false)
			);

			rec["NUM1"].Set(new NumericTemp(1.144m), true);
			Assert.AreEqual(1.1m, rec["NUM1"].ToDecimal());

			rec["NUM1"].Set(new NumericTemp(1.156m), true);
			Assert.AreEqual(1.2m, rec["NUM1"].ToDecimal());
		}
	}
}
