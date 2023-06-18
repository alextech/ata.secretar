using System;

namespace SharedKernel
{
    public class Range : ValueObject<Range>, ICloneable
    {
        public int Min { get; init; }
        public int Max { get; init; }

        public Range()
        {

        }

        public Range(int min, int max) =>
            (Min, Max) = (min, max);

        public static Range operator +(Range a, int b)
        {
            return new Range(a.Min + b, a.Max + b);
        }

        public static Range operator -(Range a, int b)
        {
            return new Range(a.Min - b, a.Max - b);
        }

        public object Clone()
        {
            return new Range(Min, Max);
        }

        public bool Intersects(Range compare)
        {
            return
                compare.Max >= Min && compare.Max <= Max ||
                compare.Min >= Min && compare.Min <= Max;
        }

        public int LowestIntersectionWith(Range range)
        {
            return Math.Max(range.Min, Min);
        }
    }
}
