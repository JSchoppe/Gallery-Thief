using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtPieceScript
{
    [SerializeField] [Tooltip("How much money(float) this art piece is worth")]
    public float value = 100f;
    [SerializeField] [Tooltip("How long(float) it will take to steal the art piece")]
    public float stealTime = 3f;
}