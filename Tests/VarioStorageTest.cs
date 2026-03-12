using System;
using NUnit.Framework;
using UnityEngine;

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
            var globalVariable = new IntVarioVariable { Name = "TestInt", Value = 100 };
            globalStorage.Update("TestInt", 100);
            VarioSettings.RegisterGlobalStorage(globalStorage);

            var localVariable = new IntVarioVariable { Name = "TestInt", Value = 200 };
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
            var variable = new IntVarioVariable { Name = "StoredInt", Value = 99 };
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

            Assert.Equals(10, storage.Get<int>("TestInt"));
        }

        [Test]
        public void GetVariable_ReturnsNull_WhenVariableDoesNotExist()
        {
            var retrieved = storage.Get<int>("NonExistent");
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
            storage.Update<Guid>("TestGuid", expectedGuid);

            var result = VarioValue<Guid>.FromStorage("TestGuid").Evaluate(storage);
            Assert.That(result, Is.EqualTo(expectedGuid));
        }

        [Serializable]
        private class GuidVarioVariable : VarioVariable<Guid, string> { }
    }
}