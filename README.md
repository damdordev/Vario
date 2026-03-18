# Vario

A flexible and serializable variable storage system for Unity. This package allows you to manage collections 
of variables with support for various types.

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

## Global Storage

You can register global storages that are accessible throughout your application. This is useful for shared configuration or game-wide state.

```csharp
using UnityEngine;
using Damdor.Vario;

public class GameInitializer : MonoBehaviour
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        InitializeGlobalVariableStorages();
    }
    
    private void Awake()
    {
        InitializeGlobalVariableStorages();
    }
    
    private void InitializeGlobalVariableStorages()
    {
        // find your custom global storage
        GlobalVarioStorage myGlobalStorage = ...
        
        // Register a custom global storage
        VarioSettings.RegisterGlobalStorage(myGlobalStorage.Storage);

        // Register a default global storage (e.g., Easing curves)
        VarioSettings.RegisterGlobalStorage(VarioDefaultStorage.Easing);        
    }
}
```

## Custom Types

### Creating a Custom Variable

To add a new type, inherit from `Variable<T>`.

```csharp
using System;
using Damdor.Vario;
using UnityEngine;

[Serializable]
[VarioVariableName("myCustomType")]
public class MyCustomVarioVariable : VarioVariable<MyCustomType> { }

public class VariableStorageIntegration
{ 
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        VarioSettings.RegisterVariableType<MyCustomVarioVariable>();
    }
}
```

## Supported Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`
