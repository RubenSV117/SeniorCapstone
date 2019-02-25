using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [SerializeField] private Animator anim;
    [SerializeField] private Text messageText;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowNotification(string message)
    {
        messageText.text = message;

        anim.Play("Notification");
    }
}
