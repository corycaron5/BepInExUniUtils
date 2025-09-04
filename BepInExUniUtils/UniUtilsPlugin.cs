using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInExUniUtils.Configuration;
using BepInExUniUtils.InputFramework;
using HarmonyLib;
using UnityEngine;

namespace BepInExUniUtils;

/// <summary>
/// Main class for the plugin.
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class UniUtilsPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    // ReSharper disable once UnusedMember.Local
    private readonly Harmony _harmony = new (MyPluginInfo.PLUGIN_GUID);
    
    /// <summary>
    /// Name of the input config file.
    /// </summary>
    public const string InputFileName = "Input";

    private static readonly Dictionary<string, Type> ActionRegistry = new();
    internal static Dictionary<InputTrigger, List<AbstractAction>> InputActions = new ();
    internal static Dictionary<InputTrigger, List<AbstractAction>> RepeatingInputActions = new ();

    /// <summary>
    /// Input config object that handles loading and saving the input config file.
    /// </summary>
    public static readonly InputConfig Inputs = new (InputFileName, Paths.ConfigPath);
    
    /// <summary>
    /// Key to reload the input config file.
    /// </summary>
    public static ConfigEntry<KeyCode> ReloadInputsKey;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        ReloadInputsKey = Config.Bind("General", "ReloadInputsKey", KeyCode.F11, "Key to reload input config file");
        
        RegisterInternalActions();
    }
    
    private void Start()
    {
        SetupConfig();
    }
    
    /// <summary>
    /// Triggers actions, action will not be called again until the key is released. Update is called every frame.
    /// </summary>
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(ReloadInputsKey.Value))
            {
                Inputs.LoadConfig();
                return;
            }
            foreach (InputTrigger key in InputActions.Keys)
            {
                if (key.IsDown())
                {
                    foreach(AbstractAction action in InputActions[key])action.Execute();
                }
            }
        }
    }
    
    /// <summary>
    /// Triggers repeating actions. FixedUpdate is called every physics frame. (20 ticks per second default)
    /// </summary>
    private void FixedUpdate()
    {
        if (Input.anyKey)
        {
            foreach (InputTrigger key in RepeatingInputActions.Keys)
            {
                if (key.IsPressed())
                {
                    foreach(AbstractAction action in RepeatingInputActions[key])action.Execute();
                }
            }
        }
    }

    /// <summary>
    /// Loads or creates the custom configs.
    /// </summary>
    private void SetupConfig()
    {
        Inputs.ConfigLoaded += ResetInputMappings;
        Inputs.LoadOrCreateConfig();
    }
    
    /// <summary>
    /// Attempts to create an action from the registry.
    /// If the action is not found in the registry, it will try to create it from the assembly qualified name.
    /// </summary>
    /// <see cref="Type.AssemblyQualifiedName"/>
    /// <param name="name">The ID of the action to create.</param>
    /// <returns>The created action, or null if the action could not be created.</returns>
    public static AbstractAction CreateAction(string name)
    {
        bool reg = ActionRegistry.TryGetValue(name, out Type actionType);
        if(reg) return (AbstractAction)Activator.CreateInstance(actionType, name);
        actionType = Type.GetType(name);
        if (actionType == null) return null;
        return (AbstractAction)Activator.CreateInstance(actionType, name);
    }
    
    /// <summary>
    /// Adds an action to the registry.
    /// </summary>
    /// <param name="abstractActionType">The type of action to register. Should be a subclass of AbstractAction.</param>
    /// <returns>Whether the action was registered successfully.</returns>
    public static bool RegisterAction(Type abstractActionType)
    {
        if (!abstractActionType.IsSubclassOf(typeof(AbstractAction)))
        {
            Logger.LogDebug($"Failed to register action: {abstractActionType.Name} is not a subtype of AbstractAction");
            return false;
        }
        bool added = ActionRegistry.TryAdd(abstractActionType.Name, abstractActionType);
        if (!added) Logger.LogDebug("Failed to register action: " + abstractActionType.Name);
        else Logger.LogInfo("Registered action: " + abstractActionType.Name);
        return added;
    }

    /// <summary>
    /// Automatically registers all actions in this assembly.
    /// </summary>
    private static void RegisterInternalActions()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            // ReSharper disable once ArrangeStaticMemberQualifier
            if(type.IsSubclassOf(typeof(AbstractAction))) UniUtilsPlugin.RegisterAction(type);
        }
    }
    
    /// <summary>
    /// Clears the input actions.
    /// </summary>
    public static void ClearInputActions()
    {
        InputActions.Clear();
        RepeatingInputActions.Clear();
    }

    /// <summary>
    /// Clears the input actions and registers them again from the default sections in the config.
    /// </summary>
    protected static void ResetInputMappings(object sender, EventArgs eventArgs)
    {
        Logger.LogDebug("Resetting input mappings");
        ClearInputActions();
        RegisterInputActions(Inputs.InputActions, ref InputActions);
        RegisterInputActions(Inputs.RepeatingInputActions, ref RepeatingInputActions);
    }

    /// <summary>
    /// A system for converting serialized input mappings to input triggers and abstract actions.
    /// </summary>
    /// <see cref="InputMapping"/>
    /// <see cref="InputTrigger"/>
    /// <see cref="AbstractAction"/>
    /// <param name="inputMappings">The list of input actions to read from.</param>
    /// <param name="mappingsDict">The dictionary of actions to write the actions to.</param>
    public static void RegisterInputActions(List<InputMapping> inputMappings, ref Dictionary<InputTrigger, List<AbstractAction>> mappingsDict)
    {
        foreach (InputMapping inMap in inputMappings)
        {
            KeyValuePair<InputTrigger, List<string>> entry = inMap.ToKeyValuePair();
            List<AbstractAction> actions = new();
            foreach (string action in entry.Value)
            {
                AbstractAction act = CreateAction(action);
                if (act == null) continue;
                actions.Add(act);
            }
            mappingsDict.Add(entry.Key, actions);
            Logger.LogInfo($"Registered input mapping: {entry.Key} -> {string.Join(',', entry.Value)}");
        }
    }
}