using BepInExUniUtils.InputFramework;

namespace BepInExUniUtils.Actions;

/// <summary>
/// A dummy action that does nothing.
/// </summary>
/// <param name="id">The ID of the action.</param>
public class DummyAction(string id) :AbstractAction(id)
{
    /// <summary>
    /// Does nothing.
    /// </summary>
    public override void Execute()
    {
        UniUtilsPlugin.Logger.LogDebug("This does nothing");
    }
}