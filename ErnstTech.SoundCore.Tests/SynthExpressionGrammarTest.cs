using System;
using ErnstTech.SoundCore.Synthesis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErnstTech.SoundCore.Tests
{
    [TestClass]
    public class SynthExpressionGrammarTest
    {
        const double Epsilon = 1e-6;

        readonly ExpressionParser _Parser = new ExpressionParser();

        [TestMethod]
        public void TestConstant()
        {
            string expression = "2";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.AreEqual(2, func(0), Epsilon);
            Assert.AreEqual(2, func(0.1), Epsilon);
            Assert.AreEqual(2, func(2), Epsilon);
            Assert.AreEqual(2, func(3.1), Epsilon);
        }

        [TestMethod]
        public void TestPi()
        {
            var func = _Parser.Parse("pi");

            Assert.IsNotNull(func);
            Assert.AreEqual(Math.PI, func(0), Epsilon);
            Assert.AreEqual(Math.PI, func(1), Epsilon);
            Assert.AreEqual(Math.PI, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestEuler()
        {
            var func = _Parser.Parse("e");

            Assert.IsNotNull(func);
            Assert.AreEqual(Math.E, func(0), Epsilon);
            Assert.AreEqual(Math.E, func(1), Epsilon);
            Assert.AreEqual(Math.E, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestSimpleAdd()
        {
            var func = _Parser.Parse("2+3");

            Assert.IsNotNull(func);
            Assert.AreEqual(5, func(0), Epsilon);
            Assert.AreEqual(5, func(1), Epsilon);
            Assert.AreEqual(5, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestSimpleSub()
        {
            var func = _Parser.Parse("2-3");

            Assert.IsNotNull(func);
            Assert.AreEqual(-1, func(0), Epsilon);
            Assert.AreEqual(-1, func(1), Epsilon);
            Assert.AreEqual(-1, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestSimpleMul()
        {
            var func = _Parser.Parse("2*3");

            Assert.IsNotNull(func);
            Assert.AreEqual(6, func(0), Epsilon);
            Assert.AreEqual(6, func(1), Epsilon);
            Assert.AreEqual(6, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestSimpleDiv()
        {
            var func = _Parser.Parse("2/3");

            Assert.IsNotNull(func);
            Assert.AreEqual(2.0 / 3, func(0), Epsilon);
            Assert.AreEqual(2.0 / 3, func(1), Epsilon);
            Assert.AreEqual(2.0 / 3, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestTimeVar()
        {
            var func = _Parser.Parse("t");

            Assert.IsNotNull(func);
            Assert.AreEqual(1, func(1), Epsilon);
            Assert.AreEqual(2, func(2), Epsilon);
            Assert.AreNotEqual(1, func(2), Epsilon);
        }

        [TestMethod]
        public void TestAddMul()
        {
            var func = _Parser.Parse("2+3*4");

            Assert.IsNotNull(func);
            Assert.AreEqual(14, func(0), Epsilon);
            Assert.AreEqual(14, func(1), Epsilon);
            Assert.AreEqual(14, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestSubDiv()
        {
            var func = _Parser.Parse("2-3/4");

            Assert.IsNotNull(func);
            Assert.AreEqual(1.25, func(0), Epsilon);
            Assert.AreEqual(1.25, func(1), Epsilon);
            Assert.AreEqual(1.25, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestTimeExpr()
        {
            var func = _Parser.Parse("2*t");

            Assert.IsNotNull(func);
            Assert.AreEqual(0, func(0), Epsilon);
            Assert.AreEqual(2, func(1), Epsilon);
            Assert.AreEqual(-2, func(-1), Epsilon);
        }

        [TestMethod]
        public void TestComplexExpression()
        {
            string expression = "2*cos(2*pi*t)";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.AreEqual(2, func(0), Epsilon);
            Assert.AreEqual(1.618033988749895, func(0.1), Epsilon);
            Assert.AreEqual(2, func(2), Epsilon);
            Assert.AreEqual(1.6180339887498958, func(3.1), Epsilon);
        }

        [TestMethod]
        public void TestSubExpression()
        {
            string expression = "cos(2 * PI * (220 + 4 * cos(2 * PI * 10 * t)) * t) * 0.5";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
        }

        [TestMethod]
        public void TestADSREnvelope()
        {
            string expression = "adsr(1)";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.AreEqual(0, func(0), Epsilon);
            Assert.AreEqual(1, func(0.01), Epsilon);
            Assert.AreEqual(0.95, func(0.02), Epsilon);
            Assert.AreEqual(0.5, func(0.25), Epsilon);
            Assert.AreEqual(0.5, func(0.26), Epsilon);
            Assert.AreEqual(0.5, func(0.27), Epsilon);
            Assert.AreEqual(0, func(0.50), Epsilon);
            Assert.AreEqual(0, func(0.75), Epsilon);

        }

        [TestMethod]
        public void TestSawWave()
        {
            string expression = "saw(t)";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.AreEqual(-1.0, func(0.0), Epsilon);
            Assert.AreEqual(1.0, func(0.999_999_999), Epsilon);
            Assert.AreEqual(0.0, func(0.5), Epsilon);
            Assert.AreEqual(-1.0, func(1.0), Epsilon);
            Assert.AreEqual(1.0, func(1.999_999_999), Epsilon);
            Assert.AreEqual(0.0, func(1.5), Epsilon);
        }

        [TestMethod]
        public void TestTriWave()
        {
            string expression = "tri(t)";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.AreEqual(-1.0, func(0.0), Epsilon);
            Assert.AreEqual(1.0, func(0.50), Epsilon);
            Assert.AreEqual(0.0, func(0.25), Epsilon);
            Assert.AreEqual(0.0, func(0.75), Epsilon);
            Assert.AreEqual(-1.0, func(1.0), Epsilon);
            Assert.AreEqual(1.0, func(1.50), Epsilon);
            Assert.AreEqual(0.0, func(1.25), Epsilon);
            Assert.AreEqual(0.0, func(1.75), Epsilon);
        }
    }
}
