using System.Collections.Generic;
using System.IO;
using BepInExUniUtils.InputFramework;
using Newtonsoft.Json;
using UnityEngine;

namespace BepInExUniUtils.Configuration;

/// <summary>
/// Config for mapping inputs to custom actions.
/// </summary>
/// <param name="configName">Name of the config file.</param>
/// <param name="dirPath">Directory path for the config file.</param>
[JsonObject(MemberSerialization.OptIn)]
public class InputConfig(string configName, string dirPath) : JsonConfigBase(configName, dirPath)
{
    [JsonProperty]
    public readonly List<InputMapping> InputActions = new ();
    
    [JsonProperty]
    public readonly List<InputMapping> RepeatingInputActions = new ();

    /// <summary>
    /// Generates the default config.
    /// </summary>
    protected override void GenerateDefaultConfig()
    {
        InputActions.Add(new InputMapping(new InputTrigger(KeyCode.F9), new List<string>{"DummyAction"}));
        RepeatingInputActions.Add(new InputMapping(new InputTrigger(KeyCode.F10, KeyCode.RightShift), new List<string>{"DummyAction", "DummyAction"}));
        SaveConfig();
        OnConfigLoaded();
    }

    /// <summary>
    /// Loads this config from the file system.
    /// </summary>
    public override void LoadConfig()
    {
        string jsonText = File.ReadAllText(FilePath);
        InputConfig config = JsonConvert.DeserializeObject<InputConfig>(jsonText);
        InputActions.Clear();
        InputActions.AddRange(config.InputActions);
        RepeatingInputActions.Clear();
        RepeatingInputActions.AddRange(config.RepeatingInputActions);
        OnConfigLoaded();
    }

    /// <summary>
    /// Saves the current config to the file system.
    /// </summary>
    public override void SaveConfig()
    {
        string json = JsonConvert.SerializeObject(this, GetSerializerSettings());
        File.WriteAllText(FilePath, json);
    }
}