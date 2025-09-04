namespace BepInExUniUtils.InputFramework;

/// <summary>
/// An abstract class representing an action.
/// </summary>
/// <param name="id">The ID of the action.</param>
public abstract class AbstractAction(string id)
{
    /// <summary>
    /// The ID of the action.
    /// Automatically set when the action is created.
    /// </summary>
    public readonly string Id = id;
    
    /// <summary>
    /// This function is called when the action is executed.
    /// This must be overwritten by the derived class.
    /// </summary>
    public abstract void Execute();
}