using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    [Tooltip("The game object where the intro popup window is is located.")]
    [SerializeField] private GameObject introWindow = null;
    private bool isPopupShown = true;


    // Update is called once per frame
    void Update()
    {

        if (isPopupShown)
        {
            Time.timeScale = 0f;
        }
        else
        {
            DisableIntro();
        }
    }

    public void DisableIntro()
    {
        introWindow.SetActive(false);
        Time.timeScale = 1f;

    }
}

