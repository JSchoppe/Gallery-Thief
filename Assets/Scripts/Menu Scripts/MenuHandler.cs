﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject creditsText;
    [SerializeField] GameObject settingsMenu;
    private bool creditsScrolling;

    //loads next scene
    private void Update()
    {
        ScrollCredits();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    //shows the main menu and hides other menus, stops credits scrolling
    public void ShowMainMenu()
    {
        creditsScrolling = false;
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    //shows credits and begins scrolling, hides main menu
    public void ShowCredits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        creditsScrolling = true;
    }

    //shows settings, hides main menu
    public void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    //exits game
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }

    private void ScrollCredits()
    {
        RectTransform rectTransform = creditsText.GetComponent<RectTransform>();
        float startPos = -600; //position where credits start
        float endPos = 1200; //position where credits end
        float scrollSpeed = .8f;

        if (creditsScrolling)
        {
            if(rectTransform.anchoredPosition.y < endPos)
            {
                rectTransform.anchoredPosition += Vector2.up * scrollSpeed;
                
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPos);
            }
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPos);
        }

    }
}