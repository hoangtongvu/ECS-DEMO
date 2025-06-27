using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Core.GameResource;
using Core.Testing;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Utilities.Helpers;

namespace Tests.EditMode.Utilities.Helpers
{
    public class ResourceWalletHelperTests : ECSTestsFixture
    {
        private Entity singletonEntity;
        private NativeHashMap<Entity, int> entityToContainerIndexMap;
        private NativeList<uint> entitySpawningCostsContainer;

        public override void SetUp()
        {
            base.SetUp();

            this.singletonEntity = this.EntityManager.CreateEntity();

            // EntityToContainerIndexMap
            this.entityToContainerIndexMap = new NativeHashMap<Entity, int>(1, Allocator.Persistent)
            {
                { this.singletonEntity, 0 }
            };

            this.EntityManager.AddComponentData(singletonEntity, new EntityToContainerIndexMap
            {
                Value = this.entityToContainerIndexMap,
            });

            // EntitySpawningCostsContainer
            this.entitySpawningCostsContainer = new NativeList<uint>(ResourceType_Length.Value, Allocator.Persistent)
            {
                { 1 },
                { 2 },
                { 3 },
                { 4 },
            };

            this.EntityManager.AddComponentData(singletonEntity, new EntitySpawningCostsContainer
            {
                Value = this.entitySpawningCostsContainer,
            });

        }

        public override void TearDown()
        {
            base.TearDown();
            this.entityToContainerIndexMap.Dispose();
            this.entitySpawningCostsContainer.Dispose();
        }

        [Test]
        public void GetCostsSlice_ValidInputEntity_ReturnValidSlice()
        {
            var entityToContainerIndexMap = this.EntityManager.GetComponentData<EntityToContainerIndexMap>(this.singletonEntity);
            var entitySpawningCostsContainer = this.EntityManager.GetComponentData<EntitySpawningCostsContainer>(this.singletonEntity);

            ResourceWalletHelper.GetCostsSlice(
                in entityToContainerIndexMap
                , in entitySpawningCostsContainer
                , in this.singletonEntity
                , out var costs);

            for (int i = 0; i < ResourceType_Length.Value; i++)
            {
                Assert.AreEqual(entitySpawningCostsContainer.Value[i], costs[i], $"cost: {costs[i]}");
            }

        }

        [Test]
        public void TrySpendResources1_CanSpendResources_ReturnTrue()
        {
            var entity = this.EntityManager.CreateEntity();
            const uint quantityPerWalletElement = 15;

            var walletArr = ResourceWalletHelper
                .AddResourceWalletToEntity(this.EntityManager, in entity, quantityPerWalletElement)
                .ToNativeArray(Allocator.Temp);

            var entityToContainerIndexMap = this.EntityManager.GetComponentData<EntityToContainerIndexMap>(this.singletonEntity);
            var entitySpawningCostsContainer = this.EntityManager.GetComponentData<EntitySpawningCostsContainer>(this.singletonEntity);

            bool canSpendResources = ResourceWalletHelper
                .TrySpendResources(ref walletArr, entityToContainerIndexMap, entitySpawningCostsContainer, this.singletonEntity);

            Assert.IsTrue(canSpendResources);

            foreach (var walletElement in walletArr)
            {
                int containerIndex = (byte)walletElement.Type;
                uint cost = entitySpawningCostsContainer.Value[containerIndex];

                uint actual = walletElement.Quantity;
                uint expected = quantityPerWalletElement - cost;

                Assert.AreEqual(expected, actual, $"cost: {cost}");
                TestContext.WriteLine($"Passed: ResourceType={walletElement.Type}, Quantity={actual}");
            }

        }

        [Test]
        public void TrySpendResources1_CanNotSpendResources_ReturnFalse()
        {
            var entity = this.EntityManager.CreateEntity();
            const uint quantityPerWalletElement = 2;

            var walletArr = ResourceWalletHelper
                .AddResourceWalletToEntity(this.EntityManager, in entity, quantityPerWalletElement)
                .ToNativeArray(Allocator.Temp);

            var entityToContainerIndexMap = this.EntityManager.GetComponentData<EntityToContainerIndexMap>(this.singletonEntity);
            var entitySpawningCostsContainer = this.EntityManager.GetComponentData<EntitySpawningCostsContainer>(this.singletonEntity);

            bool canSpendResources = ResourceWalletHelper
                .TrySpendResources(ref walletArr, entityToContainerIndexMap, entitySpawningCostsContainer, this.singletonEntity);

            Assert.IsFalse(canSpendResources);

            foreach (var walletElement in walletArr)
            {
                int containerIndex = (byte)walletElement.Type;
                uint cost = entitySpawningCostsContainer.Value[containerIndex];

                uint actual = walletElement.Quantity;
                uint expected = quantityPerWalletElement >= cost
                    ? quantityPerWalletElement - cost
                    : quantityPerWalletElement;

                Assert.AreEqual(expected, actual, $"cost: {cost}");
                TestContext.WriteLine($"Passed: ResourceType={walletElement.Type}, Quantity={actual}");
            }

        }

    }

}