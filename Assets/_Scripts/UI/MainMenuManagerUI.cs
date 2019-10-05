using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManagerUI : MonoBehaviour, IPanel
{
    public static MainMenuManagerUI Instance;

    [SerializeField] private GameObject canvas;

    [Header("Main Display UI Elements")]
    [SerializeField] private GameObject header; // bar at the top
    [SerializeField] private GameObject navigationPanel;

    public event EventHandler CloseablePanels; // Panels that implement the IPanel interface so they can be closed.

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Enable()
    {
        canvas.SetActive(true);
    }

    public void Disable()
    {
        canvas.SetActive(false);
    }

    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        throw new System.NotImplementedException();
    }

    ///// <summary>
    ///// Enables the 'Favorites' panel on the main menu.
    ///// </summary>
    //public void ShowFavorites()
    //{
    //    Debug.Log("ShowFavorites() invoked.");
    //    favoritesPanel.SetActive(true);
    //}

    ///// <summary>
    ///// Disables the 'Favorites' panel on the main menu.
    ///// </summary>
    //public void HideFavorites()
    //{
    //    Debug.Log("HideFavorites() invoked.");
    //    favoritesPanel.SetActive(false);
    //}

    ///// <summary>
    ///// Enables the 'Search Results' panel on the main menu.
    ///// </summary>
    //public void ShowSearchResults()
    //{
    //    Debug.Log("ShowSearchResults() invoked.");
    //    //searchResultsPanel.SetActive(true);
    //}

    ///// <summary>
    ///// Disables the 'Search Results' panel on the main menu.
    ///// </summary>
    //public void HideSearchResults()
    //{
    //    Debug.Log("HideSearchResults() invoked.");
    //    //searchResultsPanel.SetActive(false);
    //}

    //public bool FavoritesIsActive()
    //{
    //    return favoritesPanel.activeInHierarchy;
    //}

    /// <summary>
    /// Close any active IPanels before enabling a new one.
    /// </summary>
    public void CloseAllPanels()
    {
        CloseablePanels?.Invoke(this, null);
    }
}
