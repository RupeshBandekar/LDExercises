namespace ProbabilityExercise.Tests
{
    using System;
    using Xunit;

    public class ProbabilityTests
    {
        [Theory]
        [InlineData(1.5)]
        [InlineData(-0.5)]
        public void RaiseRangeExceptionProbability(decimal decProbA)
        {
            Assert.Throws<ArgumentException>(() => new Probability(decProbA));
        }

        [Theory]
        [InlineData(0.1,0.2, 0.02)]
        public void AndProbability(decimal decProbA, decimal decProbB, decimal expectedResult)
        {
            var p1 = new Probability(decProbA);
            var p2 = new Probability(decProbB);
            var oactResult = p1.And(p2);
            var oexpResult = new Probability(expectedResult);
            Assert.True(oactResult.Equals(oexpResult));
        }

        [Theory]
        [InlineData(0.1, 0.2, 0.28)]
        public void OrProbability(decimal decProbA, decimal decProbB, decimal expectedResult)
        {
            var p1 = new Probability(decProbA);
            var p2 = new Probability(decProbB);
            var oactResult = p1.Or(p2);
            var oexpResult = new Probability(expectedResult);
            Assert.True(oactResult.Equals(oexpResult));
        }
        
        [Theory]
        [InlineData(0.5, 0.5)]
        public void InverseProbability(decimal decProbA, decimal expectedResult)
        {
            var p1 = new Probability(decProbA);
            Assert.Equal(expectedResult, p1.Inverse());
        }

        [Theory]
        [InlineData(0.1, 0.1)]
        public void CheckEqualsOperator(decimal decProbA, decimal decProbB)
        {
            var p1 = new Probability(decProbA);
            var p2 = new Probability(decProbB);
            Assert.True(p1 == p2);
        }

        [Theory]
        [InlineData(0.1, 0.2)]
        public void CheckNotEqualsOperator(decimal decProbA, decimal decProbB)
        {
            var p1 = new Probability(decProbA);
            var p2 = new Probability(decProbB);
            Assert.True(p1 != p2);
        }

         [Theory]
         [InlineData(0.1,0.1,true)]
         [InlineData(0.1, null, false)]
        public void CheckNullObjectEquals(decimal decProbA, decimal decProbB, bool expectedResult)
        {
            var p1 = new Probability(decProbA);
            var p2 = new Probability(decProbB);
            Assert.Equal(expectedResult, p1.Equals(p2));
        }

        [Theory]
        [InlineData(0.1,false)]
        public void CheckProbabilityObjectEquals(decimal decProbA, bool expectedResult)
        {
            var p1 = new Probability(decProbA);
            object p2 = null;
            
            Assert.Equal(expectedResult, p1.Equals(p2));
        }

        [Theory]
        [InlineData(0.1)]
        public void CheckHashCode(decimal decProbA)
        {
            var p1 = new Probability(decProbA);
            Assert.Equal(decProbA.GetHashCode(), p1.GetHashCode());
        }
    }
}
