using UnityEngine;

/// <summary>
/// Handles content size fitting for nested objects
///
/// Ruben Sanchez
/// </summary>
public class NestedContentSizeFitter : MonoBehaviour
{
    [Tooltip("Default rect whose dimensions will be used to grow the parent Rect")]
    [SerializeField] private RectTransform defaultRect;
    [SerializeField] private RectTransform rectToGrow;

    [SerializeField] private bool growHorizontally;
    [SerializeField] private bool growVertically;

    public void Grow(float height = 0, float width = 0)
    {
        if (growVertically)
        {
            float newHeight = rectToGrow.rect.height + height == 0
                ? defaultRect.rect.height
                : height;

            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        if (growHorizontally)
        {
            float newWidth = rectToGrow.rect.width + width == 0
                ? defaultRect.rect.width
                : width;

            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void Grow()
    {
        if (growVertically)
        {
            float newHeight = rectToGrow.rect.height + defaultRect.rect.height;
            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        if (growHorizontally)
        {
            float newWidth = rectToGrow.rect.width + defaultRect.rect.width;
            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void Shrink()
    {
        if (growVertically)
        {
            float newHeight = rectToGrow.rect.height - defaultRect.rect.height;
            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        if (growHorizontally)
        {
            float newWidth = rectToGrow.rect.width - defaultRect.rect.width;
            rectToGrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}
