using System;
using System.IO;
using Newtonsoft.Json;

namespace BepInExUniUtils.Configuration;

/// <summary>
/// Base class for JSON configurations.
/// </summary>
/// <param name="configName">Name of the config file.</param>
/// <param name="dirPath">Directory path for the config file.</param>
[JsonObject(MemberSerialization.OptIn)]
public abstract class JsonConfigBase(string configName, string dirPath)
{
    public readonly string ConfigName = configName;
    public readonly string DirPath = dirPath;
    public event EventHandler ConfigLoaded;

    /// <summary>
    /// Abstract method to generates the default config.
    /// Must be overridden by the derived class.
    /// </summary>
    protected abstract void GenerateDefaultConfig();
    
    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Abstract method to load the config.
    /// Must be overridden by the derived class.
    /// </summary>
    public abstract void LoadConfig();
    
    /// <summary>
    /// Abstract method to save the config.
    /// Must be overridden by the derived class.
    /// </summary>
    public abstract void SaveConfig();
    
    /// <summary>
    /// Filepath of the config file.
    /// </summary>
    /// <returns>String with the filepath for this config.</returns>
    public string FilePath => Path.Combine(DirPath, $"{ConfigName}.json");
    
    /// <summary>
    /// Checks if this config exists on the file system.
    /// </summary>
    /// <returns>True if the file is found.</returns>
    public bool DoesConfigExist() => File.Exists(FilePath);

    /// <summary>
    /// Loads or creates the config.
    /// </summary>
    /// <returns>This config after being loaded or created.</returns>
    public virtual JsonConfigBase LoadOrCreateConfig()
    {
        if (!DoesConfigExist()) GenerateDefaultConfig();
        else LoadConfig();
        return this;
    }

    /// <summary>
    /// Gets the serializer settings for the config.
    /// The default settings are indented formatting.
    /// </summary>
    /// <returns>The serializer settings for the config.</returns>
    public virtual JsonSerializerSettings GetSerializerSettings()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Formatting = Formatting.Indented;
        return settings;
    }
    
    /// <summary>
    /// Event invocation for when the config is loaded.
    /// </summary>
    protected virtual void OnConfigLoaded() => ConfigLoaded?.Invoke(this, EventArgs.Empty);
}