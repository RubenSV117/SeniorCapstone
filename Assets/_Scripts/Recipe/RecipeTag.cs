using UnityEngine;

/// <summary>
/// Holds reference for tag string
///
/// Ruben Sanchez
/// </summary>
public class RecipeTag : MonoBehaviour
{
    [SerializeField] private new string tag;

    public string GetTag()
    {
        return tag;
    }
}
