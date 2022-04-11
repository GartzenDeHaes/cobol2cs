//using DOR.Core.Collections;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace DOR.Core.Test
//{


//     <summary>
//    This is a test class for VectorTest and is intended
//    to contain all VectorTest Unit Tests
//    </summary>
//    [TestClass()]
//    public class VectorTest
//    {


//        private TestContext testContextInstance;

//         <summary>
//        Gets or sets the test context which provides
//        information about and functionality for the current test run.
//        </summary>
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
		 
//        You can use the following additional attributes as you write your tests:
		
//        Use ClassInitialize to run code before running the first test in the class
//        [ClassInitialize()]
//        public static void MyClassInitialize(TestContext testContext)
//        {
//        }
		
//        Use ClassCleanup to run code after all tests in a class have run
//        [ClassCleanup()]
//        public static void MyClassCleanup()
//        {
//        }
		
//        Use TestInitialize to run code before running each test
//        [TestInitialize()]
//        public void MyTestInitialize()
//        {
//        }
		
//        Use TestCleanup to run code after each test has run
//        [TestCleanup()]
//        public void MyTestCleanup()
//        {
//        }
		
//        #endregion


//         <summary>
//        A test for Vector`1 Constructor
//        </summary>
//        public void VectorConstructorTestHelper<T>()
//        {
//            int size = 4; 
//            Vector<T> target = new Vector<T>(size);

//        }

//        [TestMethod()]
//        public void VectorConstructorTest()
//        {
//            VectorConstructorTestHelper<string>();
//        }

//         <summary>
//        A test for Add
//        </summary>
//        public void AddTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T[] ta = null; // TODO: Initialize to an appropriate value
//            target.Add(ta);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void AddTest()
//        {
//            AddTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Add
//        </summary>
//        public void AddTest1Helper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            target.Add(o);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void AddTest1()
//        {
//            AddTest1Helper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for AddElement
//        </summary>
//        public void AddElementTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            int expected = 0; // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.AddElement(o);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void AddElementTest()
//        {
//            AddElementTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for AddRange
//        </summary>
//        public void AddRangeTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            Vector<T> v = null; // TODO: Initialize to an appropriate value
//            target.AddRange(v);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void AddRangeTest()
//        {
//            AddRangeTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for AddRange
//        </summary>
//        public void AddRangeTest1Helper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T[] v = null; // TODO: Initialize to an appropriate value
//            target.AddRange(v);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void AddRangeTest1()
//        {
//            AddRangeTest1Helper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for AddRangeWithPad
//        </summary>
//        public void AddRangeWithPadTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T[] a = null; // TODO: Initialize to an appropriate value
//            int count = 0; // TODO: Initialize to an appropriate value
//            target.AddRangeWithPad(a, count);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void AddRangeWithPadTest()
//        {
//            AddRangeWithPadTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Capacity
//        </summary>
//        public void CapacityTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int expected = 0; // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.Capacity();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void CapacityTest()
//        {
//            CapacityTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Clear
//        </summary>
//        public void ClearTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            target.Clear();
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void ClearTest()
//        {
//            ClearTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Copy
//        </summary>
//        public void CopyTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            Vector<T> v = null; // TODO: Initialize to an appropriate value
//            target.Copy(v);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void CopyTest()
//        {
//            CopyTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Data
//        </summary>
//        public void DataTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T[] expected = null; // TODO: Initialize to an appropriate value
//            T[] actual;
//            actual = target.Data();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void DataTest()
//        {
//            DataTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Dispose
//        </summary>
//        public void DisposeTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            target.Dispose();
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void DisposeTest()
//        {
//            DisposeTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for ElementAt
//        </summary>
//        public void ElementAtTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int at = 0; // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.ElementAt(at);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ElementAtTest()
//        {
//            ElementAtTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Extend
//        </summary>
//        public void ExtendTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            target.Extend();
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void ExtendTest()
//        {
//            ExtendTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for FirstElement
//        </summary>
//        public void FirstElementTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.FirstElement();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void FirstElementTest()
//        {
//            FirstElementTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for GetEnumerator
//        </summary>
//        public void GetEnumeratorTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            VectorEnum<T> expected = null; // TODO: Initialize to an appropriate value
//            VectorEnum<T> actual;
//            actual = target.GetEnumerator();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void GetEnumeratorTest()
//        {
//            GetEnumeratorTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Head
//        </summary>
//        public void HeadTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.Head();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void HeadTest()
//        {
//            HeadTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for IndexOf
//        </summary>
//        public void IndexOfTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            int expected = 0; // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.IndexOf(o);
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void IndexOfTest()
//        {
//            IndexOfTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for InsertBefore
//        </summary>
//        public void InsertBeforeTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T oNew = default(T); // TODO: Initialize to an appropriate value
//            T oExisting = default(T); // TODO: Initialize to an appropriate value
//            target.InsertBefore(oNew, oExisting);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void InsertBeforeTest()
//        {
//            InsertBeforeTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for InsertElementAt
//        </summary>
//        public void InsertElementAtTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            int at = 0; // TODO: Initialize to an appropriate value
//            target.InsertElementAt(o, at);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void InsertElementAtTest()
//        {
//            InsertElementAtTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for IsEmpty
//        </summary>
//        public void IsEmptyTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            bool expected = false; // TODO: Initialize to an appropriate value
//            bool actual;
//            actual = target.IsEmpty();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void IsEmptyTest()
//        {
//            IsEmptyTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for LastElement
//        </summary>
//        public void LastElementTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.LastElement();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void LastElementTest()
//        {
//            LastElementTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Peek
//        </summary>
//        public void PeekTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.Peek();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void PeekTest()
//        {
//            PeekTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Pop
//        </summary>
//        public void PopTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.Pop();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void PopTest()
//        {
//            PopTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for RemoveElement
//        </summary>
//        public void RemoveElementTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            target.RemoveElement();
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void RemoveElementTest()
//        {
//            RemoveElementTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for RemoveElement
//        </summary>
//        public void RemoveElementTest1Helper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            target.RemoveElement(o);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void RemoveElementTest1()
//        {
//            RemoveElementTest1Helper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for RemoveElementAt
//        </summary>
//        public void RemoveElementAtTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int at = 0; // TODO: Initialize to an appropriate value
//            target.RemoveElementAt(at);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void RemoveElementAtTest()
//        {
//            RemoveElementAtTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for SetElementAt
//        </summary>
//        public void SetElementAtTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int at = 0; // TODO: Initialize to an appropriate value
//            T o = default(T); // TODO: Initialize to an appropriate value
//            target.SetElementAt(at, o);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void SetElementAtTest()
//        {
//            SetElementAtTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for SetSize
//        </summary>
//        public void SetSizeTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int s = 0; // TODO: Initialize to an appropriate value
//            target.SetSize(s);
//            Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        }

