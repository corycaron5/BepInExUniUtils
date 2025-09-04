# BepInExUniUtils

BepInEx Universal Utilities for modding Unity games with BepInEx.

---

## Features

- Input framework for mapping actions to Unity keycodes, including modifiers
- Simple Json configuration system for when BepInEx configs aren't powerful enough

---

## Dependencies

- [BepInEx](https://github.com/BepInEx/BepInEx)
- [NewtonsoftJson](https://github.com/JamesNK/Newtonsoft.Json)
- Netstandard 2.x (Target framework) (InputTrigger uses the HashCode utility for generating it's hashcode, to support other frameworks, clone this repository and generate the hashcode another way)

## Installation

### End Users

Download the latest version from the [Releases](https://github.com/corycaron5/BepInExUniUtils/releases) page.
Unzip and copy the `BepInExUniUtils.dll` file into the BepInEx/plugins folder.

### Developers

Currently there is no Nuget package for this, just reference the assembly directly in your IDE of choice.

---

## Input Framework

The Input Framework is pretty straight forward.  
An [InputTrigger](BepInExUtils/InputFramework/InputTrigger.cs) is mapped to a list of [AbstractActions](BepInExUtils/InputFramework/AbstractAction.cs) that are all executed sequentially when the input is detected.  
Inputs can trigger once or repeatedly.  
The [InputTrigger](BepInExUtils/InputFramework/InputTrigger.cs) is a struct containing a main [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html) that is used to detect the input, and an optional modifier [KeyCode](https://docs.unity3d.com/ScriptReference/KeyCode.html) that must be held down to trigger it.  

### Input Mappings

Inputs are mapped to actions in the Input.json config file. This file is saved directly along other BepInEx configs.  
A default file will be generated if it doesn't exist, but here is an example:
> ```
> {
>   "InputActions": [
>     {
>       "InputTrigger": {
>         "Modifier": "None",
>         "Key": "F9"
>       },
>       "Actions": [
>         "DummyAction"
>       ]
>     }
>   ],
>   "RepeatingInputActions": [
>     {
>       "InputTrigger": {
>         "Modifier": "RightShift",
>         "Key": "F10"
>       },
>       "Actions": [
>         "DummyAction",
>         "DummyAction"
>       ]
>     }
>   ]
> }
> ```

### Creating Actions

Creating new actions is as simple as extending AbstractAction and implementing the Execute method.  
The execute method has no arguments and the ID of the action is handled automatically.

> ```
> /// <summary>
> /// A dummy action that does nothing.
> /// </summary>
> /// <param name="id">The ID of the action.</param>
> public class DummyAction(string id) :AbstractAction(id)
> {
>     public override void Execute()
>     {
>         UniUtilsPlugin.Logger.LogDebug("This does nothing");
>     }
> }
> ```

### Registering Custom Actions

Actions can be manually registered by passing each `Type` that extends [AbstractActions](BepInExUtils/InputFramework/AbstractAction.cs) to the method [UniUtilsPlugin.RegisterAction(Type)](BepInExUniUtils/UniUtilsPlugin.cs#L116)  
However there is a much simpler method that uses reflection to automatically register all actions. Simply implement this method in your plugin and call it in `Awake()`.
> ```
> /// <summary>
> /// Automatically registers all actions in this assembly.
> /// </summary>
> private static void RegisterInternalActions()
> {
>     foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
>     {
>         if(type.IsSubclassOf(typeof(AbstractAction))) UniUtilsPlugin.RegisterAction(type);
>     }
> }
> ```

---

## Json Configs

Json can be a powerful tool for serializing data and allows for much more complex functionality over TOML.  
It does have a few drawbacks though, the inability to add comments and readability are the biggest when using it for configs.  
My recommendation would be to use the standard BepInEx config unless you need to serialize collections or types not supported by the standard BepInEx config.  
That being said, you can simply extend [JsonConfigBase](BepInExUtils/Configuration/JsonConfigBase.cs) and implement `GenerateDefaultConfig()`, `LoadConfig()`, and `SaveConfig()` if you want to create a Json config.  
There is also an EventHandler `ConfigLoaded` in the event you need to trigger code upon loading the config.  
You should make sure to call `OnConfigLoaded()` at the end of your `LoadConfig()` method to trigger the ConfigLoaded event.  
See [InputConfig](BepInExUtils/Configuration/InputConfig.cs) for an example implementation.
