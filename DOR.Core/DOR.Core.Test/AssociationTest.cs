//using DOR.Core.Collections;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;

//namespace DOR.Core.Test
//{
    
    
//    /// <summary>
//    ///This is a test class for AssociationTest and is intended
//    ///to contain all AssociationTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class AssociationTest
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
//        ///A test for Association`2 Constructor
//        ///</summary>
//        public void AssociationConstructorTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b);
//            Assert.Inconclusive("TODO: Implement code to verify target");
//        }

//        [TestMethod()]
//        public void AssociationConstructorTest()
//        {
//            AssociationConstructorTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Clone
//        ///</summary>
//        public void CloneTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            Association<A, B> expected = null; // TODO: Initialize to an appropriate value
//            Association<A, B> actual;
//            actual = target.Clone();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void CloneTest()
//        {
//            CloneTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Equals
//        ///</summary>
//        public void EqualsTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            object obj = null; // TODO: Initialize to an appropriate value
//            bool expected = false; // TODO: Initialize to an appropriate value
//            bool actual;
//            actual = target.Equals(obj);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void EqualsTest()
//        {
//            EqualsTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for GetHashCode
//        ///</summary>
//        public void GetHashCodeTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            int expected = 0; // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.GetHashCode();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void GetHashCodeTest()
//        {
//            GetHashCodeTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for ToXml
//        ///</summary>
//        public void ToXmlTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            string expected = string.Empty; // TODO: Initialize to an appropriate value
//            string actual;
//            actual = target.ToXml();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ToXmlTest()
//        {
//            ToXmlTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for op_Equality
//        ///</summary>
//        public void op_EqualityTestHelper<A, B>()
//        {
//            Association<A, B> a1 = null; // TODO: Initialize to an appropriate value
//            Association<A, B> a2 = null; // TODO: Initialize to an appropriate value
//            bool expected = false; // TODO: Initialize to an appropriate value
//            bool actual;
//            actual = (a1 == a2);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void op_EqualityTest()
//        {
//            op_EqualityTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for op_Inequality
//        ///</summary>
//        public void op_InequalityTestHelper<A, B>()
//        {
//            Association<A, B> a1 = null; // TODO: Initialize to an appropriate value
//            Association<A, B> a2 = null; // TODO: Initialize to an appropriate value
//            bool expected = false; // TODO: Initialize to an appropriate value
//            bool actual;
//            actual = (a1 != a2);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void op_InequalityTest()
//        {
//            op_InequalityTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Key
//        ///</summary>
//        public void KeyTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            A expected = default(A); // TODO: Initialize to an appropriate value
//            A actual;
//            target.Key = expected;
//            actual = target.Key;
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void KeyTest()
//        {
//            KeyTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Left
//        ///</summary>
//        public void LeftTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            A actual;
//            actual = target.Left;
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void LeftTest()
//        {
//            LeftTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Right
//        ///</summary>
//        public void RightTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            B actual;
//            actual = target.Right;
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void RightTest()
//        {
//            RightTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }

//        /// <summary>
//        ///A test for Value
//        ///</summary>
//        public void ValueTestHelper<A, B>()
//        {
//            A a = default(A); // TODO: Initialize to an appropriate value
//            B b = default(B); // TODO: Initialize to an appropriate value
//            Association<A, B> target = new Association<A, B>(a, b); // TODO: Initialize to an appropriate value
//            B expected = default(B); // TODO: Initialize to an appropriate value
//            B actual;
//            target.Value = expected;
//            actual = target.Value;
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ValueTest()
//        {
//            ValueTestHelper<GenericParameterHelper, GenericParameterHelper>();
//        }
//    }
//}
