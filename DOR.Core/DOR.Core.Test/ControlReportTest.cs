#if INTEGRATION
using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;
using Assert = NUnit.Framework.Assert;
using TestMethod = NUnit.Framework.TestAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using System;
using System.IO;
using System.Text;

using DOR.Core;

namespace DOR.Core.Test
{
	/// <summary>
	///This is a test class for ControlReportTest and is intended
	///to contain all ControlReportTest Unit Tests
	///</summary>
	[TestClass]
	public class ControlReportTest
	{
		#region Constants

		private const string TEST_REPORT_SUB_DIR_NAME = "ReportsForTest";
		private const string TEST_REPORT_PROCESS_NAME = "ControlReportTest";
		#endregion Constants

		#region Properties

		#endregion Properties
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
		[ClassInitialize]
#if INTEGRATION
		public void MyClassInitialize()
#else		
		public static void MyClassInitialize(TestContext testContext)
#endif
		{
			string testReportDir = Directory.GetCurrentDirectory() + "\\" + TEST_REPORT_SUB_DIR_NAME;
			if (Directory.Exists(testReportDir))
				Directory.Delete(testReportDir, true);
		}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup]
#if INTEGRATION
		public void MyClassCleanup()
#else
		public static void MyClassCleanup()
#endif
		{
			string testReportDir = Directory.GetCurrentDirectory() + "\\" + TEST_REPORT_SUB_DIR_NAME;
			if (Directory.Exists(testReportDir))
				Directory.Delete(testReportDir, true);
		}
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
		///A test for HasErrors and ErrorCount
		///</summary>
		[TestMethod]
		public void HasErrorsTest()
		{
			var target = CreateControlReportForTest();

			Assert.IsFalse(target.HasErrors);

			const string TEST_ERROR1 = "this is a test error by string";
			const string TEST_ERROR2 = "this is a test error by exception";

			// add an error by string message
			target.AddError(TEST_ERROR1);

			Assert.AreEqual(1, target.ErrorCount, "Error count s/b 1");
			Assert.IsTrue(target.HasErrors,
				"Target should have errors after adding 1 error");

			// add an error by exception
			target.AddError(new Exception(TEST_ERROR2));

			Assert.AreEqual(2, target.ErrorCount, "Error count s/b 2");
			Assert.IsTrue(target.HasErrors,
						"Target should have errors after adding 2 error");

			Assert.IsTrue(StringIsInReportFile(
							target.ExceptionReport.Filename,
							TEST_ERROR1));
			Assert.IsTrue(StringIsInReportFile(
							target.ExceptionReport.Filename,
							TEST_ERROR2));
		}

		/// <summary>
		///A test for the ExceptionReport attached to a Control Report
		///</summary>
		[TestMethod]
		public void ExceptionReportTest()
		{
			var target = CreateControlReportForTest();

			Assert.AreEqual(0, target.ExceptionReport.ErrorCount);
			Assert.IsFalse(target.ExceptionReport.HasErrors);

			// add an error by string message
			target.AddError("this is a test error");

			Assert.AreEqual(1, target.ExceptionReport.ErrorCount);
			Assert.IsTrue(target.ExceptionReport.HasErrors);

			// add an error by exception
			target.AddError(new Exception("this is a test error by exception"));

			Assert.AreEqual(2, target.ExceptionReport.ErrorCount);
			Assert.IsTrue(target.ExceptionReport.HasErrors);

			// add an error directly to the Exception Report
			// it should increment the total on the Control report

			target.ExceptionReport.AppendError(
					"Error added directly to Except report by string AppendError");
			Assert.AreEqual(3, target.ErrorCount);

			// add another error directly to the Exception Report via addMessage
			// it should increment the total on the Control report

			target.ExceptionReport.AddMessage(
					"Error added directly to Except report by string AddMessage");
			Assert.AreEqual(4, target.ErrorCount);

		}

		/// <summary>
		///A test for RunComplete
		///</summary>
		[TestMethod]
		public void RunCompleteTest()
		{
			/*
			 * test 1 - no errors
			 */
			var target = CreateControlReportForTest();
			target.RunComplete();
			// if no errors, RunComplete should write "IN BALANCE" to the Control Report
			Assert.IsTrue(StringIsInReportFile(target.Filename, 
							"CONTROL REPORT IN BALANCE"));

			/*
			 * test 2 - with errors 
			 */
			target = CreateControlReportForTest();

			target.AddError("This is a test Error");
			target.RunComplete();

			// if no errors, RunComplete should write "IN BALANCE" to 
			// the Control Report
			Assert.IsFalse(StringIsInReportFile(target.Filename, 
				"CONTROL REPORT IN BALANCE"));
			Assert.IsTrue(
				StringIsInReportFile(target.Filename,
							"!!! ERRORS IN PROCESSING: SEE EXCEPTION REPORT !!!")
				);
		}

