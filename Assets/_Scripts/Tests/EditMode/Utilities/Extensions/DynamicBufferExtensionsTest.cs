using Core.Testing;
using NUnit.Framework;
using System;
using Unity.Entities;
using Utilities.Extensions;

namespace Tests.EditMode.Utilities.Extensions
{
    public struct TestBufferElement : IBufferElementData, IEquatable<TestBufferElement>
    {
        public int Value;

        public override bool Equals(object obj)
        {
            if (obj is TestBufferElement testBufferElement)
            {
                return Equals(testBufferElement);
            }

            return base.Equals(obj);
        }

        public static bool operator ==(TestBufferElement first, TestBufferElement second) => first.Equals(second);

        public static bool operator !=(TestBufferElement first, TestBufferElement second) => !(first == second);

        public bool Equals(TestBufferElement other) => this.Value.Equals(other.Value);

        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => $"{nameof(Value)}: {Value}";

    }

    public class DynamicBufferExtensionsTest : ECSTestsFixture
    {
        [TestCase(10)]
        [TestCase(11)]
        public void TryPop_Once(int length)
        {
            Entity entity = this.CreateEntity();
            var buffer = this.EntityManager.AddBuffer<TestBufferElement>(entity);

            for (int i = 0; i < length; i++)
            {
                buffer.Add(new()
                {
                    Value = i,
                });
            }

            buffer.TryPop(out var actualPopItem);
            TestBufferElement expectedPopItem = new()
            {
                Value = length - 1,
            };

            Assert.AreEqual(expectedPopItem, actualPopItem, "Pop wrong item");
            Assert.AreEqual(buffer.Length, length - 1, "Wrong buffer length");

        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(11)]
        public void TryPop_TillEmpty(int length)
        {
            Entity entity = this.CreateEntity();
            var buffer = this.EntityManager.AddBuffer<TestBufferElement>(entity);

            for (int i = 0; i < length; i++)
            {
                buffer.Add(new()
                {
                    Value = i,
                });
            }

            while (buffer.TryPop(out var actualPopItem))
            {
                TestBufferElement expectedPopItem = new()
                {
                    Value = buffer.Length,
                };

                Assert.AreEqual(expectedPopItem, actualPopItem, "Pop wrong item");
            }
            
        }

    }

}
