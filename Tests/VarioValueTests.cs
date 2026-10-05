using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Damdor.Vario.Tests
{
    [TestFixture]
    public class VarioValueTests
    {
        private List<VarioStorage> globals;
        private VarioStorage[] savedGlobals;
        private bool savedInitialized;
        private static readonly FieldInfo Initialized = typeof(VarioSettings)
            .GetField("globalStoragesInit", BindingFlags.Static | BindingFlags.NonPublic);

        [SetUp]
        public void SetUp()
        {
            // Avoid creating Resources assets or depending on the project's global library.
            globals = (List<VarioStorage>)typeof(VarioSettings)
                .GetField("globalStorages", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            savedGlobals = globals.ToArray();
            savedInitialized = (bool)Initialized.GetValue(null);
            globals.Clear();
            Initialized.SetValue(null, true);
        }

        [TearDown]
        public void TearDown()
        {
            globals.Clear();
            globals.AddRange(savedGlobals);
            Initialized.SetValue(null, savedInitialized);
        }

        [Test]
        public void RawAndImplicitValues_EvaluateWithoutStorage()
        {
            Assert.That(VarioValue<int>.Raw(42).Evaluate(null), Is.EqualTo(42));
            VarioValue<string> implicitValue = "text";
            Assert.That(implicitValue.Source, Is.EqualTo(ValueSource.Raw));
            Assert.That(implicitValue.Evaluate(null), Is.EqualTo("text"));
            Assert.That(default(VarioValue<int>).Evaluate(null, 99), Is.Zero);
        }

        [Test]
        public void RawValue_DoesNotResolveMatchingStorageName()
        {
            var storage = new VarioStorage();
            storage.Update("value", 99);
            var value = VarioValue<int>.Raw(42);
            value.Name = "value";
            Assert.That(value.Evaluate(storage), Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_LocalValueTakesPrecedenceOverGlobals()
        {
            var local = new VarioStorage();
            local.Update("value", 1);
            AddGlobal("value", 2);
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(local), Is.EqualTo(1));
        }

        [Test]
        public void Evaluate_MissingLocalValueFallsBackToGlobals()
        {
            AddGlobal("value", 42);
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(new VarioStorage()), Is.EqualTo(42));
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(null), Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_LocalTypeMismatchFallsBackToMatchingGlobalType()
        {
            var local = new VarioStorage();
            local.Update("value", "wrong type");
            AddGlobal("value", 42);
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(local), Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_UsesFirstMatchingGlobalAndSkipsNullOrWrongTypeStorages()
        {
            globals.Add(null);
            AddGlobal("value", "wrong type");
            AddGlobal("value", 42);
            AddGlobal("value", 99);
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(null), Is.EqualTo(42));
        }

        [Test]
        public void Evaluate_StoredNullAndZeroDoNotFallBackToGlobals()
        {
            var local = new VarioStorage();
            local.Update<string>("text", null);
            local.Update("number", 0);
            AddGlobal("text", "fallback");
            AddGlobal("number", 42);
            Assert.That(VarioValue<string>.FromStorage("text").Evaluate(local, "default"), Is.Null);
            Assert.That(VarioValue<int>.FromStorage("number").Evaluate(local, -1), Is.Zero);
        }

        [Test]
        public void EvaluateLocal_IgnoresGlobalsAndReturnsFallback()
        {
            AddGlobal("value", 42);
            var value = VarioValue<int>.FromStorage("value");
            Assert.That(value.EvaluateLocal(new VarioStorage(), -1), Is.EqualTo(-1));
            Assert.That(value.EvaluateLocal(null, -1), Is.EqualTo(-1));
        }

        [Test]
        public void Evaluate_MissingOrWrongTypeValuesReturnFallback()
        {
            AddGlobal("value", "wrong type");
            Assert.That(VarioValue<int>.FromStorage("value").Evaluate(null, -1), Is.EqualTo(-1));
            Assert.That(VarioValue<int>.FromStorage("missing").Evaluate(null, -1), Is.EqualTo(-1));
        }

        [Test]
        public void Evaluate_SkipsNullSerializedVariableEntries()
        {
            var local = new VarioStorage();
            VarioStorageRegressionTests.SetSerializedVariables(local, null,
                new VarioVariable<int> { Name = "value", Value = 42 });
            Assert.That(VarioValue<int>.FromStorage("value").EvaluateLocal(local), Is.EqualTo(42));
        }

        [Test]
        public void UnknownSource_IsRejectedByBothEvaluationModes()
        {
            var value = new VarioValue<int> { Source = (ValueSource)123 };
            Assert.Throws<ArgumentOutOfRangeException>(() => value.Evaluate(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => value.EvaluateLocal(null));
        }

        private void AddGlobal<T>(string name, T value)
        {
            var storage = new VarioStorage();
            storage.Update(name, value);
            globals.Add(storage);
        }
    }
}
