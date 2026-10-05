using System;
using System.Collections.Generic;
using Damdor.Vario.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Damdor.Vario.Tests
{
    [TestFixture]
    public class SerializedPropertyHelperTests
    {
        public class TestHost : ScriptableObject
        {
            public VarioValue<int> direct;
            public VarioValue<int>[] array = { VarioValue<int>.Raw(1) };
            public Settings settings = new Settings();
            [SerializeReference] public Entry polymorphic = new DerivedEntry();
        }

        [Serializable]
        public class Settings
        {
            public List<VarioValue<int>> items = new List<VarioValue<int>> { VarioValue<int>.Raw(1) };
            public VarioValue<int>[] array = { VarioValue<int>.Raw(1) };
        }

        [Serializable]
        public class Entry { }

        [Serializable]
        public class DerivedEntry : Entry
        {
            public VarioValue<string> value;
        }

        private TestHost host;
        private SerializedObject serializedObject;

        [SetUp]
        public void SetUp()
        {
            host = ScriptableObject.CreateInstance<TestHost>();
            serializedObject = new SerializedObject(host);
        }

        [TearDown]
        public void TearDown()
        {
            serializedObject.Dispose();
            UnityEngine.Object.DestroyImmediate(host);
        }

        [TestCase("direct")]
        [TestCase("array.Array.data[0]")]
        [TestCase("settings.items.Array.data[0]")]
        [TestCase("settings.array.Array.data[0]")]
        public void GetPropertyType_ResolvesDirectAndNestedCollectionFields(string path)
        {
            var property = serializedObject.FindProperty(path);
            Assert.That(property, Is.Not.Null, "Test property must exist: " + path);
            Assert.That(SerializedPropertyHelper.GetPropertyType(property), Is.EqualTo(typeof(VarioValue<int>)));
        }

        [Test]
        public void GetPropertyType_ResolvesManagedReferenceDerivedField()
        {
            var property = serializedObject.FindProperty("polymorphic.value");
            Assert.That(property, Is.Not.Null);
            Assert.That(SerializedPropertyHelper.GetPropertyType(property), Is.EqualTo(typeof(VarioValue<string>)));
        }

        [Test]
        public void Storage_JsonRoundTrip_PreservesTypesValuesAndLookup()
        {
            var original = new VarioStorage();
            original.Update("number", 42);
            original.Update("text", "hello");
            var restored = JsonUtility.FromJson<VarioStorage>(JsonUtility.ToJson(original));
            Assert.That(restored.Get<int>("number"), Is.EqualTo(42));
            Assert.That(restored.Get<string>("text"), Is.EqualTo("hello"));
            Assert.That(restored.Contains<string>("number"), Is.False);
            Assert.That(VarioValue<int>.FromStorage("number").EvaluateLocal(restored), Is.EqualTo(42));
        }
    }
}
