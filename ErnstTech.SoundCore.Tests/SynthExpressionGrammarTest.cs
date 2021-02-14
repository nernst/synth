using System;
using ErnstTech.SoundCore.Synthesis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErnstTech.SoundCore.Tests
{
    [TestClass]
    public class SynthExpressionGrammarTest
    {
        const double Epsilon = 1e-6;

        static bool IsApproximatelyEqual(double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs) <= Epsilon;

        }

        readonly ExpressionParser _Parser = new ExpressionParser();

        [TestMethod]
        public void TestConstant()
        {
            string expression = "2";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0), 2));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.1), 2));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(2), 2));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(3.1), 2));
        }

        [TestMethod]
        public void TestPi()
        {
            var func = _Parser.Parse("pi");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.PI, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.PI, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.PI, func(-1)));
        }

        [TestMethod]
        public void TestEuler()
        {
            var func = _Parser.Parse("e");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.E, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.E, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(Math.E, func(-1)));
        }

        [TestMethod]
        public void TestSimpleAdd()
        {
            var func = _Parser.Parse("2+3");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(5, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(5, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(5, func(-1)));
        }

        [TestMethod]
        public void TestSimpleSub()
        {
            var func = _Parser.Parse("2-3");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(-1, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(-1, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(-1, func(-1)));
        }

        [TestMethod]
        public void TestSimpleMul()
        {
            var func = _Parser.Parse("2*3");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(6, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(6, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(6, func(-1)));
        }

        [TestMethod]
        public void TestSimpleDiv()
        {
            var func = _Parser.Parse("2/3");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(2.0 / 3, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(2.0 / 3, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(2.0 / 3, func(-1)));
        }

        [TestMethod]
        public void TestTimeVar()
        {
            var func = _Parser.Parse("t");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(1, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(2, func(2)));
            Assert.IsFalse(SynthExpressionGrammarTest.IsApproximatelyEqual(1, func(2)));
        }

        [TestMethod]
        public void TestAddMul()
        {
            var func = _Parser.Parse("2+3*4");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(14, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(14, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(14, func(-1)));
        }

        [TestMethod]
        public void TestSubDiv()
        {
            var func = _Parser.Parse("2-3/4");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(1.25, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(1.25, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(1.25, func(-1)));
        }

        [TestMethod]
        public void TestTimeExpr()
        {
            var func = _Parser.Parse("2*t");

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(0, func(0)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(2, func(1)));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(-2, func(-1)));
        }

        [TestMethod]
        public void TestComplexExpression()
        {
            string expression = "2*cos(2*pi*t)";
            var func = _Parser.Parse(expression);

            Assert.IsNotNull(func);
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0), 2));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.1), 1.618033988749895));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(2), 2));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(3.1), 1.6180339887498958));
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
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0), 0));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.01), 1));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.02), 0.98));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.25), 0.52));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.26), 0.5));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.27), 0.5));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.50), 0.5));
            Assert.IsTrue(SynthExpressionGrammarTest.IsApproximatelyEqual(func(0.75), 0.5));

        }
    }
}
