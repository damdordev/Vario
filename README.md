# Vario

A flexible and serializable variable storage system and generic math operations library for Unity. This package allows you to manage collections of variables with support for various types, as well as perform generic numeric operations (`Lerp`, `Sum`, `Subtract`) without boxing overhead.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
  - [Creating Storage and Adding Variables](#creating-storage-and-adding-variables)
  - [Retrieving Values](#retrieving-values)
  - [Using VarioValue in Components](#using-variovalue-in-components)
  - [Cloning Storage](#cloning-storage)
  - [Object Pooling](#object-pooling)
  - [Creating custom types](#creating-custom-types)
  - [Global Storage](#global-storage)
  - [Default global storages](#default-global-storages)
- [Numeric Operations](#numeric-operations)
  - [Getting a Numeric Operations Provider](#getting-a-numeric-operations-provider)
  - [Writing a Generic Interpolator](#writing-a-generic-interpolator)
  - [Adding Support for Custom Types](#adding-support-for-custom-types)
- [Supported Types](#supported-types)
  - [Variable Types](#variable-types)
  - [Numeric Operation Types](#numeric-operation-types)

## Features

*   **Typed Variables**: Strongly typed variable storage (Int, Float, String, Vector3, etc.).
*   **Generic Numeric Operations**: Standardized `INumericOperations<T>` interface for `Lerp`, `LerpUnclamped`, `Sum`, and `Subtract` without boxing overhead, ideal for tweening and interpolation.
*   **Auto-Discovery**: Automatically discovers and registers numeric operations across assemblies using `[NumericOperations]`.
*   **Serialization Support**: Built-in support for Unity serialization.
*   **Global storage**: Evaluate variables with fallback to global storage.
*   **Flexible Input**: `VarioValue<T>` allows fields in your scripts to easily switch between a constant value and a variable reference.
*   **Object Pooling**: Built-in object pooling for storages and variables to minimize GC allocations.
*   **Extensible**: Easy to add custom variable types and numeric operation handlers.

## Installation

This package is currently under development. In the future, it will be available via a UPM registry. For now, you can install it using the Git URL.

**Option A: Install via Package Manager window**
1. In Unity, open **Window** > **Package Manager**.
2. Click the **+** button and select **Add package from git URL...**
3. Enter the following URL and click **Add**:
   `https://github.com/damdordev/Vario.git#1.0.0-preview`

**Option B: Install via `manifest.json`**
Open your project's `Packages/manifest.json` file and add the following line to your `"dependencies"` block:
```json
"com.damdor.vario": "https://github.com/damdordev/Vario.git#1.0.0-preview"
```

## Basic Usage

### Creating Storage and Adding Variables

You can create a new storage directly or use the object pool to reduce allocations:
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

Vario has built-in support for object pooling, which helps manage memory efficiently. After you finish using the storage,
GC can collect it - but it's recommended to release it explicitly to return it to the pool:

```csharp
storage.Release();
```

### Retrieving Values

```csharp
// Get value directly
int score = VarioValue<int>.FromStorage("Score").Evaluate(storage);
int localScore = VarioValue<int>.FromStorage("Score").EvaluateLocal(storage);   // ignores global storages

// Get value with a default if not found
int health = VarioValue<int>.FromStorage("Health").Evaluate(storage, -1);
int localHealth = VarioValue<int>.FromStorage("Health").EvaluateLocal(storage, -1);    // ignores global storages

// Check if variable exists
bool hasScore = storage.Contains<int>("Score");
```

### Using VarioValue in Components

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
**Note:** `Release()` should *only* be called on cloned `VarioStorage` instances, or instances created via `VarioStorage.Create()`, 
as it clears the object and releases it along with its variables back to the pool.

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

To create a custom variable type containing a value of type `T`, create a class deriving from `VarioVariable<T>`
and add the `VarioVariable` attribute. The library uses code generation to create a list of supported variable types. 

```csharp
using System; 
using Damdor.Vario;

[Serializable]
public class MyCustomType {}

[Serializable]
[VarioVariable("MyCustomType")]
public class MyCustomTypeVariable : VarioVariable<MyCustomType> { }
```

### Global Storage

You can register global storages that are accessible throughout your application.
* Create a Global Storage (`Assets/Create/Damdor/Vario/Global Storage`). You can have as many global storages as you want.
* Create a Global Storage Library (`Assets/Create/Damdor/Vario/Global Storage Library`). The library should be put in the `Resources` folder and
be named `VarioGlobalStorages`. 
* Add your global storage to the library

### Default global storages

Vario has default global storages you can use. They are placed in the Vario package's `DefaultStorages` folder. 
Right now `Vario` comes with the following default storages:
* `Easing` - list of default easing animation curves

## Numeric Operations

Vario includes a generic numeric operations system that provides math operations (`Lerp`, `LerpUnclamped`, `Sum`, `Subtract`) 
for various types without the overhead of heavy boxing/unboxing or manually writing type-specific methods over and over.
This provides a lightweight alternative to .NET generic math ([INumber<TSelf>](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.inumber-1?view=net-10.0))
for interpolation and basic arithmetic in Unity.

The `INumericOperations<T>` interface makes it easy to create generic tweeners, 
interpolators, and math utilities that work natively with `float`, `Vector3`, `Color`, and more.

### Getting a Numeric Operations Provider

To get an operation provider for a specific type, use `VarioSettings.GetNumericOperations<T>()`:

```csharp
using UnityEngine;
using Damdor.Vario;

public class NumericExample : MonoBehaviour
{
    void Start()
    {
        // Get the operations for float
        var floatOps = VarioSettings.GetNumericOperations<float>();
        float lerpedFloat = floatOps.Lerp(0f, 10f, 0.5f); // Returns 5.0f
        
        // Get the operations for Vector3
        var vectorOps = VarioSettings.GetNumericOperations<Vector3>();
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
        ops = VarioSettings.GetNumericOperations<T>();
        
        if (ops == null)
        {
            throw new System.ArgumentException($"No Numeric Operations registered for type {typeof(T)}!");
        }
    }

    public T GetValueAt(float time)
    {
        return ops.Lerp(startValue, endValue, time);
    }
}
```

### Adding Support for Custom Types

If you have a custom struct or class that you want to animate/interpolate, inherit from `BaseNumericOperations<T>` and
add the `[NumericOperations]` attribute for auto-discovery. Vario uses code generation to create a list of supported types:

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

## Supported Types

### Variable Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`

### Numeric Operation Types

Built-in `INumericOperations<T>` implementations are available for:
*   Primitives: `float`, `double`, `bool`, `int`
*   Unity Types: `Vector2`, `Vector3`, `Vector4`, `Color`, `Rect`, `Quaternion`
