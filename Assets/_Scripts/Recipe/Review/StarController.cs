using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages rating stars
///
/// Ruben Sanchez
/// </summary>
public class StarController : MonoBehaviour
{
    [SerializeField] private int maxStars = 5;
    [SerializeField] private GameObject starPrefab;

    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    private List<ColorToggle> stars = new List<ColorToggle>();

    private void Awake()
    {
        if (layoutGroup == null)
            return;

        for (int i = 0; i < maxStars; i++)
        {
            ColorToggle color = Instantiate(starPrefab, layoutGroup.transform).GetComponent<ColorToggle>();
            color.OnToggle += SetStars;
            stars.Add(color);
        }
    }

    private int GetNumberOfActiveStars()
    {
        return stars.Count(star => (!star.IsOff));
    }


    private void SetStars(ColorToggle colorToggle)
    {
        int indexOfToggledStar = stars.IndexOf(colorToggle);

        print("Toggled Star: " + indexOfToggledStar);

        foreach (var star in stars)
        {
            print("star: " + star.transform.GetSiblingIndex());

            if (colorToggle.IsOff
                && star.transform.GetSiblingIndex() > indexOfToggledStar)
                    star.TurnOff();

            else if (star.transform.GetSiblingIndex() < indexOfToggledStar)
                star.TurnOn();
        }
    }
}
