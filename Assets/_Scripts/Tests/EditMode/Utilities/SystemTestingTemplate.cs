using NUnit.Framework;
using Utilities;
using Core.Testing;

namespace Tests.EditMode.Utilities
{
    public class SystemTestingTemplate : ECSTestsFixture
    {
        public override void SetUp()
        {
            base.SetUp();
            //this.CreateManagedSystem<ResourceTypeLengthInitSystem>();
        }

        public override void TearDown()
        {
            base.TearDown();
            SingletonUtilities.DestroyInstance();
        }

        [Test]
        public void TestSomething()
        {
            //var resourceTypeEnumLength = SingletonUtilities.GetInstance(this.EntityManager)
            //    .GetSingleton<EnumLength<ResourceType>>();

            //this.UpdateManagedSystem<ResourceTypeLengthInitSystem>();

            //Assert.AreEqual(resourceTypeEnumLength.Value, 3);

        }

    }

}