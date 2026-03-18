using NUnit.Framework;

namespace Damdor.Vario.Tests
{
    [TestFixture]
    public class VarioStorageTest
    {
        private VarioStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new VarioStorage();
            VarioSettings.ResetToInitialSettings();
        }

        [TearDown]
        public void TearDown()
        {
            VarioSettings.ResetToInitialSettings();
        }

        [Test]
        public void AddVariable_AddsVariableToStorage()
        {
            storage.Update("TestInt", 10);

            Assert.That(storage.Contains<int>("TestInt"), Is.True);
            Assert.That(VarioValue<int>.FromStorage("TestInt").Evaluate(storage), Is.EqualTo(10));
        }

        [Test]
        public void Evaluate_ReturnsDefaultValue_WhenVariableNotFound()
        {
            var result = VarioValue<int>.FromStorage("NonExistent").Evaluate(storage, 5);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void Evaluate_ReturnsValue_WhenVariableExists()
        {
            storage.Update("TestString", "Hello");

            var result = VarioValue<string>.FromStorage("TestString").Evaluate(storage);
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsParentValue_WhenVariableNotFoundLocally()
        {
            var globalStorage = new VarioStorage();
            globalStorage.Update("TestFloat", 3.14f);

            VarioSettings.RegisterGlobalStorage(globalStorage);
            var result = VarioValue<float>.FromStorage("TestFloat").Evaluate(storage);
            Assert.That(result, Is.EqualTo(3.14f));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsLocalValue_WhenVariableExistsLocally()
        {
            var globalStorage = new VarioStorage();
            globalStorage.Update("TestInt", 100);
            VarioSettings.RegisterGlobalStorage(globalStorage);

            storage.Update("TestInt", 200);

            var result = VarioValue<int>.FromStorage("TestInt").Evaluate(storage);
            Assert.That(result, Is.EqualTo(200));
        }

        [Test]
        public void Evaluate_StorageValue_RawSource_ReturnsRawValue()
        {
            var storageValue = new VarioValue<int>
            {
                Source = ValueSource.Raw,
                Value = 42
            };

            var result = storageValue.Evaluate(storage);
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_StorageValue_StorageSource_ReturnsStoredValue()
        {
            storage.Update("StoredInt", 99);

            var storageValue = new VarioValue<int>
            {
                Source = ValueSource.Storage,
                Name = "StoredInt"
            };

            var result = storageValue.Evaluate(storage);
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void Evaluate_StorageValue_WithParent_ReturnsParentValue()
        {
            var globalStorage = new VarioStorage();
            globalStorage.Update("StoredInt", 99);
            VarioSettings.RegisterGlobalStorage(globalStorage);

            var storageValue = new VarioValue<int>
            {
                Source = ValueSource.Storage,
                Name = "StoredInt"
            };

            var result = storageValue.Evaluate(storage);
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void UpdateVariable_UpdatesExistingVariable()
        {
            storage.Update("UpdateTest", 1);

            storage.Update("UpdateTest", 2);
            
            Assert.That( VarioValue<int>.FromStorage("UpdateTest").Evaluate(storage), Is.EqualTo(2));
        }

        [Test]
        public void GetVariable_ReturnsTypedVariable_WhenExists()
        {
            storage.Update("TestInt", 10);

            Assert.AreEqual(10, storage.Get<int>("TestInt"));
        }

        [Test]
        public void GetVariable_ReturnsDefaultValue_WhenVariableDoesNotExist()
        {
            Assert.AreEqual(10, storage.Get("NonExistent", 10));
        }
        
    }
}