using UnityEngine;

/// <summary>
/// Handles content size fitting for nested objects
///
/// Ruben Sanchez
/// </summary>
public class NestedContentSizeFitter : MonoBehaviour
{
    #region Private Fields
    [Tooltip("Rect to grow or shrink")]
    [SerializeField] private RectTransform rectToUpdate;

    [Tooltip("Showld the RectToGrow be modified horizontally?")]
    [SerializeField] private bool resizeHorizontally;

    [Tooltip("Showld the RectToGrow be modified vertically?")]
    [SerializeField] private bool resizeVertically;

    [Header("Auto Matching")]
    [SerializeField] private bool autoMatchChildren;

    [Header("Rect Matching")]
    [Tooltip("If not automatching, Default rect whose dimensions will be used to incrementally update the parent Rect")]
    [SerializeField] private Rect defaultRect;

    #endregion

    #region MonoBehavior Callbacks
    private void OnEnable()
    {
        // subscribe to whatever events that needs to lead to rectToUpdate being resized
        if (autoMatchChildren)
        {
            PublishingManagerUI.Instance.OnUIElementRemoved += AutoMatch;
            PublishingManagerUI.Instance.OnUIElementAdded += AutoMatch;
        }

        else
        {
            PublishingManagerUI.Instance.OnUIElementRemoved += Shrink;
            PublishingManagerUI.Instance.OnUIElementAdded += Grow;
        }
    }

    private void OnDisable()
    {
        // unsubscribe on disable
        if (autoMatchChildren)
        {
            PublishingManagerUI.Instance.OnUIElementRemoved -= AutoMatch;
            PublishingManagerUI.Instance.OnUIElementAdded -= AutoMatch;
        }

        else
        {
            PublishingManagerUI.Instance.OnUIElementRemoved -= Shrink;
            PublishingManagerUI.Instance.OnUIElementAdded -= Grow; 
        }
    }
    #endregion

    #region Public Methods
    public void AutoMatch()
    {
        float newHeight = 0;
        float newWidth = 0;

        // get rect children of rectToUpdate
        RectTransform[] rectsToMatch = rectToUpdate.GetComponentsInChildren<RectTransform>();

        // get the sum of dimension for the children of rectToUpdate
        foreach (var r in rectsToMatch)
        {
            newHeight += r.rect.height;
            newWidth += r.rect.width;
        }

        // resize to match the children
        if (resizeHorizontally)
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        if (resizeVertically)
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }

    public void Grow(float height = 0, float width = 0)
    {
        // grow vertically by the given height or the given default rect height
        if (resizeVertically)
        {
            float newHeight = rectToUpdate.rect.height + height == 0
                ? defaultRect.height
                : height;

            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        // grow horizontally by the given width or the given default rect width
        if (resizeHorizontally)
        {
            float newWidth = rectToUpdate.rect.width + width == 0
                ? defaultRect.width
                : width;

            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void Grow()
    {
        // grow vertically by the defaultRect height
        if (resizeVertically)
        {
            float newHeight = rectToUpdate.rect.height + defaultRect.height;
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        // grow horizontally by the given defaultRect width
        if (resizeHorizontally)
        {
            float newWidth = rectToUpdate.rect.width + defaultRect.width;
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void Shrink()
    {
        // shrink by the given defaultRect height
        if (resizeVertically)
        {
            float newHeight = rectToUpdate.rect.height - defaultRect.height;
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        // shrink by the given defaultRect width
        if (resizeHorizontally)
        {
            float newWidth = rectToUpdate.rect.width - defaultRect.width;
            rectToUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    } 
    #endregion
}
