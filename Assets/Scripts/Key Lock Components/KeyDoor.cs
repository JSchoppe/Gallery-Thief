using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class KeyDoor : MonoBehaviour
{
    [Tooltip("The key identity that unlocks this door.")]
    [SerializeField] private KeyID doorIdentity = KeyID.A;


    private void OnTriggerEnter(Collider other)
    {
        
    }
}
