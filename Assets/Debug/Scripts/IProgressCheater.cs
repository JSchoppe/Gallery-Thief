/// <summary>
/// Defines a class that can dynamically cheat its progress.
/// </summary>
public interface IProgressCheater
{
    /// <summary>
    /// Increases a value or logical step for this class. 
    /// </summary>
    void IncrementProgress();
    /// <summary>
    /// Decreases a value or logical step for this class. 
    /// </summary>
    void DecrementProgress();
}
