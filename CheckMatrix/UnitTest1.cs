using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Win01;

namespace CheckMatrix
{
    [TestClass]
    public class UnitTest1
    {
        Motor m = new Motor(5,5);
        
        [TestMethod]
        public void TestMethod1()
        {
            m.victoryEvent += new Motor.victoryDel(test);
            m.run();
            m.fillAToTest(0);           
            bool b =m.checkA(0,0);
            Assert.AreEqual(false, b);
        }
        [TestMethod]
        public void TestMethod2()
        {
            m.victoryEvent += new Motor.victoryDel(test);
            m.run();
            m.fillAToTest(1);
            bool b = m.checkA(0, 0);
            Assert.AreEqual(true, b);
        }
        [TestMethod]
        public void TestMethod3()
        {
            m.victoryEvent += new Motor.victoryDel(test);
            m.run();
            m.fillAToTest(2);
            bool b = m.checkA(0, 0);
            Assert.AreEqual(true, b);
        }
        [TestMethod]
        public void TestMethod4()
        {
            m.victoryEvent += new Motor.victoryDel(test);
            m.run();
            m.fillAToTest(3);
            bool b = m.checkA(0, 0);
            Assert.AreEqual(true, b);
        }
        [TestMethod]
        public void TestMethod5()
        {
            m.victoryEvent += new Motor.victoryDel(test);
            m.run();
            m.fillAToTest(4);
            bool b = m.checkA(4, 0);
            Assert.AreEqual(true, b);
        }

        public void test(int x, int y, int des, Motor.CONNECT type) { }
    }
}
