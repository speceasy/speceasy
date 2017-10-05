using System;
using System.Collections.Generic;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public sealed class Integers : IEquatable<Integers>, IComparable<Integers>, IComparable
    {
        public static readonly Integers Min   = new Integers(int.MinValue);
        public static readonly Integers One   = new Integers(1);
        public static readonly Integers Two   = new Integers(2);
        public static readonly Integers Three = new Integers(3);
        public static readonly Integers Max   = new Integers(int.MaxValue);

        private static readonly Dictionary<Integers, Integers> DecrementDictionary = new Dictionary<Integers, Integers>
        {
            [Max]   = Three,
            [Three] = Two,
            [Two]   = One,
            [One]   = Min,
            [Min]   = Min
        };

        private static readonly Dictionary<Integers, Integers> IncrementDictionary = new Dictionary<Integers, Integers>
        {
            [Min]   = One,
            [One]   = Two,
            [Two]   = Three,
            [Three] = Max,
            [Max]   = Max
        };

        private readonly int rawValue;

        private Integers(int rawValue)
        {
            this.rawValue = rawValue;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Integers);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 17 * 23 + rawValue.GetHashCode();
            }
        }

        public bool Equals(Integers other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return rawValue == other?.rawValue;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Integers);
        }

        public int CompareTo(Integers other)
        {
            return rawValue - other?.rawValue ?? 1;
        }

        public override string ToString()
        {
            if (Equals(Min))
            {
                return "int.MinValue";
            }

            if (Equals(Max))
            {
                return "int.MaxValue";
            }

            return $"{rawValue}";
        }

        public static bool operator ==(Integers first, Integers second)
        {
            if ((object)first == null || (object)second == null)
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(Integers left, Integers right)
        {
            return !(left == right);
        }

        public static bool operator >(Integers left, Integers right)
        {
            return left.rawValue > right.rawValue;
        }

        public static bool operator <(Integers left, Integers right)
        {
            return left.rawValue < right.rawValue;
        }

        public static Integers operator --(Integers integersSut)
        {
            return DecrementDictionary[integersSut];
        }

        public static Integers operator ++(Integers integersSut)
        {
            return IncrementDictionary[integersSut];
        }

        public static explicit operator double(Integers integersSut)
        {
            return integersSut.rawValue;
        }
    }
}
