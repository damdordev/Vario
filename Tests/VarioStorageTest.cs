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