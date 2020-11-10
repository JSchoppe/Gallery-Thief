using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Managers a sequence of objectives.
/// </summary>
public sealed class ObjectiveManager : MonoBehaviour, IProgressCheater
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
        // This list needs to be accumulated and marked
        // after the following foreach iterator. Otherwise
        // the compiler throws a fit for modifying the collection.
        List<IObjective> markedInProgress = new List<IObjective>();
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
                    markedInProgress.Add(objective);
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
        foreach (IObjective objective in markedInProgress)
            objectiveState[objective] = Completion.InProgress;
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
    #region Cheating Implementation
    public void IncrementProgress()
    {
        foreach (IObjective objective in objectiveState.Keys)
        {
            if (objectiveState[objective] == Completion.InProgress
                && objective is IProgressCheater cheater)
            {
                cheater.IncrementProgress();
            }
        }
    }
    public void DecrementProgress()
    {
        foreach (IObjective objective in objectiveState.Keys)
        {
            if (objectiveState[objective] == Completion.InProgress
                && objective is IProgressCheater cheater)
            {
                cheater.DecrementProgress();
            }
        }
    }
    #endregion
}
