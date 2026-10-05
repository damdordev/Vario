using NUnit.Framework;

namespace Damdor.Vario.Tests
{
    [TestFixture]
    public class VarioPoolingTests
    {
        private int storagePoolSize;
        private int variablePoolSize;

        [SetUp]
        public void SetUp()
        {
            storagePoolSize = VarioSettings.StoragePoolSize;
            variablePoolSize = VarioSettings.VariablePoolSize;
            VarioSettings.StoragePoolSize = 0;
            VarioSettings.VariablePoolSize = 0;
            VarioSettings.StoragePoolSize = 4;
            VarioSettings.VariablePoolSize = 4;
        }

        [TearDown]
        public void TearDown()
        {
            VarioSettings.StoragePoolSize = 0;
            VarioSettings.VariablePoolSize = 0;
            VarioSettings.StoragePoolSize = storagePoolSize;
            VarioSettings.VariablePoolSize = variablePoolSize;
        }

        [Test]
        public void Release_ClearsValuesBeforeStorageIsRentedAgain()
        {
            var original = VarioStorage.Create();
            original.Update("value", 42);
            original.Release();
            var rented = VarioStorage.Create();
            try
            {
                Assert.That(rented, Is.SameAs(original));
                Assert.That(rented.Variables, Is.Empty);
                Assert.That(rented.Contains("value"), Is.False);
                rented.Update("new", 99);
                Assert.That(rented.Get<int>("new"), Is.EqualTo(99));
            }
            finally { rented.Release(); }
        }

        [Test]
        public void Release_Twice_DoesNotReturnSameInstanceToTwoCallers()
        {
            var original = VarioStorage.Create();
            original.Release();
            original.Release();
            var first = VarioStorage.Create();
            var second = VarioStorage.Create();
            try
            {
                Assert.That(second, Is.Not.SameAs(first));
                first.Update("value", 42);
                Assert.That(second.Contains("value"), Is.False);
            }
            finally { first.Release(); second.Release(); }
        }

        [Test]
        public void RentedStorage_CanBeReleasedAgainAfterReuse()
        {
            var original = VarioStorage.Create();
            original.Release();
            var rented = VarioStorage.Create();
            rented.Update("value", 42);
            rented.Release();
            var next = VarioStorage.Create();
            try
            {
                Assert.That(next, Is.SameAs(rented));
                Assert.That(next.Variables, Is.Empty);
            }
            finally { next.Release(); }
        }

        [Test]
        public void ZeroStoragePoolSize_DisablesStorageReuse()
        {
            VarioSettings.StoragePoolSize = 0;
            var original = VarioStorage.Create();
            original.Release();
            var next = VarioStorage.Create();
            try { Assert.That(next, Is.Not.SameAs(original)); }
            finally { next.Release(); }
        }

        [Test]
        public void ReducingStoragePoolSize_DiscardsExcessInstances()
        {
            var first = VarioStorage.Create();
            var second = VarioStorage.Create();
            first.Release();
            second.Release();
            VarioSettings.StoragePoolSize = 1;
            var retained = VarioStorage.Create();
            var fresh = VarioStorage.Create();
            try
            {
                Assert.That(retained, Is.SameAs(first).Or.SameAs(second));
                Assert.That(fresh, Is.Not.SameAs(first));
                Assert.That(fresh, Is.Not.SameAs(second));
            }
            finally { retained.Release(); fresh.Release(); }
        }

        [Test]
        public void NegativePoolSizes_AreClampedToZeroEvenWhenPoolsContainObjects()
        {
            var original = VarioStorage.Create();
            original.Update("value", 42);
            original.Release();
            Assert.DoesNotThrow(() => VarioSettings.StoragePoolSize = -1);
            Assert.DoesNotThrow(() => VarioSettings.VariablePoolSize = -1);
            Assert.That(VarioSettings.StoragePoolSize, Is.Zero);
            Assert.That(VarioSettings.VariablePoolSize, Is.Zero);
        }

        [Test]
        public void RemovedVariable_IsReusedWithoutItsOldNameOrValue()
        {
            var storage = new VarioStorage();
            storage.Update("old", 42);
            var removed = storage.Variables[0];
            storage.Remove("old");
            storage.Update("new", 99);
            Assert.That(storage.Variables[0], Is.SameAs(removed));
            Assert.That(storage.Variables[0].Name, Is.EqualTo("new"));
            Assert.That(storage.Get<int>("new"), Is.EqualTo(99));
            Assert.That(storage.Contains("old"), Is.False);
        }
    }
}
