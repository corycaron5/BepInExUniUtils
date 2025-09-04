using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace BepInExUniUtils.InputFramework;

/// <summary>
/// A struct representing an input.
/// </summary>
/// <param name="key">The main key to listen for.</param>
/// <param name="modifier">A modifier key that must be held when pressing the main key for the input to fire. Can be ommited.</param>
[JsonObject(MemberSerialization.OptIn)]
public struct InputTrigger(KeyCode key, KeyCode modifier = KeyCode.None): IEquatable<InputTrigger>
{
    /// <summary>
    /// Modifier that must be held when pressing the main key for the input to fire.
    /// </summary>
    [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
    public readonly KeyCode Modifier = modifier;
    
    /// <summary>
    /// Main key to listen for.
    /// </summary>
    [JsonProperty, JsonConverter(typeof(StringEnumConverter))]
    public readonly KeyCode Key = key;

    /// <summary>
    /// Checks if the input is currently pressed.
    /// Will return true every tick while the key is held down.
    /// </summary>
    /// <returns>True if the input is currently pressed.</returns>
    public bool IsPressed()
    {
        if (Input.GetKey(Key))
        {
            UniUtilsPlugin.Logger.LogDebug($"{Key} is pressed. Mod is pressed: {Modifier == KeyCode.None || Input.GetKey(Modifier)}");
            return Modifier == KeyCode.None || Input.GetKey(Modifier);
        }
        return false;
    }

    /// <summary>
    /// Checks if the input is currently down.
    /// Only triggers once until the main key is released.
    /// </summary>
    /// <returns>True if the input is currently down.</returns>
    public bool IsDown()
    {
        if (Input.GetKeyDown(Key))
        {
            UniUtilsPlugin.Logger.LogDebug($"{Key} is down. Mod is pressed: {Modifier == KeyCode.None || Input.GetKey(Modifier)}");
            return Modifier == KeyCode.None || Input.GetKey(Modifier);
        }
        return false;
    }

    /// <summary>
    /// Converts the input to a string.
    /// </summary>
    /// <returns>The string representation of the input.</returns>
    public override string ToString()
    {
        return Modifier == KeyCode.None ? $"{Key}" : $"{Key}+{Modifier}";
    }

    /// <summary>
    /// Checks if this InputTrigger is equal to another InputTrigger.
    /// </summary>
    /// <param name="other">The other InputTrigger to compare to.</param>
    /// <returns>True if the two inputs are equal.</returns>
    public bool Equals(InputTrigger other)
    {
        return Modifier == other.Modifier && Key == other.Key;
    }

    /// <summary>
    /// Checks if this InputTrigger is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the object is a InputTrigger and is equal to this one.</returns>
    public override bool Equals(object obj)
    {
        return obj is InputTrigger other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this InputTrigger.
    /// </summary>
    /// <returns>The hash code for this InputTrigger.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)Modifier, (int)Key);
    }

    /// <summary>
    /// Attempts to convert a string to a InputTrigger.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="success">True if the value was successfully parsed.</param>
    /// <returns>The parsed InputTrigger, or a default value if parsing failed.</returns>
    public static InputTrigger TryParse(string input, out bool success)
    {
        string[] keys = input.Split('+');
        if (keys.Length is 1 or 2)
        {
            bool succ = Enum.TryParse(keys[0].Trim(), true, out KeyCode code);
            if (succ)
            {
                KeyCode mainKey = code;
                if (keys.Length == 2)
                {
                    bool succ2 = Enum.TryParse(keys[1].Trim(), true, out KeyCode mod);
                    if (!succ2)
                    {
                        UniUtilsPlugin.Logger.LogError($"Failed to parse modifier {keys[1]}");
                        success = false;
                        return new InputTrigger(KeyCode.None);
                    }
                    success = true;
                    return new InputTrigger(mainKey, mod);
                }
                else
                {
                    success = true;
                    return new InputTrigger(mainKey);
                }
            }
            else
            {
                UniUtilsPlugin.Logger.LogError($"Failed to parse key {keys[0]}");
                success = false;
                return new InputTrigger(KeyCode.None);
            }
        }
        else
        {
            UniUtilsPlugin.Logger.LogError($"Failed to parse input {input}");
            success = false;
            return new InputTrigger(KeyCode.None);
        }
    }
}