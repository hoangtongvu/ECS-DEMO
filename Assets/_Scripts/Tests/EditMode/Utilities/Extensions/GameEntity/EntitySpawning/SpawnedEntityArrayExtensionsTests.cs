using Components.GameEntity.EntitySpawning;
using Core.Testing;
using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions.GameEntity.EntitySpawning;

namespace Tests.EditMode.Utilities.Extensions.GameEntity.EntitySpawning
{
    public class SpawnedEntityArrayExtensionsTests : ECSTestsFixture
    {
        private const int spawnedEntityArrayLength = 5;
        private SpawnedEntityArray spawnedEntityArray;

        public override void SetUp()
        {
            base.SetUp();

            this.spawnedEntityArray = new()
            {
                Value = new(spawnedEntityArrayLength, Allocator.Persistent),
            };

        }

        public override void TearDown()
        {
            base.TearDown();
            this.spawnedEntityArray.Value.Dispose();
        }

        [Test]
        public void Clear_ZigzagElements_AllCleared()
        {
            // Arrange
            var tempArray = new NativeArray<Entity>(spawnedEntityArrayLength, Allocator.Temp);

            for (int i = 0; (i % 2 == 0) && (i < spawnedEntityArrayLength); i++)
            {
                var newEntity = this.EntityManager.CreateEntity();
                tempArray[i] = newEntity;
                this.spawnedEntityArray.Value[i] = newEntity;
            }

            //Act
            this.spawnedEntityArray.Clear();

            // Assert
            for (int i = 0; i < spawnedEntityArrayLength; i++)
            {
                Assert.AreEqual(Entity.Null, this.spawnedEntityArray.Value[i]);
            }

            this.EntityManager.DestroyEntity(tempArray);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(spawnedEntityArrayLength)]
        public void Add_AddInSequence_ValuesAdded(int entityCount)
        {
            // Arrange & Act
            var tempArray = new NativeArray<Entity>(entityCount, Allocator.Temp);

            for (int i = 0; i < entityCount; i++)
            {
                var newEntity = this.EntityManager.CreateEntity();
                tempArray[i] = newEntity;
                this.spawnedEntityArray.Add(newEntity);
            }

            // Assert
            for (int i = 0; i < entityCount; i++)
            {
                Assert.AreEqual(tempArray[i], this.spawnedEntityArray.Value[i]);
            }

            for (int i = entityCount; i < spawnedEntityArrayLength; i++)
            {
                Assert.AreEqual(Entity.Null, this.spawnedEntityArray.Value[i]);
            }

            this.EntityManager.DestroyEntity(tempArray);
            this.spawnedEntityArray.Clear();
        }

        [TestCase(spawnedEntityArrayLength + 1)]
        [TestCase(spawnedEntityArrayLength + 2)]
        public void Add_AddExceedLength_GotException(int entityCount)
        {
            var tempArray = new NativeArray<Entity>(entityCount, Allocator.Temp);

            for (int i = 0; i < entityCount; i++)
            {
                var newEntity = this.EntityManager.CreateEntity();
                tempArray[i] = newEntity;

                if (i < spawnedEntityArrayLength)
                {
                    this.spawnedEntityArray.Add(newEntity);
                    continue;
                }

                Assert.Throws<System.Exception>(() => this.spawnedEntityArray.Add(newEntity));
            }

            this.EntityManager.DestroyEntity(tempArray);
            this.spawnedEntityArray.Clear();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(spawnedEntityArrayLength)]
        public void Remove_ValidElements_ElementsRemoved(int entityCount)
        {
            // Arrange
            var tempArray = new NativeArray<Entity>(entityCount, Allocator.Temp);

            for (int i = 0; i < entityCount; i++)
            {
                var newEntity = this.EntityManager.CreateEntity();
                tempArray[i] = newEntity;
                this.spawnedEntityArray.Add(newEntity);
            }

            // Act & Assert
            for (int i = 0; i < entityCount; i++)
            {
                this.spawnedEntityArray.Remove(tempArray[i]);
                Assert.AreEqual(Entity.Null, this.spawnedEntityArray.Value[i]);
            }

            for (int i = entityCount; i < spawnedEntityArrayLength; i++)
            {
                Assert.AreEqual(Entity.Null, this.spawnedEntityArray.Value[i]);
            }

            this.EntityManager.DestroyEntity(tempArray);
            this.spawnedEntityArray.Clear();
        }

        [Test]
        public void Remove_InvalidElement_GotException()
        {
            // Arrange
            var tempArray = new NativeArray<Entity>(spawnedEntityArrayLength, Allocator.Temp);

            for (int i = 0; i < spawnedEntityArrayLength; i++)
            {
                var newEntity = this.EntityManager.CreateEntity();
                tempArray[i] = newEntity;
                this.spawnedEntityArray.Add(newEntity);
            }

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => this.spawnedEntityArray.Remove(Entity.Null));

            this.EntityManager.DestroyEntity(tempArray);
            this.spawnedEntityArray.Clear();
        }

    }

}