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

        [Test]
        public void Contains_ReturnsTrue_WhenVariableExists()
        {
            storage.Update("TestVar", 42);

            Assert.That(storage.Contains("TestVar"), Is.True);
        }

        [Test]
        public void Contains_ReturnsFalse_WhenVariableDoesNotExist()
        {
            Assert.That(storage.Contains("NonExistent"), Is.False);
        }

        [Test]
        public void ContainsGeneric_ReturnsTrue_WhenVariableExistsWithMatchingType()
        {
            storage.Update("TestInt", 10);

            Assert.That(storage.Contains<int>("TestInt"), Is.True);
        }

        [Test]
        public void ContainsGeneric_ReturnsFalse_WhenVariableExistsWithDifferentType()
        {
            storage.Update("TestInt", 10);

            Assert.That(storage.Contains<string>("TestInt"), Is.False);
        }

        [Test]
        public void ContainsGeneric_ReturnsFalse_WhenVariableDoesNotExist()
        {
            Assert.That(storage.Contains<int>("NonExistent"), Is.False);
        }

        [Test]
        public void GetVariableType_ReturnsTypeOfVariable_WhenVariableExists()
        {
            storage.Update("TestInt", 42);

            Assert.That(storage.GetType("TestInt"), Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetVariableType_ReturnsNull_WhenVariableDoesNotExist()
        {
            Assert.That(storage.GetType("NonExistent"), Is.Null);
        }

        [Test]
        public void Remove_ReturnsTrueAndRemovesVariable_WhenVariableExists()
        {
            storage.Update("TestVar", 10);

            var removed = storage.Remove("TestVar");

            Assert.That(removed, Is.True);
            Assert.That(storage.Contains("TestVar"), Is.False);
            Assert.That(storage.Variables.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ReturnsFalse_WhenVariableDoesNotExist()
        {
            var removed = storage.Remove("NonExistent");

            Assert.That(removed, Is.False);
        }

        [Test]
        public void Update_ReplacesExistingVariable_WhenUpdatedWithDifferentType()
        {
            storage.Update("DynamicVar", 123);
            Assert.That(storage.Contains<int>("DynamicVar"), Is.True);
            Assert.That(storage.Get<int>("DynamicVar"), Is.EqualTo(123));
            Assert.That(storage.GetType("DynamicVar"), Is.EqualTo(typeof(int)));
            
            storage.Update("DynamicVar", "UpdatedString");

            Assert.That(storage.Contains<string>("DynamicVar"), Is.True);
            Assert.That(storage.Contains<int>("DynamicVar"), Is.False);
            Assert.That(storage.Get<string>("DynamicVar"), Is.EqualTo("UpdatedString"));
            Assert.That(storage.Get<int>("DynamicVar", -1), Is.EqualTo(-1));
            Assert.That(storage.GetType("DynamicVar"), Is.EqualTo(typeof(string)));
            Assert.That(storage.Variables.Count, Is.EqualTo(1));
        }
        
        
        
    }
}