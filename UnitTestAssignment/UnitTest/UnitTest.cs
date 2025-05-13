public class DivisorCountSolverTests
{
    [TestFixture]
    public class CountMatchingDivisorPairsTests
    {
        [Test]
        public void CountMatchingDivisorPairs_NegativeInput_ThrowsException()
        {
            Assert.Throws<Exception>(() => DivisorCountSolver.CountMatchingDivisorPairs(-1));
        }

        [Test]
        public void CountMatchingDivisorPairs_ZeroInput_ReturnsZero()
        {
            Assert.AreEqual(0, DivisorCountSolver.CountMatchingDivisorPairs(0));
        }

        [Test]
        public void CountMatchingDivisorPairs_OneInput_ReturnsZero()
        {
            Assert.AreEqual(0, DivisorCountSolver.CountMatchingDivisorPairs(1));
        }

        [Test]
        public void CountMatchingDivisorPairs_TwoInput_ReturnsZero()
        {
            Assert.AreEqual(0, DivisorCountSolver.CountMatchingDivisorPairs(2));
        }

        [Test]
        public void CountMatchingDivisorPairs_ThreeInput_ReturnsOne()
        {
            Assert.AreEqual(1, DivisorCountSolver.CountMatchingDivisorPairs(3));
        }

        [Test]
        public void CountMatchingDivisorPairs_FourInput_ReturnsOne()
        {
            Assert.AreEqual(1, DivisorCountSolver.CountMatchingDivisorPairs(4));
        }

        [Test]
        public void CountMatchingDivisorPairs_SixInput_ReturnsOne()
        {
            Assert.AreEqual(1, DivisorCountSolver.CountMatchingDivisorPairs(6));
        }

        [Test]
        public void CountMatchingDivisorPairs_LargeInput_ReturnsTwo()
        {
            Assert.AreEqual(2, DivisorCountSolver.CountMatchingDivisorPairs(20));
        }

        [Test]
        public void CountMatchingDivisorPairs_LargeInput_NoException()
        {
            Assert.DoesNotThrow(() => DivisorCountSolver.CountMatchingDivisorPairs(100));
        }
    }
}