//        [TestMethod()]
//        public void SetSizeTest()
//        {
//            SetSizeTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for System.Collections.IEnumerable.GetEnumerator
//        </summary>
//        public void GetEnumeratorTest1Helper<T>()
//        {
//            IEnumerable target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            IEnumerator expected = null; // TODO: Initialize to an appropriate value
//            IEnumerator actual;
//            actual = target.GetEnumerator();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        [DeploymentItem("DOR.Core.dll")]
//        public void GetEnumeratorTest1()
//        {
//            GetEnumeratorTest1Helper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Tail
//        </summary>
//        public void TailTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            actual = target.Tail();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void TailTest()
//        {
//            TailTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for ToArray
//        </summary>
//        public void ToArrayTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            T[] expected = null; // TODO: Initialize to an appropriate value
//            T[] actual;
//            actual = target.ToArray();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ToArrayTest()
//        {
//            ToArrayTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for ToList
//        </summary>
//        public void ToListTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            List<T> expected = null; // TODO: Initialize to an appropriate value
//            List<T> actual;
//            actual = target.ToList();
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ToListTest()
//        {
//            ToListTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Count
//        </summary>
//        public void CountTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.Count;
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void CountTest()
//        {
//            CountTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Item
//        </summary>
//        public void ItemTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int idx = 0; // TODO: Initialize to an appropriate value
//            T expected = default(T); // TODO: Initialize to an appropriate value
//            T actual;
//            target[idx] = expected;
//            actual = target[idx];
//            Assert.AreEqual(expected, actual);
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void ItemTest()
//        {
//            ItemTestHelper<GenericParameterHelper>();
//        }

//         <summary>
//        A test for Size
//        </summary>
//        public void SizeTestHelper<T>()
//        {
//            Vector<T> target = new Vector<T>(); // TODO: Initialize to an appropriate value
//            int actual;
//            actual = target.Size;
//            Assert.Inconclusive("Verify the correctness of this test method.");
//        }

//        [TestMethod()]
//        public void SizeTest()
//        {
//            SizeTestHelper<GenericParameterHelper>();
//        }
//    }
//}
