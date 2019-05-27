namespace ProbabilityExercise
{
    using System;
    public struct Probability : IEquatable<Probability>
    {
        private readonly decimal _decNum;

        public Probability(decimal decNum)
        {
            if (!(decNum >= 0 && decNum <= 1))
                throw new ArgumentException("Probability should be in between 0 and 1");
            _decNum = decNum;
        }

        public Probability And(Probability other)
        {
            return new Probability(_decNum * other._decNum);
        }

        public Probability Or(Probability other)
        {
            return new Probability(_decNum + other._decNum - (_decNum * other._decNum));
        }

        public decimal Inverse()
        {
            return 1 - _decNum;
        }

        public bool Equals(Probability other)
        {
            return _decNum == other._decNum;
        }

        public static bool operator ==(Probability left, Probability right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Probability left, Probability right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Probability other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _decNum.GetHashCode();
        }
    }

}
