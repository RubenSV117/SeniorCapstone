using UnityEngine;
using System.Collections;

/// <summary>
/// Manages Login UI
///
/// References LoginInService, implements IPanel
///
/// Ruben Sanchez
/// 2/23/19
/// </summary>
public class LoginManagerUI : MonoBehaviour, IPanel
{
    [SerializeField] private GameObject canvas;

    private string email;
    private string passsword;

    private void Awake()
    {
    }

    public void UpdateEmail(string value)
    {
        email = value;
    }

    public void UpdatePassword(string value)
    {
        passsword = value;
    }

    public void LogIn()
    {
        LoginService.Instance.SignInUserWithEmail(email, passsword);
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
}
