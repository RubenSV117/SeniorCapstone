using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainDisplayUI : MonoBehaviour, IPanel
{
    public static MainDisplayUI Instance;

    [SerializeField] private GameObject canvas;

    //[Header("Prefabs")]
    //[SerializeField] private ;
    //[Space(20)]

    [Header("UI")]
    [SerializeField] private InputField _searchInputField;
    [SerializeField] private Button _searchButton;
	
	private string _searchInputText;
	
    static MainDisplayUI()
    {
        
    }

    public void Start()
    {
    }
	
	public void UpdateSearchInputText(string value)
	{
		_searchInputText = value;
		Debug.Log(_searchInputText);
	}

    public void SendSearchInput()
    {
        if (!string.IsNullOrWhiteSpace(_searchInputText))
        {
            Debug.Log("MainDisplayUI.SendSearchInput() - Search text wasn't null or whitespace");
            // send text to something else for processing
        }
        else
        {
            Debug.Log("MainDisplayUI.SendSearchInput() - Search text was null or whitespace");
        }
        ShowIncompleteDialog();
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

    // for debugging purposes
    public void ShowIncompleteDialog()
    {
        // UnityEditor.EditorUtility.DisplayDialog("Error", "This feature isn't fully implemented yet, sorry!", "OK");
    }
}
