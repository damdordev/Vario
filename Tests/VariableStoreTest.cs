using System;
using System.Linq;
using NUnit.Framework;

namespace Damdor.VariableStorage
{
    public class VariableStoreTest
    {
        public struct NotSupportedVariableType {}

        public struct SupportedVariableType
        {
            public int X;
        }
        public class SupportedVariableTypeVariable : Variable<SupportedVariableType> {}
        
        private VariableStorage storage;

        [SetUp]
        public void Setup()
        {
            storage = new VariableStorage();
            VariableStorage.RegisterVariableType<SupportedVariableTypeVariable, SupportedVariableType>();
        }
        
        [Test]
        public void TestIfNewStorageHasNoVariables()
        {
            Assert.AreEqual(0, storage.AllVariables.Count());
        }
        
        [Test]
        public void TestAddingVariable()
        {
            storage.Set("someName", 5);

            CollectionAssert.AreEqual(
                new VariableMetadata[] { new("someName", typeof(int)) },
                storage.AllVariables
            );
            Assert.IsTrue(storage.Contains<int>("someName"));
            Assert.AreEqual(5, storage.Get<int>("someName"));
        }

        [Test]
        public void TestIfVariablesAreNotCovariant()
        {
            storage.Set("someName", 5);

            Assert.IsFalse(storage.Contains<float>("someName"));
        }

        [Test]
        public void TestRemovingVariable()
        {
            storage.Set("someName", 5);
            storage.Remove<int>("someName");

            CollectionAssert.AreEqual(
                Array.Empty<VariableMetadata>(),
                storage.AllVariables
            );
            Assert.IsFalse(storage.Contains<int>("someName"));
        }

        [Test]
        public void TestIfGetNonExistingVariableReturnsDefault()
        {
            storage.Set("someName", 5);
            
            Assert.IsFalse(storage.Contains<int>("otherName"));
            Assert.AreEqual(0, storage.Get<int>("otherName"));
        }
        
        [Test]
        public void TestRenamingVariable()
        {
            storage.Set("someName", 5);
            var renameResult = storage.Rename<int>("someName", "otherName");

            Assert.IsTrue(renameResult);
            CollectionAssert.AreEqual(
                new VariableMetadata[] { new("otherName", typeof(int)) },
                storage.AllVariables
            );
            Assert.IsFalse(storage.Contains<int>("someName"));
            Assert.IsTrue(storage.Contains<int>("otherName"));
            Assert.AreEqual(5, storage.Get<int>("otherName"));
        }

        [Test]
        public void TestRenamingNonExistingVariable()
        {
            storage.Set("someName", 5);
            var renameResult = storage.Rename<int>("otherName", "otherName2");
            
            Assert.IsFalse(renameResult);
            CollectionAssert.AreEqual(
                new VariableMetadata[] { new("someName", typeof(int)) },
                storage.AllVariables
            );
        }

        [Test]
        public void TestRenamingVariableToExistingName()
        {
            storage.Set("someName", 5);
            storage.Set("otherName", 12);
            var renameResult = storage.Rename<int>("someName", "otherName");

            Assert.IsFalse(renameResult);
            CollectionAssert.AreEqual(
                new VariableMetadata[]
                {
                    new("someName", typeof(int)),
                    new("otherName", typeof(int))
                },
                storage.AllVariables
            );
            Assert.IsTrue(storage.Contains<int>("someName"));
            Assert.IsTrue(storage.Contains<int>("otherName"));
            Assert.AreEqual(5, storage.Get<int>("someName"));
            Assert.AreEqual(12, storage.Get<int>("otherName"));
        }
        
        [Test]
        public void TestRenamingVariableToExistingNameOfOtherType()
        {
            storage.Set("someName", 5);
            storage.Set("otherName", 12f);
            var renameResult = storage.Rename<int>("someName", "otherName");

            Assert.IsTrue(renameResult);
            CollectionAssert.AreEqual(
                new VariableMetadata[]
                {
                    new("otherName", typeof(int)),
                    new("otherName", typeof(float))
                },
                storage.AllVariables
            );
            Assert.IsFalse(storage.Contains<int>("someName"));
            Assert.IsTrue(storage.Contains<int>("otherName"));
            Assert.AreEqual(5, storage.Get<int>("otherName"));
        }

        [Test]
        public void TestSettingNotSupportedVariable()
        {
            storage.Set("someName", new NotSupportedVariableType());
            
            CollectionAssert.AreEqual(
                Array.Empty<VariableMetadata>(),
                storage.AllVariables
            );
            Assert.IsFalse(storage.Contains<NotSupportedVariableType>("someName"));
            Assert.AreEqual(new NotSupportedVariableType(), storage.Get<NotSupportedVariableType>("someName"));
        }
        
        [Test]
        public void TestSettingNewSupportedVariable()
        {
            storage.Set("someName", new SupportedVariableType { X = 11 });
            
            CollectionAssert.AreEqual(
                new VariableMetadata[] { new("someName", typeof(SupportedVariableType)) },
                storage.AllVariables
            );
            Assert.IsTrue(storage.Contains<SupportedVariableType>("someName"));
            Assert.AreEqual(new SupportedVariableType { X = 11 }, storage.Get<SupportedVariableType>("someName"));
        }
        
    }
}