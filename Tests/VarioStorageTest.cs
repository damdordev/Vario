using System;
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
            var variable = new IntVarioVariable { Name = "TestInt", Value = 10 };
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
            var variable = new StringVarioVariable { Name = "TestString", Value = "Hello" };
            storage.AddVariable(variable);

            var result = storage.Evaluate<string>("TestString");
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsParentValue_WhenVariableNotFoundLocally()
        {
            var globalStorage = new VarioStorage();
            var variable = new FloatVarioVariable { Name = "TestFloat", Value = 3.14f };
            globalStorage.AddVariable(variable);

            VarioSettings.RegisterGlobalStorage(globalStorage);
            var result = storage.Evaluate<float>("TestFloat");
            Assert.That(result, Is.EqualTo(3.14f));
        }

        [Test]
        public void Evaluate_WithParent_ReturnsLocalValue_WhenVariableExistsLocally()
        {
            var globalStorage = new VarioStorage();
            var globalVariable = new IntVarioVariable { Name = "TestInt", Value = 100 };
            globalStorage.AddVariable(globalVariable);
            VarioSettings.RegisterGlobalStorage(globalStorage);

            var localVariable = new IntVarioVariable { Name = "TestInt", Value = 200 };
            storage.AddVariable(localVariable);

            var result = storage.Evaluate<int>("TestInt");
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

            var result = storage.Evaluate(storageValue);
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_StorageValue_StorageSource_ReturnsStoredValue()
        {
            var variable = new IntVarioVariable { Name = "StoredInt", Value = 99 };
            storage.AddVariable(variable);

            var storageValue = new VarioValue<int>
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
            var globalStorage = new VarioStorage();
            var variable = new IntVarioVariable { Name = "StoredInt", Value = 99 };
            globalStorage.AddVariable(variable);
            VarioSettings.RegisterGlobalStorage(globalStorage);

            var storageValue = new VarioValue<int>
            {
                Source = ValueSource.Storage,
                Name = "StoredInt"
            };

            var result = storage.Evaluate(storageValue);
            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void UpdateVariable_UpdatesExistingVariable()
        {
            var variable = new IntVarioVariable { Name = "UpdateTest", Value = 1 };
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
            var variable = new IntVarioVariable { Name = "TestInt", Value = 10 };
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
            VarioSettings.RegisterConverter<Guid, string>(
                guid => guid.ToString(),
                Guid.Parse
            );
            
            var expectedGuid = Guid.NewGuid();
            var variable = new GuidVarioVariable { Name = "TestGuid", Value = expectedGuid };
            storage.AddVariable(variable);

            var result = storage.Evaluate<Guid>("TestGuid");
            Assert.That(result, Is.EqualTo(expectedGuid));
        }

        [Serializable]
        private class GuidVarioVariable : VarioVariable<Guid, string> { }
    }
}