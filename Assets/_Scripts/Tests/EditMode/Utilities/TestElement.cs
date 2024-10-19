

namespace Tests.EditMode.Utilities
{
    public struct TestElement
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

    }


}