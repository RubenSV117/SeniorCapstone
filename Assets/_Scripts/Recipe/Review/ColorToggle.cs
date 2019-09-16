using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles color of attached 
///
/// Ruben Sanchez
/// </summary>
public class ColorToggle : MonoBehaviour
{
    public delegate void Toggle(ColorToggle colorToggle);
    public event Toggle OnToggle;

    private bool isOff;
    public bool IsOff
    {
        get => isOff;

        private set
        {
            isOff = value;

            image.color = isOff
                ? offColor
                : onColor;

        }
    }

    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    
    private Image image;

    private void Awake()
    {
        if (gameObject.GetComponent<Image>())
        {
            image = gameObject.GetComponent<Image>();
            image.color = onColor;
        }
    }

    public void ToggleColor()
    {
        IsOff = !IsOff;
        OnToggle?.Invoke(this);
    }

    public void TurnOn()
    {
        IsOff = false;
    }

    public void TurnOff()
    {
        IsOff = true;
    }
}
