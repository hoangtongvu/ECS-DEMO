using NUnit.Framework;
using Unity.Collections;
using Core.Utilities.Extensions;
using Core.Testing.Assertions;

namespace Tests.EditMode.Utilities
{
    public partial class NativeListExtensionsTest
    {
        [Test]
        public void QuickRemoveAtTest()
        {
            var testList = new NativeList<TestElement>(10, Allocator.Persistent)
            {
                new(5),
                new(1),
                new(5),
                new(6),
                new(2),
                new(5)
            };

            for (int i = 0; i < testList.Length; i++)
            {
                if (testList[i].Value != 5) continue;
                testList.QuickRemoveAt(i);
                i--;
            }

            var expectedList = new NativeList<TestElement>(10, Allocator.Persistent)
            {
                new(2),
                new(1),
                new(6),
            };

            //this.PrintList(testList);
            //this.PrintList(expectedList);

            for (int i = 0; i < testList.Length; i++)
            {
                if (testList[i].Value == expectedList[i].Value) continue;
                Assert.True(false);
            }

            Assert.True(true);

        }

        [TestCase(10)]
        [TestCase(11)]
        public void ReverseListTest(int length)
        {
            // Arrange
            var toBeModifiedList = new NativeList<TestElement>(length, Allocator.Temp);
            var verifyList = new NativeList<TestElement>(length, Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                toBeModifiedList.Add(new(i));
            }

            for (int i = length - 1; i >= 0; i--)
            {
                verifyList.Add(toBeModifiedList[i]);
            }

            // Act
            toBeModifiedList.Reverse();

            // Assert
            NativeListAssert.AreEqual(verifyList, toBeModifiedList);

        }

        private void PrintList<T>(NativeList<T> list)
            where T : unmanaged
        {
            string log = "";

            for (int i = 0; i < list.Length; i++)
            {
                log += list[i] + ", ";
            }

            UnityEngine.Debug.Log(log);

        }

    }

}