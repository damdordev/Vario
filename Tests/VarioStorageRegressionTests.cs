using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Damdor.Vario.Tests
{
    [TestFixture]
    public class VarioStorageRegressionTests
    {
        private VarioStorage storage;

        [SetUp]
        public void SetUp() => storage = new VarioStorage();

        [Test]
        public void TryGet_DistinguishesStoredDefaultFromMissingValue()
        {
            storage.Update("zero", 0);
            Assert.That(storage.TryGet<int>("zero", out var zero), Is.True);
            Assert.That(zero, Is.Zero);
            Assert.That(storage.TryGet<int>("missing", out var missing), Is.False);
            Assert.That(missing, Is.Zero);
        }

        [Test]
        public void TryGet_ReturnsFalseForWrongType()
        {
            storage.Update("value", 42);
            Assert.That(storage.TryGet<string>("value", out var value), Is.False);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void NullNames_AreRejectedByStorageOperations()
        {
            Assert.Throws<ArgumentNullException>(() => storage.Get<int>(null));
            Assert.Throws<ArgumentNullException>(() => storage.TryGet<int>(null, out _));
            Assert.Throws<ArgumentNullException>(() => storage.Contains(null));
            Assert.Throws<ArgumentNullException>(() => storage.Contains<int>(null));
            Assert.Throws<ArgumentNullException>(() => storage.GetType(null));
            Assert.Throws<ArgumentNullException>(() => storage.Update(null, 1));
            Assert.Throws<ArgumentNullException>(() => storage.Remove(null));
            Assert.Throws<ArgumentNullException>(() => storage.Update((VarioVariable)null));
            Assert.Throws<ArgumentNullException>(() => storage.Update(new VarioVariable<int>()));
        }

        [Test]
        public void Clear_RemovesCachedValuesAndAllowsReuse()
        {
            storage.Update("old", 42);
            storage.Clear();
            Assert.That(storage.Variables, Is.Empty);
            Assert.That(storage.Contains("old"), Is.False);
            storage.Update("old", "replacement");
            Assert.That(storage.Get<string>("old"), Is.EqualTo("replacement"));
        }

        [Test]
        public void Update_VariableWithDifferentType_ReplacesCachedEntry()
        {
            storage.Update("value", 1);
            var source = new VarioVariable<string> { Name = "value", Value = "new" };
            storage.Update(source);
            source.Value = "changed";
            Assert.That(storage.Get<string>("value"), Is.EqualTo("new"));
            Assert.That(storage.Contains<int>("value"), Is.False);
            Assert.That(storage.Variables.Count, Is.EqualTo(1));
        }

        [Test]
        public void Update_OwnVariable_DoesNotDuplicateOrResetIt()
        {
            storage.Update("value", 42);
            storage.Update(storage.Variables[0]);
            Assert.That(storage.Get<int>("value"), Is.EqualTo(42));
            Assert.That(storage.Variables.Count, Is.EqualTo(1));
        }

        [Test]
        public void Clone_CopiesValuesAndKeepsMutationsIndependent()
        {
            storage.Update("value", 42);
            storage.Update("text", "original");
            var clone = storage.Clone();
            try
            {
                Assert.That(clone.Get<int>("value"), Is.EqualTo(42));
                Assert.That(clone.Get<string>("text"), Is.EqualTo("original"));
                Assert.That(clone.Variables[0], Is.Not.SameAs(storage.Variables[0]));
                clone.Update("value", 99);
                clone.Remove("text");
                Assert.That(storage.Get<int>("value"), Is.EqualTo(42));
                Assert.That(storage.Get<string>("text"), Is.EqualTo("original"));
            }
            finally { clone.Release(); }
            Assert.That(storage.Get<int>("value"), Is.EqualTo(42));
        }

        [Test]
        public void Clone_ReferenceValues_AreShallowCopied()
        {
            var value = new List<int> { 1 };
            storage.Update("list", value);
            var clone = storage.Clone();
            try { Assert.That(clone.Get<List<int>>("list"), Is.SameAs(value)); }
            finally { clone.Release(); }
        }

        [Test]
        public void CopyFrom_MergesAndReplacesTypesWithoutSharingVariables()
        {
            storage.Update("retained", true);
            storage.Update("value", 1);
            var source = new VarioStorage();
            source.Update("value", "copied");
            source.Update("added", 42);
            storage.CopyFrom(source);
            source.Update("added", 99);
            Assert.That(storage.Get<bool>("retained"), Is.True);
            Assert.That(storage.Get<string>("value"), Is.EqualTo("copied"));
            Assert.That(storage.Get<int>("added"), Is.EqualTo(42));
            Assert.That(storage.Variables.Count, Is.EqualTo(3));
        }

        [Test]
        public void CopyFrom_NullAndSelf_AreNoOps()
        {
            storage.Update("value", 42);
            storage.CopyFrom(null);
            storage.CopyFrom(storage);
            Assert.That(storage.Get<int>("value"), Is.EqualTo(42));
            Assert.That(storage.Variables.Count, Is.EqualTo(1));
        }

        [Test]
        public void OnAfterDeserialize_RebuildsPreviouslyPopulatedCache()
        {
            storage.Update("old", 1);
            SetSerializedVariables(storage, new VarioVariable<int> { Name = "new", Value = 42 });
            ((ISerializationCallbackReceiver)storage).OnAfterDeserialize();
            Assert.That(storage.Contains("old"), Is.False);
            Assert.That(storage.Get<int>("new"), Is.EqualTo(42));
        }

        [Test]
        public void OnAfterDeserialize_IgnoresNullAndUnnamedEntries()
        {
            SetSerializedVariables(storage, null, new VarioVariable<int>(),
                new VarioVariable<int> { Name = "valid", Value = 42 });
            Assert.DoesNotThrow(() => ((ISerializationCallbackReceiver)storage).OnAfterDeserialize());
            Assert.That(storage.Get<int>("valid"), Is.EqualTo(42));
        }

        [Test]
        public void DuplicateSerializedNames_EvaluateAgreesWithGet()
        {
            SetSerializedVariables(storage,
                new VarioVariable<int> { Name = "value", Value = 1 },
                new VarioVariable<int> { Name = "value", Value = 2 });
            ((ISerializationCallbackReceiver)storage).OnAfterDeserialize();
            Assert.That(VarioValue<int>.FromStorage("value").EvaluateLocal(storage),
                Is.EqualTo(storage.Get<int>("value")));
        }

        [Test]
        public void DuplicateNamesWithDifferentTypes_EvaluateAgreesWithGet()
        {
            SetSerializedVariables(storage,
                new VarioVariable<int> { Name = "value", Value = 1 },
                new VarioVariable<string> { Name = "value", Value = "last" });
            ((ISerializationCallbackReceiver)storage).OnAfterDeserialize();
            Assert.That(VarioValue<int>.FromStorage("value").EvaluateLocal(storage, -1),
                Is.EqualTo(storage.Get<int>("value", -1)));
        }

        [Test]
        public void Clone_UnnamedSerializedEntry_DoesNotPreventCopyingValidValues()
        {
            SetSerializedVariables(storage, new VarioVariable<int>(),
                new VarioVariable<int> { Name = "valid", Value = 42 });
            VarioStorage clone = null;
            try
            {
                Assert.DoesNotThrow(() => clone = storage.Clone());
                Assert.That(clone.Get<int>("valid"), Is.EqualTo(42));
            }
            finally { clone?.Release(); }
        }

        internal static void SetSerializedVariables(VarioStorage target, params VarioVariable[] variables)
        {
            // Simulates inspector-authored data, which bypasses Update's validation.
            typeof(VarioStorage).GetField("variables", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(target, new List<VarioVariable>(variables));
        }
    }
}
