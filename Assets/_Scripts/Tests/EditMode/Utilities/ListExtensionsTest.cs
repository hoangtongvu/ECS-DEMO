using NUnit.Framework;
using Core.Utilities.Extensions;
using System.Collections.Generic;

namespace Tests.EditMode.Utilities
{
    public partial class ListExtensionsTest
    {

        [Test]
        public void QuickRemoveAtTest()
        {
            var testList = new List<TestElement>()
            {
                new(5),
                new(1),
                new(5),
                new(6),
                new(2),
                new(5)
            };

            for (int i = 0; i < testList.Count; i++)
            {
                if (testList[i].Value != 5) continue;
                testList.QuickRemoveAt(i);
                i--;
            }

            var expectedList = new List<TestElement>()
            {
                new(2),
                new(1),
                new(6),
            };

            //this.PrintList(testList);
            //this.PrintList(expectedList);

            for (int i = 0; i < testList.Count; i++)
            {
                if (testList[i].Value == expectedList[i].Value) continue;
                Assert.True(false);
            }

            Assert.True(true);

        }

        private void PrintList<T>(List<T> list)
            where T : unmanaged
        {
            string log = "";

            for (int i = 0; i < list.Count; i++)
            {
                log += list[i] + ", ";
            }

            UnityEngine.Debug.Log(log);

        }

    }
}