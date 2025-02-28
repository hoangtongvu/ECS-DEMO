using NUnit.Framework;
using Unity.Collections;
using Core.Misc.WorldMap.PathFinding;
using System.Collections.Generic;

namespace Tests.EditMode.Core.Misc.WorldMap.PathFinding
{
    public partial class NativePriorityQueueTests
    {
        private NativePriorityQueue<Node> queue;

        [SetUp]
        public void SetUp()
        {
            this.queue = new(10, Allocator.Temp);
        }

        [TearDown]
        public void TearDown()
        {
            this.queue.Dispose();
        }

        [Test]
        public void Add_ShouldInsertElementsInOrder()
        {
            Node node1 = new() { NodeIndex = 1, FCost = 3f };
            Node node2 = new() { NodeIndex = 2, FCost = 1f };
            Node node3 = new() { NodeIndex = 3, FCost = 2f };

            this.queue.Add(node1);
            this.queue.Add(node2);
            this.queue.Add(node3);

            Assert.AreEqual(node2, this.queue.Pop()); // Lowest cost first
            Assert.AreEqual(node3, this.queue.Pop());
            Assert.AreEqual(node1, this.queue.Pop());
        }

        [Test]
        public void Pop_ShouldRemoveElementsInOrder()
        {
            Node node1 = new() { NodeIndex = 1, FCost = 2f };
            Node node2 = new() { NodeIndex = 2, FCost = 1f };

            this.queue.Add(node1);
            this.queue.Add(node2);

            Assert.AreEqual(node2, this.queue.Pop()); // Lowest cost first
            Assert.AreEqual(node1, this.queue.Pop());
        }

        [Test]
        public void Contains_ShouldReturnTrueIfItemExists()
        {
            Node node = new() { NodeIndex = 1, FCost = 2f };
            this.queue.Add(node);

            bool contains = this.queue.Contains(node, out int index);

            Assert.IsTrue(contains);
            Assert.AreEqual(0, index);
        }

        [Test]
        public void Contains_ShouldReturnFalseIfItemDoesNotExist()
        {
            Node node = new() { NodeIndex = 1, FCost = 2f };

            bool contains = this.queue.Contains(node, out int index);

            Assert.IsFalse(contains);
            Assert.AreEqual(-1, index);
        }

        [Test]
        public void RandomizedTest_ShouldMaintainPriorityOrder()
        {
            const int numberOfNodes = 100;
            System.Random random = new(42); // Fixed seed for reproducibility
            NativeList<Node> expectedNodes = new(numberOfNodes, Allocator.Temp);

            // Create a list of random nodes
            for (int i = 0; i < numberOfNodes; i++)
            {
                Node node = new()
                {
                    NodeIndex = i,
                    FCost = (float)random.NextDouble() * 100
                };
                expectedNodes.Add(node);
                this.queue.Add(node);
            }

            // Sort expected nodes based on the cost (highest cost first)
            expectedNodes.Sort(Comparer<Node>.Create((a, b) => a.CompareTo(b)));

            // Now, pop nodes from the queue and verify the order
            foreach (var expected in expectedNodes)
            {
                Node actual = this.queue.Pop();
                Assert.AreEqual(expected.FCost, actual.FCost, 0.0001f, "Popped node cost does not match expected order.");
            }

            expectedNodes.Dispose();

        }


    }

}