using System;

namespace Tests.EditMode.Utilities
{
    public struct TestElement : IEquatable<TestElement>
    {
        public int Value;

        public TestElement(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public override bool Equals(object obj)
        {
            if (obj is TestElement testElement)
            {
                return Equals(testElement);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(TestElement first, TestElement second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(TestElement first, TestElement second)
        {
            return !(first == second);
        }

        public bool Equals(TestElement other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }

}