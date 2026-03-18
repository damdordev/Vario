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
## Settings file

To modify default value behavior you can create a file <b>vario_settings.json</b> in any <b>Resources</b> folder.

### Creating custom types

```csharp
namespace MyNamespace
{
    [Serializable]
    public class MyCustomVarioVariable : VarioVariable<MyCustomType> { }
}
```

then in your <b>vario_settings.json</b> you need to add that type:

```json
{
  "types": {
    "myCustomType": "MyNamespace.MyCustomVarioVariable"
  }
}
```

### Global Storage

You can register global storages that are accessible throughout your application.
To do this create scriptable object <b>VarioGlobalStorage</b> in <b>Resources</b> folder
(Create/Damdor/Vario/Global storage) and add it to settings:

```json
{
  "globalStorages": [
    "MyCustomGlobalStorage"
  ]  
}
```

### Default global storages

Vario has default global storages you can use. Right now only one is supported: <b>Vario_DefaultEasing</b>.
To use it you need to add it to <b>vario_settings.json</b>:

```json
{
  "globalStorages": [
    "Vario_DefaultEasing"
  ]  
}
```    

## Supported Types

The package comes with many built-in variable types including:
*   Primitives: `bool`, `int`, `float`, `string`
*   Unity Types: `Vector2`, `Vector3`, `Color`, `Rect`, `LayerMask`
*   Assets: `GameObject`, `Texture`, `Sprite`, `Material`, `AudioClip`, `AnimationCurve`, `TextAsset`
