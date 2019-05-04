using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages UI for the profile canvas
///
/// Ruben Sanchez
/// </summary>
public class ProfileManagerUI : MonoBehaviour, IPanel
{
    public static ProfileManagerUI Instance;

    public delegate void ProfileEvent(string param);
    public static event ProfileEvent OnNewUserName;

    [SerializeField] private InputField usernameInputField;
    [SerializeField] private GameObject canvas;

    private string userName;

    private void Awake()
    {
        // auto complete the username if one has been previously entered
        if (PlayerPrefs.HasKey("Username"))
            usernameInputField.text = PlayerPrefs.GetString("Username");

        if (Instance == null)
            Instance = this;
    }

    public void UpdateUsername(string newName)
    {
        userName = newName;

        PlayerPrefs.SetString("Username", newName);

        OnNewUserName?.Invoke(userName);
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
    }

    public void Refresh()
    {
    }
}
