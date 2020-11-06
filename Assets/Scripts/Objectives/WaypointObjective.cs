using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class WaypointObjective : Objective
{
    public override event Action ObjectiveComplete;

    public override void OnObjectiveEnabled()
    {
        GameplayHUDSingleton.WaypointHUD();
    }
}
