using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Managers a sequence of objectives.
/// </summary>
public sealed class ObjectiveManager : MonoBehaviour
{
    #region Enums
    private enum Completion : byte
    {
        NotStarted, InProgress, Completed
    }
    #endregion
    #region Inspector Fields
    [Tooltip("All objectives in the stage.")]
    [SerializeField] private List<Objective> objectives = null;
    [Tooltip("All objectives in the stage that trigger stage completion.")]
    [SerializeField] private List<Objective> finalObjectives = null;
    #endregion
    #region Fields (Objective States)
    private Dictionary<IObjective, Completion> objectiveState;
    #endregion
    #region Objective Initialization (Scene Load)
    private void Start()
    {
        // Initialize all objectives from the inspector.
        objectiveState = new Dictionary<IObjective, Completion>();
        foreach (IObjective objective in objectives)
            objectiveState.Add(objective, Completion.NotStarted);
        // Start objectives with no prereqs.
        RefreshObjectives();
    }
    #endregion
    #region Objective Routing Logic
    private void RefreshObjectives()
    {
        foreach (IObjective objective in objectiveState.Keys)
        {
            // Should we consider starting this objective?
            if (objectiveState[objective] == Completion.NotStarted)
            {
                // Have all of the prerequisite objective been completed?
                bool canStart = true;
                foreach (IObjective prereq in objective.PrereqObjectives)
                {
                    if (objectiveState[prereq] != Completion.Completed)
                    {
                        canStart = false;
                        break;
                    }
                }
                if (canStart)
                {
                    // Start the objective.
                    objective.OnObjectiveEnabled();
                    objectiveState[objective] = Completion.InProgress;
                    // Listen for the completion of the event.
                    if (finalObjectives.Contains((Objective)objective))
                    {
                        // End stage.
                        objective.ObjectiveComplete += () =>
                        {
                            OnStageComplete(objective);
                        };
                    }
                    else
                    {
                        // Run this function again to start new objectives.
                        objective.ObjectiveComplete += () =>
                        {
                            objectiveState[objective] = Completion.Completed;
                            RefreshObjectives();
                        };
                    }
                }
            }
        }
    }
    private void OnStageComplete(IObjective completionObjective)
    {
        // TODO add stage completion logic here.
        // Should probably call something in another script.
        SceneManager.LoadScene(0);
        // TODO maybe add another objective of waypoint where
        // you must exit.
    }
    #endregion
}
