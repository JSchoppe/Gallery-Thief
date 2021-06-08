using System;

/// <summary>
/// One dimensional floating point range.
/// </summary>
[Serializable]
public struct Range
{
    #region Fields
    /// <summary>
    /// The lower end of the range.
    /// </summary>
    public float min;
    /// <summary>
    /// The upper end of the range.
    /// </summary>
    public float max;
    #endregion
    #region Accessor Functions
    /// <summary>
    /// Calculates the average for this range.
    /// </summary>
    public float Average
    {
        get { return (min + max) / 2f; }
    }
    /// <summary>
    /// Calculates the length of this range.
    /// </summary>
    public float Length
    {
        get { return max - min; }
    }
    #endregion
}