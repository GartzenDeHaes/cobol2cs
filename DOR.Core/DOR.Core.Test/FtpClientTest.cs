//using DOR.Core.Ftp;
//using System.IO;

//#if INTEGRATION
//using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;
//using Assert = NUnit.Framework.Assert;
//using TestMethod = NUnit.Framework.TestAttribute;
//using TestClass = NUnit.Framework.TestFixtureAttribute;
//using TestInitialize = NUnit.Framework.SetUpAttribute;
//using TestCleanup = NUnit.Framework.TearDownAttribute;
//using TestContext = System.Object;
//#else
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//#endif

//namespace DOR.Core.Test
//{
    
    
//    /// <summary>
//    ///This is a test class for FtpClientTest and is intended
//    ///to contain all FtpClientTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class FtpClientTest
//    {


//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //[TestInitialize()]
//        //public void MyTestInitialize()
//        //{
//        //}
//        //
//        //Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion


//        /// <summary>
//        ///A test for FtpClient Constructor
//        ///</summary>
//        [TestMethod()]
//        public void FtpClientConstructorTest()
//        {
//            string server = "ftp.mozilla.org";
//            string username = "anonymous";
//            string password = "cms@dor.wa.gov";
//            FtpClient target = new FtpClient(server, username, password);
			
//            string[] files = target.GetFileList();
//            Assert.AreEqual(4, files.Length);
//            Assert.AreEqual("README", files[0]);

//            if (File.Exists("README"))
//            {
//                File.Delete("README");
//            }
//            target.Download("README");
//            Assert.IsTrue(File.Exists("README"));
//            File.Delete("README");

//            target.Close();
//        }
//    }
//}
