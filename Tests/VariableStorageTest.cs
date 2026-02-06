using System;
using NUnit.Framework;

namespace Damdor.VariableStorage.Tests
{
    [TestFixture]
    public class VariableStorageTest
    {
        private VariableStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new VariableStorage();
            VariableStorageSettings.ResetToInitialSettings();
        }

        [TearDown]
        public void TearDown()
        {
            VariableStorageSettings.ResetToInitialSettings();
        }

        [Test]
        public void AddVariable_AddsVariableToStorage()
        {
            var variable = new IntVariable { Name = "TestInt", Value = 10 };
            storage.AddVariable(variable);

            Assert.That(storage.HasVariable<int>("TestInt"), Is.True);
            Assert.That(storage.Evaluate<int>("TestInt"), Is.EqualTo(10));
        }

        [Test]
        public void Evaluate_ReturnsDefaultValue_WhenVariableNotFound()
        {
            var result = storage.Evaluate("NonExistent", 5);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void Evaluate_ReturnsValue_WhenVariableExists()
        {
            var variable = new StringVariable { Name = "TestString", Value = "Hello" };
            storage.AddVariable(variable);

            var result = storage.Evaluate<string>("TestString");
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsParentValue_WhenVariableNotFoundLocally()
        {
            var parentStorage = new VariableStorage();
            var variable = new FloatVariable { Name = "TestFloat", Value = 3.14f };
            parentStorage.AddVariable(variable);

            var result = storage.Evaluate<float>("TestFloat", parentStorage);
            Assert.That(result, Is.EqualTo(3.14f));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsLocalValue_WhenVariableExistsLocally()
        {
            var parentStorage = new VariableStorage();
            var parentVariable = new IntVariable { Name = "TestInt", Value = 100 };
            parentStorage.AddVariable(parentVariable);

            var localVariable = new IntVariable { Name = "TestInt", Value = 200 };
            storage.AddVariable(localVariable);

            var result = storage.Evaluate<int>("TestInt", parentStorage);
            Assert.That(result, Is.EqualTo(200));
        }

        [Test]
        public void Evaluate_StorageValue_RawSource_ReturnsRawValue()
        {
            var storageValue = new StorageValue<int>
            {
                Source = ValueSource.Raw,
                Value = 42
            };

            var result = storage.Evaluate(storageValue);
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_StorageValue_StorageSource_ReturnsStoredValue()
        {
            var variable = new IntVariable { Name = "StoredInt", Value = 99 };
            storage.AddVariable(variable);

            var storageValue = new StorageValue<int>
            {
                Source = ValueSource.Storage,
                Name = "StoredInt"
            };

            var result = storage.Evaluate(storageValue);
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void Evaluate_StorageValue_WithParent_ReturnsParentValue()
        {
            var parentStorage = new VariableStorage();
            var variable = new IntVariable { Name = "StoredInt", Value = 99 };
            parentStorage.AddVariable(variable);

            var storageValue = new StorageValue<int>
            {
                Source = ValueSource.Storage,
                Name = "StoredInt"
            };

            var result = storage.Evaluate(storageValue, parentStorage);
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void UpdateVariable_UpdatesExistingVariable()
        {
            var variable = new IntVariable { Name = "UpdateTest", Value = 1 };
            storage.AddVariable(variable);

            var success = storage.UpdateVariable("UpdateTest", 2);
            
            Assert.That(success, Is.True);
            Assert.That(storage.Evaluate<int>("UpdateTest"), Is.EqualTo(2));
        }

        [Test]
        public void UpdateVariable_ReturnsFalse_WhenVariableDoesNotExist()
        {
            var success = storage.UpdateVariable("NonExistent", 10);
            Assert.That(success, Is.False);
        }

        [Test]
        public void GetVariable_ReturnsTypedVariable_WhenExists()
        {
            var variable = new IntVariable { Name = "TestInt", Value = 10 };
            storage.AddVariable(variable);

            var retrieved = storage.GetVariable<int>("TestInt");
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.Value, Is.EqualTo(10));
        }

        [Test]
        public void GetVariable_ReturnsNull_WhenVariableDoesNotExist()
        {
            var retrieved = storage.GetVariable<int>("NonExistent");
            Assert.That(retrieved, Is.Null);
        }

        [Test]
        public void Evaluate_ComplexVariable_ReturnsConvertedValue()
        {
            VariableStorageSettings.RegisterConverter<Guid, string>(
                guid => guid.ToString(),
                Guid.Parse
            );
            
            var expectedGuid = Guid.NewGuid();
            var variable = new GuidVariable { Name = "TestGuid", Value = expectedGuid };
            storage.AddVariable(variable);

            var result = storage.Evaluate<Guid>("TestGuid");
            Assert.That(result, Is.EqualTo(expectedGuid));
        }

        [Serializable]
        private class GuidVariable : Variable<Guid, string> { }
    }
}