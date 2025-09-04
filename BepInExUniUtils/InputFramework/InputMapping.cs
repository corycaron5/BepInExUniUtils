using System.Collections.Generic;

namespace BepInExUniUtils.InputFramework;

/// <summary>
/// A class used for serializing input mappings.
/// </summary>
/// <param name="inputTrigger"></param>
/// <param name="actions"></param>
public struct InputMapping(InputTrigger inputTrigger, List<string> actions)
{
    /// <summary>
    /// The input trigger to listen for.
    /// </summary>
    public InputTrigger InputTrigger = inputTrigger;
    
    /// <summary>
    /// The names of the actions to trigger when the input is pressed.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public List<string> Actions = actions;

    /// <summary>
    /// Converts the input action to a key value pair.
    /// </summary>
    /// <returns>The key value pair representing this input action.</returns>
    public KeyValuePair<InputTrigger, List<string>> ToKeyValuePair()
    {
        return new KeyValuePair<InputTrigger, List<string>>(InputTrigger, Actions);
    }
}