using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps this rect transform at an offset under another rect transform
///
/// Ruben Sanchez
/// </summary>
public class KeepOffet : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransToMove;
    [SerializeField] private RectTransform targetRectTrans;
    [SerializeField] private Vector2 offset;
    [SerializeField] private CanvasScaler scaler;

    private void Awake()
    {
        offset *= (new Vector2(Screen.width, Screen.height)) / scaler.referenceResolution;
    }

    private void Update()
    {
        rectTransToMove.position = (Vector2)targetRectTrans.position + (Vector2.down * targetRectTrans.rect.height) + offset;
    }
}
