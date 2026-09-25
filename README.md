# Vario

A flexible and serializable variable storage system and generic math operations library for Unity. This package allows you to manage collections of variables with support for various types, as well as perform generic numeric operations (`Lerp`, `Sum`, `Subtract`) without boxing overhead.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
  - [Creating Storage and Adding Variables](#creating-storage-and-adding-variables)
  - [Retrieving Values](#retrieving-values)
  - [Using StorageValue in Components](#using-storagevalue-in-components)
  - [Cloning Storage](#cloning-storage)
  - [Object Pooling](#object-pooling)
  - [Creating custom types](#creating-custom-types)
  - [Global Storage](#global-storage)
  - [Default global storages](#default-global-storages)
  - [UIElements Pointer](#uielements-pointer)
- [Numeric Operations](#numeric-operations)
  - [Getting Started](#getting-started)
  - [Writing a Generic Interpolator](#writing-a-generic-interpolator)
  - [Adding Support for Custom Types](#adding-support-for-custom-types)
  - [Supported Numeric Types](#supported-numeric-types)
- [Supported Types](#supported-types)
  - [Variable Types](#variable-types)
  - [Numeric Operation Types](#numeric-operation-types)

## Features

*   **Typed Variables**: Strongly typed variable storage (Int, Float, String, Vector3, etc.).
*   **Generic Numeric Operations**: Standardized `INumericOperations<T>` interface for `Lerp`, `LerpUnclamped`, `Sum`, and `Subtract` without boxing overhead, ideal for tweening and interpolation.
*   **Auto-Discovery**: Automatically discovers and registers numeric operations across assemblies using `[NumericOperations]`.
*   **Serialization Support**: Built-in support for Unity serialization.
*   **Hierarchical Evaluation**: Evaluate variables with fallback to parent storage.
*   **Flexible Input**: `VarioValue<T>` allows fields in your scripts to easily switch between a constant value and a variable reference.
*   **Object Pooling**: Built-in object pooling for storages and variables to minimize GC allocations.
*   **Extensible**: Easy to add custom variable types, converters, and numeric operation handlers.
*   **UIElements Support**: Utilities like `UIElementsPointer<T>` to query UI Toolkit elements or pass direct references.

## Installation

This package is available via UPM.

## Basic Usage

### Creating Storage and Adding Variables

You can create a new storage directly, or optimally use the object pool:

```csharp
using Damdor.Vario;

// Creates a new instance or retrieves from the pool
var storage = VarioStorage.Create();

// Add an integer variable
storage.Update<int>("Score", 100);
// or
storage.Update("Score", 100);

// Add a string variable
storage.Update<string>("PlayerName", "Hero");
// or
storage.Update("PlayerName", "Hero");
```

### Retrieving Values

```csharp
// Get value directly
int score = VarioValue<int>.FromStorage("Score").Evaluate(storage);

// Get value with a default if not found
int health = VarioValue<int>.FromStorage("Health").Evaluate(storage, -1);

// Check if variable exists
bool hasScore = storage.Contains<int>("Score");
```

### Using StorageValue in Components

`VarioValue<T>` is a helper struct that allows you to expose a field in the Inspector that can either be a raw value or a reference to a variable in a storage.

```csharp
using UnityEngine;
using Damdor.Vario;

public class HealthComponent : MonoBehaviour, IVarioStorageSource
{
    public VarioStorage Storage => storage;
    
    // In Inspector, you can choose Source: Raw (value 100) or Storage (name "MaxHealth")
    public VarioValue<float> maxHealth; 
    public VarioStorage storage; 

    void Start()
    {
        // Evaluates based on the Source setting
        float currentMax = maxHealth.Evaluate(storage);
        Debug.Log($"Max Health: {currentMax}");
    }
}
```

### Cloning Storage

You can create a copy of a storage utilizing an underlying object pooling mechanism to reduce memory allocations:

```csharp
var clone = originalStorage.Clone();

// Later when the clone is no longer needed:
clone.Release();
```
**Note:** `Release()` should *only* be called on cloned `VarioStorage` instances, or instances created via `VarioStorage.Create()`, as it clears the object and releases it along with its variables back to the pool.

### Object Pooling

The Vario system uses object pooling under the hood to minimize garbage collection allocations when creating and cloning storages or adding variables.

You can configure the maximum size of the pools to balance memory usage and performance:

```csharp
using Damdor.Vario;

// Set the maximum number of variables of EACH type to keep in the pool. Default is 100.
VarioSettings.VariablePoolSize = 200;

// Set the maximum number of VarioStorage instances to keep in the pool. Default is 100.
VarioSettings.StoragePoolSize = 50;
```

### Creating custom types

To create a custom variable type containing value of type `<c>T</c>`, create class deriving from `<c>VarioVariable<T></c>`
and add `<c>VarioVariable</c>` attribute. Library automatically created list of supported variable types. 

```csharp

public class MyCustomType {}

[Serializable]
[VarioVariable("MyCustomType")]
public class MyCustomTypeVariable : VarioVariable<MyCustomType> { }
```

### Global Storage

You can register global storages that are accessible throughout your application.
* Create global storage (`Assets/Create/Damdor/Vario/Global Storage`). You can have as many global storages as you want.
* Create global storage library (`Assets/Create/Damdor/Vario/Global Storage Library`). Library should be put in `Resources` folder and
be named `VarioGlobalStorages`. 
* Add your global storage to the library

### Default global storages

Vario has default global storages you can use. They are placed in vario library in `DefaultStorages` folder. 
Right now `Vario` comes with following default storages:
* `Easing` - list of default easing animation curves

### UIElements Pointer

If the package is used in a project utilizing UI Elements (`com.unity.modules.uielements`), the `UIElementsPointer<T>` feature is unlocked. It allows for flexible references to `VisualElement` derived classes. It can either store a direct reference or dynamically query the visual tree using `UiElementsQuery`.

To enable querying, a root UI element or `UIDocument` must be injected into a `VarioStorage`:

```csharp
using UnityEngine.UIElements;
using Damdor.Vario;

var storage = VarioStorage.Create();
var document = GetComponent<UIDocument>();

// Adds the root VisualElement to the storage for UIElementsPointer to use
VarioHelper.PutRoot(storage, document);
```

Then you can evaluate the pointer:

```csharp
using UnityEngine.UIElements;
using Damdor.Vario;

// Configured in inspector
public UIElementsPointer<Button> submitButtonPointer;
public VarioStorage storage;

public void OnEnable()
{
    // Will return a Button either via direct reference or by evaluating a query from the root
    Button submit = submitButtonPointer.Evaluate(storage);
}
```

## Numeric Operations

Vario includes a generic numeric operations system that provides math operations (`Lerp`, `LerpUnclamped`, `Sum`, `Subtract`) for various types without the overhead of heavy boxing/unboxing or manually writing type-specific methods over and over.

By defining an interface for math operations (`INumericOperations<T>`), it makes it easy to create generic tweeners, interpolators, and math utilities that work natively with `float`, `Vector3`, `Color`, and more.

### Getting Started

To get an operation provider for a specific type, use `NumerioSettings.Get<T>()`:

```csharp
using UnityEngine;
using Damdor.Vario;

public class NumericExample : MonoBehaviour
{
    void Start()
    {
        // Get the operations for float
        var floatOps = NumerioSettings.Get<float>();
        float lerpedFloat = floatOps.Lerp(0f, 10f, 0.5f); // Returns 5.0f
        
        // Get the operations for Vector3
        var vectorOps = NumerioSettings.Get<Vector3>();
        Vector3 sum = vectorOps.Sum(Vector3.one, Vector3.up); // Returns (1, 2, 1)
        
        Debug.Log($"Lerped Float: {lerpedFloat}");
        Debug.Log($"Vector3 Sum: {sum}");
    }
}
```

### Writing a Generic Interpolator

Numeric operations shine when writing generic systems, such as a generic Tween class:

```csharp
using UnityEngine;
using Damdor.Vario;

public class GenericTweener<T>
{
    private T startValue;
    private T endValue;
    private INumericOperations<T> ops;

    public GenericTweener(T start, T end)
    {
        startValue = start;
        endValue = end;
        ops = NumerioSettings.Get<T>();
        
        if (ops == null)
        {
            Debug.LogError($"No Numeric Operations registered for type {typeof(T)}!");
        }
    }

    public T GetValueAt(float time)
    {
        return ops.Lerp(startValue, endValue, time);
    }
}
```

### Adding Support for Custom Types

If you have a custom struct or class that you want to animate/interpolate, inherit from `BaseNumericOperations<T>` and add the `[NumericOperations]` attribute for auto-discovery:

```csharp
using UnityEngine;
using Damdor.Vario;

public struct MyCustomData
{
    public float Weight;
    public int Score;
}

// 1. Inherit from BaseNumericOperations<T>
// 2. Add the [NumericOperations] attribute for auto-discovery
[NumericOperations]
public class MyCustomDataOperations : BaseNumericOperations<MyCustomData>
{
    public override MyCustomData Lerp(MyCustomData a, MyCustomData b, float t)
    {
        return new MyCustomData
        {
            Weight = Mathf.Lerp(a.Weight, b.Weight, t),
            Score = Mathf.RoundToInt(Mathf.Lerp(a.Score, b.Score, t))
        };
    }

    public override MyCustomData LerpUnclamped(MyCustomData a, MyCustomData b, float t)
    {
        return new MyCustomData
        {
            Weight = Mathf.LerpUnclamped(a.Weight, b.Weight, t),
            Score = Mathf.RoundToInt(Mathf.LerpUnclamped(a.Score, b.Score, t))
        };
    }

    public override MyCustomData Sum(MyCustomData a, MyCustomData b)
    {
        return new MyCustomData
        {
            Weight = a.Weight + b.Weight,
            Score = a.Score + b.Score
        };
    }

    public override MyCustomData Subtract(MyCustomData a, MyCustomData b)
    {
        return new MyCustomData
        {
            Weight = a.Weight - b.Weight,
            Score = a.Score - b.Score
        };
    }
}
```

### Supported Numeric Types

Built-in numeric operations are provided for:
*   **Primitives**: `float`, `double`, `bool`
*   **Unity Types**: `Vector2`, `Vector3`, `Color`, `Rect`, `Quaternion`
*   **System Types**: `DateTime`, `TimeSpan`

## Supported Types

### Variable Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`

### Numeric Operation Types

Built-in `INumericOperations<T>` implementations are available for:
*   Primitives: `float`, `double`, `bool`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `Quaternion`
*   System Types: `DateTime`, `TimeSpan`
