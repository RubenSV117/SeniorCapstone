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
    [SerializeField] private GameObject searchResultsPanel;
    [SerializeField] private GameObject favoritesPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        canvas?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void ShowFavorites()
    {
        Debug.Log("ShowFavorites() invoked.");
        searchResultsPanel.SetActive(false);
        favoritesPanel.SetActive(true);
    }

    public void HideFavorites()
    {
        Debug.Log("HideFavorites() invoked.");
        searchResultsPanel.SetActive(true);
        favoritesPanel.SetActive(false);
    }

    public bool FavoritesIsActive()
    {
        return favoritesPanel.activeInHierarchy;
    }
}
