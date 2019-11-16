using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [SerializeField] private Animator anim;
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject loadingPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        if (LoginManagerUI.Instance != null) LoginManagerUI.Instance.OnAccountActionAttempt += ShowNotification;
    }

    private void OnDisable()
    {
        if (LoginManagerUI.Instance != null) LoginManagerUI.Instance.OnAccountActionAttempt -= ShowNotification;
    }

    public void SetLoadingPanel(bool isActive)
    {
        loadingPanel.SetActive(isActive);
    }

    public void ShowNotification(string message)
    {
        messageText.text = message;
        anim.Play("Notification");
    }
}
