# Variable Storage

A flexible and serializable variable storage system for Unity. This package allows you to manage collections 
of variables with support for various types.

## Features

*   **Typed Variables**: Strongly typed variable storage (Int, Float, String, Vector3, etc.).
*   **Serialization Support**: Built-in support for Unity serialization, including handling of non-serializable types via converters.
*   **Hierarchical Evaluation**: Evaluate variables with fallback to parent storage.
*   **Flexible Input**: `StorageValue<T>` allows fields in your scripts to easily switch between a constant value and a variable reference.
*   **Extensible**: Easy to add custom variable types and converters.

## Installation

This package is available via UPM.

## Basic Usage

### Creating Storage and Adding Variables

```csharp
using Damdor.VariableStorage;

var storage = new VariableStorage();

// Add an integer variable
var intVar = new IntVariable { Name = "Score", Value = 100 };
storage.AddVariable(intVar);

// Add a string variable
var stringVar = new StringVariable { Name = "PlayerName", Value = "Hero" };
storage.AddVariable(stringVar);
```

### Retrieving Values

```csharp
// Get value directly
int score = storage.Evaluate<int>("Score");

// Get value with a default if not found
int health = storage.Evaluate<int>("Health", 100);

// Check if variable exists
bool hasScore = storage.HasVariable<int>("Score");
```

### Using StorageValue in Components

`StorageValue<T>` is a helper struct that allows you to expose a field in the Inspector that can either be a raw value or a reference to a variable in a storage.

```csharp
using UnityEngine;
using Damdor.VariableStorage;

public class HealthComponent : MonoBehaviour, IParentVariableStorageSource
{
    public IParentVariableStorageSource Storage => storage;
    
    // In Inspector, you can choose Source: Raw (value 100) or Storage (name "MaxHealth")
    public StorageValue<float> maxHealth; 
    public VariableStorage storage; // Assigned from somewhere

    void Start()
    {
        // Evaluates based on the Source setting
        float currentMax = storage.Evaluate(maxHealth);
        Debug.Log($"Max Health: {currentMax}");
    }
}
```

## Custom Types

### Creating a Custom Variable

To add a new type, inherit from `Variable<T>`.

```csharp
using System;
using Damdor.VariableStorage;
using UnityEngine;

[Serializable]
[VariableTypeName("myCustomType")]
public class MyCustomVariable : Variable<MyCustomType> { }

public class VariableStorageIntegration
{ 
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        VariableStorageSettings.RegisterVariableType<MyCustomVariable>();
    }
}
```

### Handling Non-Serializable Types

If you have a type that Unity cannot serialize directly, use `Variable<T, TSerializable>` and register a converter.

1.  **Define the Variable Class**:

```csharp
[Serializable]
[VariableTypeName("dateTime")]
public class DateTimeVariable : Variable<DateTime, SerializableDateTime> { }
```

2.  **Register the Converter**:

You need to register the converter before using the variable, typically in an initialization phase.
You should do this in two places: on the very start of your game (for runtime) and after each script
compilation (for editor purpose)

```csharp
VariableStorageSettings.RegisterConverter<DateTime, SerializableDateTime>(
    dateTime => new SerializableDateTime(dateTime), // To Serializable
    serializable => serializable.ToDateTime()       // From Serializable
);
```

## Supported Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`