		/// <summary>
		///A test for AddMessage
		///</summary>
		[TestMethod]
		public void AddMessageTest()
		{
			var target = CreateControlReportForTest();
			var msg = "This is a test control report message";
			target.AddMessage(msg);
			Assert.IsTrue(StringIsInReportFile(target.Filename, msg));
			// this is just a negative test to assure that StringIsInReportFile
			// works correctly
			Assert.IsFalse(StringIsInReportFile(target.Filename, msg + "xxxxx"));
		}

		/// <summary>
		///A test for AddError by Exception
		///</summary>
		[TestMethod]
		public void AddErrorTestByException()
		{
			ControlReportForTest target = CreateControlReportForTest();
			const string ERROR_MSG = 
				"This is a test error message for AddErrorTestByException";
			target.AddError(new Exception(ERROR_MSG));
			Assert.IsTrue(StringIsInReportFile(
					target.ExceptionReport.Filename, 
					ERROR_MSG));
		}


		/// <summary>
		///A test for AddError by String
		///</summary>
		[TestMethod]
		public void AddErrorTestByString()
		{
			const string ERROR_MSG =
				"This is a test error message for AddErrorTestByString";
			var target = CreateControlReportForTest();
			target.AddError(ERROR_MSG);
			Assert.IsTrue(StringIsInReportFile(
					target.ExceptionReport.Filename,
					ERROR_MSG));
		}

		/// <summary>
		///A test for ControlReport Constructor
		///</summary>
		[TestMethod]
		public void ControlReportConstructorTestFromRelativePath()
		{
			var target = new ControlReportForTest(TEST_REPORT_SUB_DIR_NAME, 
												TEST_REPORT_PROCESS_NAME);
			var expectedFileName = string.Format("{0}\\{1}\\_{2}_CNTRL.txt", 
												Directory.GetCurrentDirectory(), 
												TEST_REPORT_SUB_DIR_NAME, 
												TEST_REPORT_PROCESS_NAME);
			Assert.AreEqual(expectedFileName, target.Filename);
			Assert.IsFalse(target.HasErrors);
			Assert.AreEqual(0,target.ErrorCount);
		}
		[TestMethod]
		public void ControlReportConstructorTestDefault()
		{
			// just implementing this for completeness.
			// the default constructor should never be used.
			var target = new ControlReportForTest();
			Assert.AreEqual(null, target.Filename);
		}

		/// <summary>
		///A test for ControlReport Constructor
		///</summary>
		[TestMethod]
		public void ControlReportConstructorTestFromFullPath()
		{
			StringBuilder fullpath = new StringBuilder();
			fullpath.Append( string.Format("{0}\\{1}",
									Directory.GetCurrentDirectory(),
									TEST_REPORT_SUB_DIR_NAME));

			var target = new ControlReportForTest(fullpath, TEST_REPORT_PROCESS_NAME);
			var expectedFileName = string.Format("{0}\\{1}\\_{2}_CNTRL.txt",
												Directory.GetCurrentDirectory(),
												TEST_REPORT_SUB_DIR_NAME,
												TEST_REPORT_PROCESS_NAME);
			Assert.AreEqual(expectedFileName, target.Filename);
			Assert.IsFalse(target.HasErrors);
			Assert.AreEqual(0, target.ErrorCount);
		}

		#region Inner Classes
		/// <summary>
		/// Transparent implementation of <see cref="ControlReport"/> for testing
		/// </summary>
		private class ControlReportForTest : ControlReport
		{
			public ControlReportForTest()
			: base(".", "UNITTEST")
			{

			}
			
			public ControlReportForTest
			(
				string fullPath,
				string processName
			)
			: base(fullPath, processName)
			{ 
			}

			public ControlReportForTest
			(
				StringBuilder fullPath,
				string processName
			)
			: base(fullPath, processName)
			{

			}
		}
		#endregion Inner Classes

		#region Private Helper Methods	

		// new up a ControlReport for testing
		private static ControlReportForTest CreateControlReportForTest()
		{
			return new ControlReportForTest(
				TEST_REPORT_SUB_DIR_NAME,
				TEST_REPORT_PROCESS_NAME);
		}

		private static bool StringIsInReportFile(
					string fileName, 
					string stringToFind)
		{
			if (!File.Exists(fileName))
				return false;

			// create reader & open file
			StreamReader reportReader = new StreamReader(fileName);

			string input;
			bool stringFound = false;
			while ((input = reportReader.ReadLine()) != null)
			{
				if (input.Contains(stringToFind))
				{
					stringFound = true;
					break;
				}
			}
			// close the stream
			reportReader.Close();

			return stringFound;
		}

		#endregion Private Helper Methods
	}
}