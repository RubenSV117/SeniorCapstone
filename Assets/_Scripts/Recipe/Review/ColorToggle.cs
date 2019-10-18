using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles color of attached 
///
/// Ruben Sanchez
/// </summary>
public class ColorToggle : MonoBehaviour
{
    #region Properties

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

    #endregion

    #region MonoBehavior Callbacks

    private void Awake()
    {
        if (gameObject.GetComponent<Image>())
        {
            image = gameObject.GetComponent<Image>();
            image.color = onColor;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    #endregion

    #region Public Methods

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

    public void SetFill(float fillAmount)
    {
        image.fillAmount = fillAmount;
    }

    #endregion
}
