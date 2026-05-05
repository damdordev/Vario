# Vario

A flexible and serializable variable storage system for Unity. This package allows you to manage collections 
of variables with support for various types.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
  - [Creating Storage and Adding Variables](#creating-storage-and-adding-variables)
  - [Retrieving Values](#retrieving-values)
  - [Using StorageValue in Components](#using-storagevalue-in-components)
  - [Creating custom types](#creating-custom-types)
  - [Global Storage](#global-storage)
  - [Default global storages](#default-global-storages)
- [Supported Types](#supported-types)

## Features

*   **Typed Variables**: Strongly typed variable storage (Int, Float, String, Vector3, etc.).
*   **Serialization Support**: Built-in support for Unity serialization.
*   **Hierarchical Evaluation**: Evaluate variables with fallback to parent storage.
*   **Flexible Input**: `VarioValue<T>` allows fields in your scripts to easily switch between a constant value and a variable reference.
*   **Extensible**: Easy to add custom variable types and converters.

## Installation

This package is available via UPM.

## Basic Usage

### Creating Storage and Adding Variables

```csharp
using Damdor.Vario;

var storage = new VarioStorage();

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
bool hasScore = storage.HasVariable<int>("Score");
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

### Creating custom types

To create a custom variable type containing value of type <c>T</c>, create class deriving from <c>VarioVariable<T></c>
and add <c>VarioVariable</c> attribute. Library automatically created list of supported variable types. 

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

## Supported Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`
