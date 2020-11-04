using System;
using System.Collections.Generic;
using UnityEngine;

#region Objective Interfaces
/// <summary>
/// Describes a stage objective (chunk of gameplay logic).
/// </summary>
public interface IObjective
{
    /// <summary>
    /// Which objectives must be completed to trigger this one.
    /// If empty the objective will be activated when the scene is loaded.
    /// </summary>
    List<IObjective> PrereqObjectives { get; }
    /// <summary>
    /// Initializes the objective in the scene.
    /// </summary>
    void OnObjectiveEnabled();
    /// <summary>
    /// Called once the objective has been completed.
    /// </summary>
    event Action ObjectiveComplete;
    // TODO Add failable objectives?
}
#endregion

/// <summary>
/// Base for gameplay objectives.
/// </summary>
public abstract class Objective : MonoBehaviour, IObjective
{
    #region Inspector Fields
    [Header("Objective Order")]
    [Tooltip("Objectives that must be completed before this one is triggered.")]
    [SerializeField] private List<Objective> prereqObjectives = null;
    #endregion
    #region IObjective Properties
    /// <summary>
    /// The prerequisite objectives to this objective.
    /// </summary>
    public List<IObjective> PrereqObjectives
    {
        // TODO there is probably a linq feature that does this;
        // otherwise abstract into extension method.
        get
        {
            List<IObjective> prereqs = new List<IObjective>();
            foreach (Objective objective in prereqObjectives)
                prereqs.Add(objective);
            return prereqs;
        }
    }
    #endregion
    #region Subclass Options
    /// <summary>
    /// Called when this objective has been completed.
    /// </summary>
    public abstract event Action ObjectiveComplete;
    /// <summary>
    /// Called to initialize an objective when it becomes active.
    /// </summary>
    public virtual void OnObjectiveEnabled() { }
    #endregion
}
